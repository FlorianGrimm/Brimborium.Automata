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

    internal void OnExit() {
        throw new NotImplementedException();
    }
}
