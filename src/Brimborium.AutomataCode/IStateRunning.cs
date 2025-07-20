namespace Brimborium.AutomataCode;

/// <summary>
/// Interface for running state instances that can process incoming messages.
/// </summary>
/// <typeparam name="TStateMessage">The type of messages that trigger state transitions.</typeparam>
public interface IStateRunning<TStateMessage> {
    /// <summary>
    /// Gets the previous running state that led to this state, or null if this is an initial state.
    /// </summary>
    IStateRunning<TStateMessage>? Previous { get; }

    /// <summary>
    /// Gets the state definition that created this running state.
    /// </summary>
    IStateDefinition<TStateMessage> Definition { get; }

    /// <summary>
    /// Handles an incoming message and determines the appropriate state transitions.
    /// </summary>
    /// <param name="message">The incoming message to process.</param>
    /// <param name="stateTransition">The state transition context for recording transition decisions.</param>
    void HandleIncoming(TStateMessage message, StateTransitionControl<TStateMessage> stateTransition);
}


public interface IStateRunningEnter<TStateMessage> {
    void Enter(HandleTransactionsResult<TStateMessage> handleTransactionsResult);
}