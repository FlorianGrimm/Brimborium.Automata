namespace Brimborium.AutomataCode;

/// <summary>
/// Interface for builders that can add state definitions to a state machine.
/// </summary>
/// <typeparam name="TStateMessage">The type of messages that trigger state transitions.</typeparam>
public interface IStateDefinitionBuilder<TStateMessage> {
    /// <summary>
    /// Adds a state definition to the builder.
    /// </summary>
    /// <param name="value">The state definition to add.</param>
    void AddStateDefinition(IStateDefinition<TStateMessage> value);
}

