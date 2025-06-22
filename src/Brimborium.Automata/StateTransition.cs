namespace Brimborium.Automata;

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

    internal void OnExecute() {
        throw new NotImplementedException();
    }
}