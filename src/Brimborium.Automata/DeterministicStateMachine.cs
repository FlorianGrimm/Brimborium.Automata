namespace Brimborium.Automata;
public class State {

    private readonly List<StateTransition> _ListTransitions = new();
    private readonly string _Name;
    public string Name => this._Name;

    public State(string name) {
        this._Name = name;
    }

    public StateTransition WithTransition(string eventName) {
        var transition = new StateTransition(eventName);
        this._ListTransitions.Add(transition);
        return transition;
    }

    public StateTransition? GetTransition(string eventName) {
        foreach (var transition in this._ListTransitions) {
            if (transition.EventName == eventName) {
                return transition;
            }
        }
        return null;
    }

    public virtual void OnEnter() {
    }

    public virtual void OnExit() {
    }
}
public class StateTransition {
    private State? _ToState;
    private readonly string _EventName;

    public State? ToState => this._ToState;

    public string EventName => this._EventName;

    public StateTransition(string eventName) {
        this._EventName = eventName;
    }

    public StateTransition To(State state) {
        this._ToState = state;
        return this;
    }

    public virtual void OnExecute() {
    }
}

public class StateMachineBuilder {
    private readonly List<State> _ListState = new();
    private State _InitialState;
    private State _FinalState;
    private State _ErrorState;

    public List<State> ListState => this._ListState;

    public State InitialState => _InitialState;

    public State FinalState => _FinalState;

    public State ErrorState => this._ErrorState;

    public StateMachineBuilder() {
        this._ListState.Add(this._InitialState = new State("InitialState"));
        this._ListState.Add(this._FinalState = new State("FinalState"));
        this._ListState.Add(this._ErrorState = new State("ErrorState"));
    }

    public State State(State state) {
        this._ListState.Add(state);
        return state;
    }

    public State State(string name) {
        State state = new State(name);
        this._ListState.Add(state);
        return state;
    }

    //public DeterministicStateMachine Build() {
    //    var result = new DeterministicStateMachine(this);
    //    return result;
    //}
}


public class DeterministicStateMachine {
    private readonly StateMachineBuilder _Builder;
    private State _State;

    public DeterministicStateMachine(StateMachineBuilder builder) {
        this._Builder = builder;
        this._State = builder.InitialState;
    }

    public State State { get => this._State; set => this._State = value; }

    public StateMachineBuilder Builder => this._Builder;

    public State Next(string eventName) {
        var transition = this._State.GetTransition(eventName);
        if (transition is null) {
            throw new InvalidOperationException($"No transition defined for event '{eventName}' in state '{this._State.Name}'.");
        }
        this._State.OnExit();

        transition.OnExecute();

        var nextState = transition.ToState;
        if (nextState is null) {
            nextState = this._Builder.ErrorState;
        } 

        {
            this._State = nextState;
            this._State.OnEnter();
            return this._State;
        }
    }
}
