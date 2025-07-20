namespace Brimborium.AutomataCode;

/// <summary>
/// Interface for state definitions that can create running state instances.
/// </summary>
/// <typeparam name="TStateMessage">The type of messages that trigger state transitions.</typeparam>
public interface IStateDefinition<TStateMessage> {
    /// <summary>
    /// Gets the hierarchical name of this state definition.
    /// </summary>
    HierachicalName Name { get; }

    /// <summary>
    /// Creates a running instance of this state definition.
    /// </summary>
    /// <param name="previous">The previous running state, or null if this is an initial state.</param>
    /// <returns>A new running state instance.</returns>
    public abstract IStateRunning<TStateMessage> CreateStateRunning(IStateRunning<TStateMessage>? previous);
}

