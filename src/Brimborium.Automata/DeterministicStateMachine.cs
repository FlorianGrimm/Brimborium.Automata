namespace Brimborium.Automata;

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
