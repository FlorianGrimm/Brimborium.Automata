
namespace Brimborium.AutomataCode2;

public class StateTransaction<TMessage> {
    private readonly ICondition<TMessage> _Condition;
    private readonly State<TMessage> _NextState;

    public StateTransaction(
        ICondition<TMessage> condition,
        State<TMessage> nextState
        ) {
        this._Condition = condition;
        this._NextState = nextState;
    }

    public ICondition<TMessage> Condition => this._Condition;

    public State<TMessage> NextState => this._NextState;

    public StateMessagePathStep<TMessage> CreatePathStep(
        long tick,
        StateMessagePathStep<TMessage>? previousStep,
        TMessage? message) {
        var runningState = this._NextState.CreateRunningState();
        var index = (previousStep is { }) ? (previousStep.Index + 1) : 0;
        var result = new StateMessagePathStep<TMessage>(
            Previous: previousStep,
            Index: index,
            Tick: tick,
            Message: message,
            State: runningState);
        return result;
    }
}

