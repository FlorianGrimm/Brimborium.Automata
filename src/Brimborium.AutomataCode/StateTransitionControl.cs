namespace Brimborium.AutomataCode;

/// <summary>
/// Manages state transitions during message processing, collecting and executing transition decisions.
/// </summary>
/// <typeparam name="TStateMessage">The type of messages that trigger state transitions.</typeparam>
public class StateTransitionControl<TStateMessage> {
    private readonly StateMaschine<TStateMessage> _StateMaschine;
    private readonly List<StateRunningTransition<TStateMessage>> _ListStateRunningTransition = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="StateTransitionControl{TStateMessage}"/> class.
    /// </summary>
    /// <param name="stateMaschine">The state machine that owns this transition context.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="stateMaschine"/> is null.</exception>
    public StateTransitionControl(StateMaschine<TStateMessage> stateMaschine) {
        this._StateMaschine = stateMaschine ?? throw new ArgumentNullException(nameof(stateMaschine));
    }

    /// <summary>
    /// Adds a start transition from a previous state to a new state.
    /// </summary>
    /// <param name="previous">The previous state.</param>
    /// <param name="next">The next state to transition to.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="previous"/> or <paramref name="next"/> is null.</exception>
    public void AddStart(IStateRunning<TStateMessage> previous, IStateRunning<TStateMessage> next) {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(next);
        this._ListStateRunningTransition.Add(new StateRunningTransition<TStateMessage>(previous, next, StateRunningTransitionKind.Start));
    }

    /// <summary>
    /// Adds a stay transition, indicating the state should remain in the current state.
    /// </summary>
    /// <param name="previous">The current state that will remain active.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="previous"/> is null.</exception>
    public void AddStay(IStateRunning<TStateMessage> previous) {
        ArgumentNullException.ThrowIfNull(previous);
        this._ListStateRunningTransition.Add(new StateRunningTransition<TStateMessage>(previous, previous, StateRunningTransitionKind.Stay));
    }

    /// <summary>
    /// Adds a next transition from a previous state to a new state.
    /// </summary>
    /// <param name="previous">The previous state.</param>
    /// <param name="next">The next state to transition to.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="previous"/> or <paramref name="next"/> is null.</exception>
    public void AddNext(IStateRunning<TStateMessage> previous, IStateRunning<TStateMessage> next) {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(next);
        this._ListStateRunningTransition.Add(new StateRunningTransition<TStateMessage>(previous, next, StateRunningTransitionKind.Next));
    }

    /// <summary>
    /// Adds a transition that either moves to the next state or stays in the current state, depending on whether the next definition is the same as the current.
    /// </summary>
    /// <param name="previous">The previous state.</param>
    /// <param name="nextDefinition">The definition of the next state.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="previous"/> or <paramref name="nextDefinition"/> is null.</exception>
    public void AddNextOrStay(IStateRunning<TStateMessage> previous, IStateDefinition<TStateMessage> nextDefinition) {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(nextDefinition);
        if (ReferenceEquals(previous.Definition, nextDefinition)) {
            this._ListStateRunningTransition.Add(new StateRunningTransition<TStateMessage>(previous, previous, StateRunningTransitionKind.Stay));
        } else {
            this._ListStateRunningTransition.Add(new StateRunningTransition<TStateMessage>(previous, nextDefinition.CreateStateRunning(previous), StateRunningTransitionKind.Next));
        }
    }

    /// <summary>
    /// Adds a transition that either moves to the next state, stays in the current state, or terminates, depending on the next definition.
    /// </summary>
    /// <param name="previous">The previous state.</param>
    /// <param name="nextDefinition">The definition of the next state, or null to terminate.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="previous"/> is null.</exception>
    public void AddNextOrStayOrTerminate(IStateRunning<TStateMessage> previous, IStateDefinition<TStateMessage>? nextDefinition) {
        ArgumentNullException.ThrowIfNull(previous);
        if (ReferenceEquals(previous.Definition, nextDefinition)) {
            this._ListStateRunningTransition.Add(new StateRunningTransition<TStateMessage>(previous, previous, StateRunningTransitionKind.Stay));
        } else if (nextDefinition is { }) {
            this._ListStateRunningTransition.Add(new StateRunningTransition<TStateMessage>(previous, nextDefinition.CreateStateRunning(previous), StateRunningTransitionKind.Next));
        } else {
            this._ListStateRunningTransition.Add(new StateRunningTransition<TStateMessage>(previous, default, StateRunningTransitionKind.Terminate));
        }
    }

    /// <summary>
    /// Adds a fork transition, creating a new parallel execution path.
    /// </summary>
    /// <param name="previous">The previous state.</param>
    /// <param name="next">The next state to fork to.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="previous"/> or <paramref name="next"/> is null.</exception>
    public void AddFork(IStateRunning<TStateMessage> previous, IStateRunning<TStateMessage> next) {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(next);
        this._ListStateRunningTransition.Add(new StateRunningTransition<TStateMessage>(previous, next, StateRunningTransitionKind.Fork));
    }

    /// <summary>
    /// Adds a terminate transition, indicating the state should be terminated.
    /// </summary>
    /// <param name="previous">The state to terminate.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="previous"/> is null.</exception>
    public void AddTerminate(IStateRunning<TStateMessage> previous) {
        ArgumentNullException.ThrowIfNull(previous);
        this._ListStateRunningTransition.Add(new StateRunningTransition<TStateMessage>(previous, default, StateRunningTransitionKind.Terminate));
    }

