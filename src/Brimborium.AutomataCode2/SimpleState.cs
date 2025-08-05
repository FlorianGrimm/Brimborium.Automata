namespace Brimborium.AutomataCode2;

public class SimpleState<TMessage>(HierachicalName name, string? comment = default)
    : State<TMessage>(name, comment) {

    private ImmutableArray<StateTransaction<TMessage>> _Transitions = ImmutableArray<StateTransaction<TMessage>>.Empty;

    public ImmutableArray<StateTransaction<TMessage>> GetTransitions() => this._Transitions;

    public void AddTransition(StateTransaction<TMessage> stateTransaction) {
        this._Transitions = this._Transitions.Add(stateTransaction);
    }

    public override RunningState<TMessage> CreateRunningState()
        => new RunningSimpleState<TMessage>(this);
}

public class RunningSimpleState<TMessage>(SimpleState<TMessage> state)
    : RunningState<TMessage>() {
    private readonly SimpleState<TMessage> _State = state;
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


public struct StateTransactionBuilder<TMessage>(
    Action<StateTransaction<TMessage>> addTransition
    ) {
    private Action<StateTransaction<TMessage>> _AddTransition = addTransition;

    public StateTransactionWithConditionBuilder<TMessage> WithCondition(ICondition<TMessage> condition)
        => new StateTransactionWithConditionBuilder<TMessage>(this._AddTransition, condition);
}

public struct StateTransactionWithConditionBuilder<TMessage>(
    Action<StateTransaction<TMessage>> addTransition,
    ICondition<TMessage> condition) {
    private readonly Action<StateTransaction<TMessage>> _AddTransition = addTransition;
    private readonly ICondition<TMessage> _Condition = condition;

    public void ToState(State<TMessage> nextState) {
        this._AddTransition(
            new StateTransaction<TMessage>(
                this._Condition, nextState));
    }
}