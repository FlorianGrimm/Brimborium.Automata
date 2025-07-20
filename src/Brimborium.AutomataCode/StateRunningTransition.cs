namespace Brimborium.AutomataCode;

/// <summary>
/// Represents a state transition record containing the previous state, next state, and transition kind.
/// </summary>
/// <typeparam name="TStateMessage">The type of messages that trigger state transitions.</typeparam>
/// <param name="Previous">The previous running state.</param>
/// <param name="Next">The next running state, or null for terminate transitions.</param>
/// <param name="Kind">The kind of transition being performed.</param>
public record struct StateRunningTransition<TStateMessage>(IStateRunning<TStateMessage>? Previous, IStateRunning<TStateMessage>? Next, StateRunningTransitionKind Kind);