    /// <summary>
    /// Adds a return transition, indicating the state should return to a previous state or complete.
    /// </summary>
    /// <param name="previous">The state that is returning.</param>
    /// <param name="next">The next state to return to, or null if completing.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="previous"/> is null.</exception>
    public void AddReturn(IStateRunning<TStateMessage> previous, IStateRunning<TStateMessage>? next) {
        ArgumentNullException.ThrowIfNull(previous);
        this._ListStateRunningTransition.Add(new StateRunningTransition<TStateMessage>(previous, next, StateRunningTransitionKind.Return));
    }

    /// <summary>
    /// Adds a custom state running transition to the transition list.
    /// </summary>
    /// <param name="stateRunningTransition">The state running transition to add.</param>
    public void Add(StateRunningTransition<TStateMessage> stateRunningTransition) {
        this._ListStateRunningTransition.Add(stateRunningTransition);
    }

    /// <summary>
    /// Processes all collected state transitions and returns the resulting state configuration.
    /// </summary>
    /// <param name="listCurrentState">The current list of running states.</param>
    /// <param name="verifiy">Whether to verify that all current states have corresponding transitions.</param>
    /// <returns>A tuple containing the next list of running states and any states that returned during processing.</returns>
    /// <exception cref="Exception">Thrown when verification fails or transition logic is inconsistent.</exception>
    public HandleTransactionsResult<TStateMessage> HandleTransactions(
        ImmutableArray<IStateRunning<TStateMessage>> listCurrentState,
        bool verifiy
        ) {
        var listStateRunningTransition = this._ListStateRunningTransition;
        if (verifiy) {
            if (listStateRunningTransition.Count < listCurrentState.Length) {
                throw new Exception("Not all transition calls Addxxx");
            }
            int idxCurrentState = 0;
            int idxStateRunningTransition = 0;
            while ((idxCurrentState < listCurrentState.Length)
                && (idxStateRunningTransition < listStateRunningTransition.Count)) {
                var currentState = listCurrentState[idxCurrentState];
                var stateRunningTransition = listStateRunningTransition[idxStateRunningTransition];
                if (ReferenceEquals(currentState, stateRunningTransition.Previous)) {
                    idxCurrentState++;
                    idxStateRunningTransition++;
                    while ((idxStateRunningTransition < listStateRunningTransition.Count)
                        && ReferenceEquals(currentState, listStateRunningTransition[idxStateRunningTransition].Previous)
                        ) {
                        idxStateRunningTransition++;
                    }
                } else {
                    throw new Exception($"{currentState} has no StateRunningTransition");
                }
            }
            if (idxCurrentState < listCurrentState.Length) {
                throw new Exception($"not all currentState has a StateRunningTransition");
            }
            if (idxStateRunningTransition < listStateRunningTransition.Count) {
                throw new Exception($"unexpected more StateRunningTransition");
            }
        }
        //
        List<IStateRunning<TStateMessage>>? listReturn = null;
        List<IStateRunning<TStateMessage>> listEnter = [];
        List<IStateRunning<TStateMessage>> result = new(listStateRunningTransition.Count);
        foreach (var stateRunningTransition in listStateRunningTransition) {
            if (stateRunningTransition.Kind == StateRunningTransitionKind.Start) {
                if (stateRunningTransition.Next is { } next) {
                    Add(result, stateRunningTransition.Next, listEnter, stateRunningTransition.Previous);
                } else {
                    throw new Exception("start but next is null");
                }
            } else if (stateRunningTransition.Kind == StateRunningTransitionKind.Stay) {
                if (stateRunningTransition.Previous is { } previous) {
                    if (stateRunningTransition.Next is { } next) {
                        if (!ReferenceEquals(previous, next)) {
                            throw new Exception("stay but previous != next");
                        }
                    }
                    Add(result, previous, null, null);
                }
            } else if (stateRunningTransition.Kind == StateRunningTransitionKind.Next) {
                if (stateRunningTransition.Next is { } next) {
                    Add(result, stateRunningTransition.Next, listEnter, stateRunningTransition.Previous);
                } else {
                    throw new Exception("next but next is null");
                }
            } else if (stateRunningTransition.Kind == StateRunningTransitionKind.Fork) {
                if (stateRunningTransition.Next is { } next) {
                    Add(result, stateRunningTransition.Next, listEnter, stateRunningTransition.Previous);
                } else {
                    throw new Exception("fork but next is null");
                }
            } else if (stateRunningTransition.Kind == StateRunningTransitionKind.Terminate) {
                // skip
            } else if (stateRunningTransition.Kind == StateRunningTransitionKind.Return) {
                if (stateRunningTransition.Previous is { } previous) {
                    (listReturn ??= new()).Add(previous);
                } else {
                    throw new Exception("return but Previous is null");
                }
            }
        }
        //        
        return new (ListNextStateRunning: result, ListEnter:listEnter, ListReturn: listReturn);

        static void Add(
            List<IStateRunning<TStateMessage>> result, IStateRunning<TStateMessage>? value,
            List<IStateRunning<TStateMessage>> listEnter, IStateRunning<TStateMessage>? previous
            ) {
            if (value is { }) {
                if (0 < result.Count && result.Contains(value)) {
                    return;
                } else {
                    result.Add(value);
                    if (listEnter is { } && !ReferenceEquals(result, previous)) {
                        listEnter.Add(value);
                    }
                }
            }
        }
    }
}

public record HandleTransactionsResult<TStateMessage>(
        List<IStateRunning<TStateMessage>> ListNextStateRunning,
        List<IStateRunning<TStateMessage>>? ListEnter,
        List<IStateRunning<TStateMessage>>? ListReturn
        );