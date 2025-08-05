namespace Brimborium.AutomataCode2;

public abstract class State<TMessage> {
    private readonly HierachicalName _Name;
    private readonly string? _Comment;

    public State(HierachicalName name, string? comment = default) {
        this._Name = name;
        this._Comment = comment;
    }

    public HierachicalName Name => this._Name;

    public string? Comment => this._Comment;

    //public abstract ImmutableArray<StateTransaction<TMessage>> GetTransitions();

    public abstract RunningState<TMessage> CreateRunningState();
}

public abstract class RunningState<TMessage> {
    public abstract State<TMessage> State { get; }

    public abstract void HandleMessage(
        long tick,
        StateMessagePathStep<TMessage> previousStep,
        TMessage? message,
        NextStateMaschine<TMessage> nextStateMaschine);
    public virtual void OnEnter(StateMessagePathStep<TMessage> nextStep) { }
}

public static class StateExtension {
    public static TState AddState<TState, TMessage>(TState state)
         where TState : State<TMessage> {
        
         return state;
    }

}