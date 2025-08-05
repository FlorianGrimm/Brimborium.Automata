namespace Brimborium.AutomataCode2;

public class MatchOneState<TMessage>(HierachicalName name, string? comment = default)
    : SimpleState<TMessage>(name, comment) {
    public override RunningState<TMessage> CreateRunningState()
        => new RunningMatchOneState<TMessage>(this);
}

public class RunningMatchOneState<TMessage>(MatchOneState<TMessage> state)
    : RunningState<TMessage>() {
    private readonly MatchOneState<TMessage> _State = state;
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
    public static MatchOneState<TMessage> CreateMatchOneState<TMessage>(this StateMaschine<TMessage> stateMaschine, HierachicalName name, string? comment = default) {
        return new MatchOneState<TMessage>(name, comment);
    }
}
