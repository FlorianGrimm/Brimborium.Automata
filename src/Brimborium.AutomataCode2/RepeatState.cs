namespace Brimborium.AutomataCode2;

public class RepeatState<TMessage>(HierachicalName name, string? comment = default)
    : State<TMessage>(name, comment) {
    //private ImmutableArray<StateTransaction<TMessage>>? _AllTransitions = null;

    //public override ImmutableArray<StateTransaction<TMessage>> GetTransitions() {
    //    if (this._AllTransitions is null) {
    //        this._AllTransitions = ImmutableArray<StateTransaction<TMessage>>.Empty
    //            .AddRange(this._RepeatTransitions)
    //            .AddRange(this._ExitTransitions);
    //    }
    //    return this._AllTransitions.Value;
    //}

    private ImmutableArray<StateTransaction<TMessage>> _RepeatTransitions = ImmutableArray<StateTransaction<TMessage>>.Empty;
    public ImmutableArray<StateTransaction<TMessage>> GetRepeatTransitions() => this._RepeatTransitions;

    public void AddRepeatTransition(StateTransaction<TMessage> stateTransaction) {
        this._RepeatTransitions = this._RepeatTransitions.Add(stateTransaction);
    }

    private ImmutableArray<StateTransaction<TMessage>> _ExitTransitions = ImmutableArray<StateTransaction<TMessage>>.Empty;


    public void AddExitTransition(StateTransaction<TMessage> stateTransaction) {
        this._ExitTransitions = this._ExitTransitions.Add(stateTransaction);
    }

    public ImmutableArray<StateTransaction<TMessage>> GetExitTransitions() => this._ExitTransitions;

    public override RunningState<TMessage> CreateRunningState()
        => new RunningRepeatState<TMessage>(this);
}

public class RunningRepeatState<TMessage>(RepeatState<TMessage> state)
    : RunningState<TMessage>() {
    private readonly RepeatState<TMessage> _State = state;
    public override State<TMessage> State => this._State;
    public override void HandleMessage(
        long tick,
        StateMessagePathStep<TMessage> previousStep,
        TMessage? message,
        NextStateMaschine<TMessage> nextStateMaschine) {
        if (message is null) {
            return; // No message to process
        } else {
            var listExitTransitions = this._State.GetExitTransitions();
            foreach (var stateTransaction in listExitTransitions) {
                if (stateTransaction.Condition.DoesMatch(message)) {
                    var pathStep = stateTransaction.CreatePathStep(tick, previousStep, message);
                    nextStateMaschine.AddPathStep(pathStep);
                }
            }
            var listStateTransaction = this._State.GetRepeatTransitions();
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
    public static RepeatState<TMessage> CreateRepeatState<TMessage>(this StateMaschine<TMessage> stateMaschine, HierachicalName name, string? comment = default) {
        return new RepeatState<TMessage>(name, comment);
    }
}
