namespace Brimborium.AutomataCode2;

public class StartState<TMessage>(HierachicalName name, string? comment = default)
    : SimpleState<TMessage>(name, comment) {
    public StateTransactionBuilder<TMessage> Builder()
        => new StateTransactionBuilder<TMessage>(this.AddTransition);
}

public class RunningStartState<TMessage>(StartState<TMessage> state)
    : RunningState<TMessage>() {
    private readonly StartState<TMessage> _State = state;
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
