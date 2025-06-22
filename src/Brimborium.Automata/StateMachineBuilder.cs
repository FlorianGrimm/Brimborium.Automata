namespace Brimborium.Automata;

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
