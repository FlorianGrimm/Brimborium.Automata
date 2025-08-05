namespace Brimborium.AutomataCode2;

public class StateMaschine<TMessage> {
    private readonly StartState<TMessage> _StartState;

    public StateMaschine() {
        this._StartState = new Brimborium.AutomataCode2.StartState<TMessage>(new("Start"), "Start State");
    }

    public StartState<TMessage> StartState => this._StartState;

    public RunningStateMaschine<TMessage> Start()
        => RunningStateMaschine<TMessage>.Start(this);
}

public struct RunningStateMaschine<TMessage> {
    public static RunningStateMaschine<TMessage> Start(StateMaschine<TMessage> stateMaschine) {
        List<StateMessagePathStep<TMessage>> currentState = new();
        currentState.Add(new StateMessagePathStep<TMessage>(
            Previous: default,
            Index: 0,
            Tick: 0,
            Message: default,
            State: stateMaschine.StartState.CreateRunningState()));
        return new RunningStateMaschine<TMessage>(1, currentState);
    }

    private readonly long _Tick;
    private readonly List<StateMessagePathStep<TMessage>> _CurrentState;

    public RunningStateMaschine(
        long tick,
        List<StateMessagePathStep<TMessage>> currentState) {
        this._CurrentState = currentState;
        this._Tick = tick;
    }

    public RunningStateMaschine<TMessage> HandleMessage(TMessage message) {
        List<StateMessagePathStep<TMessage>> listCurrentState = this._CurrentState;

        List<StateMessagePathStep<TMessage>> listNextState = new(listCurrentState.Count);
        NextStateMaschine<TMessage> nextStateMaschine = new(listNextState);
        foreach (var currentStep in listCurrentState) {
            currentStep.State.HandleMessage(
                this._Tick, currentStep,
                message, nextStateMaschine);
        }
        foreach(var nextStep in listNextState) {
            if (nextStep.Tick == this._Tick) {
                nextStep.State.OnEnter(nextStep);
            }
        }
        var result = new RunningStateMaschine<TMessage>(this._Tick + 1, listNextState);
        return result;
    }
}
public class NextStateMaschine<TMessage> {
    private readonly List<StateMessagePathStep<TMessage>> _ListNextState;

    public NextStateMaschine(List<StateMessagePathStep<TMessage>> listNextState) {
        this._ListNextState = listNextState;
    }

    public void AddPathStep(StateMessagePathStep<TMessage> stateMessagePathStep) {
        this._ListNextState.Add(stateMessagePathStep);
    }
}

public record class StateMessagePathStep<TMessage>(
    StateMessagePathStep<TMessage>? Previous,
    int Index,
    long Tick,
    TMessage? Message,
    RunningState<TMessage> State);
