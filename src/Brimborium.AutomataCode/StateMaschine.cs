using System.Collections.Immutable;
using System.Runtime.CompilerServices;


namespace Brimborium.AutomataCode;

/// <summary>
/// Represents a state machine that processes state messages and manages state transitions.
/// This class supports multiple concurrent states and complex state transition logic.
/// </summary>
/// <typeparam name="TStateMessage">The type of messages that trigger state transitions.</typeparam>
public class StateMaschine<TStateMessage> : IStateDefinitionBuilder<TStateMessage> {
    private ImmutableArray<IStateDefinition<TStateMessage>> _ListInitialState = ImmutableArray<IStateDefinition<TStateMessage>>.Empty;
    private ImmutableArray<IStateRunning<TStateMessage>> _ListCurrentState = ImmutableArray<IStateRunning<TStateMessage>>.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="StateMaschine{TStateMessage}"/> class.
    /// </summary>
    public StateMaschine() {
    }

    /// <summary>
    /// Adds a state definition to the list of initial states for this state machine.
    /// </summary>
    /// <param name="value">The state definition to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public void AddStateDefinition(IStateDefinition<TStateMessage> value) {
        ArgumentNullException.ThrowIfNull(value);
        this._ListInitialState = this._ListInitialState.Add(value);
    }

    /// <summary>
    /// Starts the state machine by creating running instances of all initial state definitions.
    /// This method must be called before processing any messages.
    /// </summary>
    public void Start() {
        List<IStateRunning<TStateMessage>> list = new();
        foreach (var initialState in this._ListInitialState) {
            IStateRunning<TStateMessage> stateRunning = initialState.CreateStateRunning(default);
            list.Add(stateRunning);
        }
        this._ListCurrentState = list.ToImmutableArray();
    }

    /// <summary>
    /// Handles an incoming state message by processing it through all current states and executing any resulting transitions.
    /// </summary>
    /// <param name="message">The state message to process.</param>
    /// <returns>A list of states that returned during processing, or null if no states returned.</returns>
    public List<IStateRunning<TStateMessage>>? HandleIncoming(TStateMessage message) {
        var listCurrentState = this.GetListCurrentState();
        var stateTransition = new StateTransitionControl<TStateMessage>(this);
        foreach (var currentState in listCurrentState) {
            currentState.HandleIncoming(message, stateTransition);
        }
        var handleTransactionsResult = stateTransition.HandleTransactions(listCurrentState, true);
        if (handleTransactionsResult.ListEnter is { } listEnter) {
            foreach (var nextState in listEnter) {
                if (nextState is IStateRunningEnter<TStateMessage> stateRunningEnter) {
                    stateRunningEnter.Enter(handleTransactionsResult);
                }
            }
        }
        this._ListCurrentState = handleTransactionsResult.ListNextStateRunning.ToImmutableArray();
        return handleTransactionsResult.ListReturn;
    }

    /// <summary>
    /// Gets the current list of running states in the state machine.
    /// </summary>
    /// <returns>An immutable array of currently running states.</returns>
    public ImmutableArray<IStateRunning<TStateMessage>> GetListCurrentState() => this._ListCurrentState;
}

