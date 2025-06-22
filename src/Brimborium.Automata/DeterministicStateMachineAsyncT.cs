namespace Brimborium.Automata;
#if true
public enum StateKind {
    Initial,
    Final,
    Error,
    Normal
}

public class State<TEvent> {

    private readonly List<StateTransition<TEvent>> _ListTransitions = new();
    private readonly string _Name;
    private readonly StateKind _StateKind;

    public string Name => this._Name;

    public StateKind StateKind => this._StateKind;

    public State(string name, StateKind stateKind = StateKind.Normal) {
        this._Name = name;
        this._StateKind = stateKind;
    }

    public StateTransition<TEvent> WithTransition(string name) {
        var transition = new StateTransition<TEvent>(name);
        this._ListTransitions.Add(transition);
        return transition;
    }

    public StateTransition<TEvent> WithTransition(StateTransition<TEvent> transition) {
        this._ListTransitions.Add(transition);
        return transition;
    }

    public StateTransition<TEvent>? GetTransition(TEvent currentEvent) {
        foreach (var transition in this._ListTransitions) {
            if (transition.CheckEvent(currentEvent)) {
                return transition;
            }
        }
        return null;
    }

    public virtual ValueTask OnEnterAsync(TEvent currentEvent, State<TEvent>? previousState) {
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask OnExitAsync(TEvent nextEvent, State<TEvent> nextState) {
        return ValueTask.CompletedTask;
    }
}

public interface IStateTransitionChecker<TEvent> {
    bool CheckEvent(TEvent currentEvent);
}

public class StateTransition<TEvent> : IStateTransitionChecker<TEvent> {
    private readonly string _EventName;
    private State<TEvent>? _ToState;

    public State<TEvent>? ToState => this._ToState;

    public string EventName => this._EventName;

    public StateTransition(string eventName) {
        this._EventName = eventName;
    }

    public StateTransition<TEvent> To(State<TEvent> state) {
        this._ToState = state;
        return this;
    }

    public virtual ValueTask OnExecuteAsync(TEvent currentEvent, State<TEvent> nextState) {
        return ValueTask.CompletedTask;
    }


    public virtual bool CheckEvent(TEvent currentEvent) {
        return false;
    }
}

public class StateTransitionWithChecker<TEvent> : StateTransition<TEvent> {
    private readonly IStateTransitionChecker<TEvent> _Checker;

    public StateTransitionWithChecker(
        string eventName,
        IStateTransitionChecker<TEvent> checker) : base(eventName) {
        this._Checker = checker;
    }

    public override bool CheckEvent(TEvent currentEvent) {
        return this._Checker.CheckEvent(currentEvent);
    }
}
public class StateMachineBuilder<TEvent> {
    private readonly List<State<TEvent>> _ListState = new();
    private readonly State<TEvent> _InitialState;
    private readonly State<TEvent> _FinalState;
    private readonly State<TEvent> _ErrorState;

    public List<State<TEvent>> ListState => this._ListState;

    public State<TEvent> InitialState => _InitialState;

    public State<TEvent> FinalState => _FinalState;

    public State<TEvent> ErrorState => this._ErrorState;

    public StateMachineBuilder(
        State<TEvent>? initialState = default,
        State<TEvent>? finalState = default,
        State<TEvent>? errorState = default
        ) {
        this._ListState.Add(this._InitialState = (initialState ?? new State<TEvent>("InitialState", StateKind.Initial)));
        this._ListState.Add(this._FinalState = (finalState ?? new State<TEvent>("FinalState", StateKind.Final)));
        this._ListState.Add(this._ErrorState = (errorState ?? new State<TEvent>("ErrorState", StateKind.Error)));
    }

    public State<TEvent> State(string name) {
        var state = new State<TEvent>(name, StateKind.Normal);
        this._ListState.Add(state);
        return state;
    }

    public State<TEvent> State(State<TEvent> state) {
        this._ListState.Add(state);
        return state;
    }
}

public class StateMachine<TEvent> {
    private readonly StateMachineBuilder<TEvent> _Builder;
    private State<TEvent>? _State;
    public StateMachine(StateMachineBuilder<TEvent> builder) {
        this._Builder = builder;
    }
    public State<TEvent> State {
        get => this._State ?? this._Builder.InitialState;
    }

    public StateMachineBuilder<TEvent> Builder => this._Builder;

    public async Task<State<TEvent>> NextAsync(TEvent currentEvent) {
        var currentState = this._State;
        if (currentState is null) {
            currentState = this._Builder.InitialState;
            this._State = currentState;
            await this._State.OnEnterAsync(currentEvent, default);
        }
        var transition = currentState.GetTransition(currentEvent);
        var nextState = transition?.ToState ?? this._Builder.ErrorState;
        if (ReferenceEquals(currentState, nextState)) {
            if (transition is { }) {
                await transition.OnExecuteAsync(currentEvent, nextState);
            }
            return currentState;
        } else {
            await currentState.OnExitAsync(currentEvent, nextState);
            if (transition is { }) {
                await transition.OnExecuteAsync(currentEvent, nextState);
            }
            this._State = nextState;
            await nextState.OnEnterAsync(currentEvent, currentState);
            return nextState;
        }
    }
}
#endif