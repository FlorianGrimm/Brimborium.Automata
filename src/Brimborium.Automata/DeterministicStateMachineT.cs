namespace Brimborium.Automata;

public class State<TEvent> {

    private readonly List<StateTransition<TEvent>> _ListTransitions = new();
    private readonly string _Name;
    public string Name => this._Name;

    public State(string name) {
        this._Name = name;
    }

    public StateTransition<TEvent> WithTransition(string name) {
        var transition = new StateTransition<TEvent>(name);
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

    public virtual void OnEnter(TEvent currentEvent) {
    }

    public virtual void OnExit(TEvent nextEvent) {
    }
}

public class StateTransition<TEvent> {
    private State? _ToState;
    private readonly string _EventName;

    public State? ToState => this._ToState;

    public string EventName => this._EventName;

    public StateTransition(string eventName) {
        this._EventName = eventName;
    }

    public StateTransition<TEvent> To(State state) {
        this._ToState = state;
        return this;
    }

    public virtual void OnExecute(TEvent currentEvent) {
    }

    public TEvent? Condition { get; set; }
    public bool CheckEvent<TEvent>(TEvent currentEvent) {
        if (this.Condition is { } condition) { 
            return EqualityComparer<TEvent>.Default.Equals(currentEvent, condition);
        } else {
            return false;
        }
    }
}

public class StateMachineBuilder<TEvent> {
    private readonly List<State<TEvent>> _ListState = new();
    private State<TEvent> _InitialState;
    private State<TEvent> _FinalState;
    private State<TEvent> _ErrorState;

    public List<State<TEvent>> ListState => this._ListState;

    public State<TEvent> InitialState => _InitialState;

    public State<TEvent> FinalState => _FinalState;

    public State<TEvent> ErrorState => this._ErrorState;

    public StateMachineBuilder() {
        this._ListState.Add(this._InitialState = new State<TEvent>("InitialState"));
        this._ListState.Add(this._FinalState = new State<TEvent>("FinalState"));
        this._ListState.Add(this._ErrorState = new State<TEvent>("ErrorState"));
    }

    public State<TEvent> State(State<TEvent> state) {
        this._ListState.Add(state);
        return state;
    }

    public State<TEvent> State(string name) {
        State<TEvent> state = new State<TEvent>(name);
        this._ListState.Add(state);
        return state;
    }
}

public class DeterministicStateMachine<TEvent> {
    private readonly StateMachineBuilder<TEvent> _Builder;
    private State<TEvent> _State;
    public DeterministicStateMachine(StateMachineBuilder<TEvent> builder) {
        this._Builder = builder;
        this._State = builder.InitialState;
    }
    public State<TEvent> State { get => this._State; set => this._State = value; }
    public StateMachineBuilder<TEvent> Builder => this._Builder;
    public State<TEvent> Next(TEvent currentEvent) {
        var transition = this._State.GetTransition(currentEvent);
        if (transition is null) {
            throw new InvalidOperationException($"No transition defined for event '{eventName}' in state '{this._State.Name}'.");
        }
        this._State.OnExit(currentEvent);
        transition.OnExecute(currentEvent);
        var nextState = transition.ToState;
        if (nextState is null) {
            nextState = this._Builder.ErrorState;
        }
        {
            this._State = nextState;
            this._State.OnEnter(currentEvent);
            return this._State;
        }
    }
}
