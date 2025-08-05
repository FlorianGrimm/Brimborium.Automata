namespace Brimborium.AutomataCode2;

public class FinishState<TMessage>(HierachicalName name, string? comment = default)
    : State<TMessage>(name, comment) {
    private ImmutableArray<StateTransaction<TMessage>> _Transitions = ImmutableArray<StateTransaction<TMessage>>.Empty;
    public ImmutableArray<StateTransaction<TMessage>> GetTransitions()
        => this._Transitions;

    public override RunningState<TMessage> CreateRunningState()
        => new RunningFinishState<TMessage>(this);

    public FinishState<TMessage> AddTransition(StateTransaction<TMessage> stateTransaction) {
        ArgumentNullException.ThrowIfNull(stateTransaction);
        this._Transitions = this._Transitions.Add(stateTransaction);
        return this;
    }
}

public class RunningFinishState<TMessage>(FinishState<TMessage> state)
    : RunningState<TMessage>() {
    private readonly FinishState<TMessage> _State = state;
    public override State<TMessage> State => this._State;

    public override void HandleMessage(
        long tick,
        StateMessagePathStep<TMessage> previousStep,
        TMessage? message,
        NextStateMaschine<TMessage> nextStateMaschine) {
        if (message is null) {
            return; // No message to process
        } else {
            var listStateTransaction = this._State.GetTransitions();
            foreach (var stateTransaction in listStateTransaction) {
                if (stateTransaction.Condition.DoesMatch(message)) {
                    var pathStep = stateTransaction.CreatePathStep(tick, previousStep, message);
                    nextStateMaschine.AddPathStep(pathStep);
                }
            }
        }
    }
}


public static partial class StateMaschineExtension {
    public static FinishState<TMessage> CreateFinishState<TMessage>(this StateMaschine<TMessage> stateMaschine, HierachicalName name, string? comment = default) {
        return new FinishState<TMessage>(name, comment);
    }
}
