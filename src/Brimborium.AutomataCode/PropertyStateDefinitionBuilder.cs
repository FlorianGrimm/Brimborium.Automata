namespace Brimborium.AutomataCode;

/// <summary>
/// A property-based state definition builder that sets a property on an owner object when a state definition is added.
/// </summary>
/// <typeparam name="TStateMessage">The type of messages that trigger state transitions.</typeparam>
/// <typeparam name="TOwner">The type of the owner object that contains the property to set.</typeparam>
public readonly struct PropertyStateDefinitionBuilder<TStateMessage, TOwner> : IStateDefinitionBuilder<TStateMessage> {
    private readonly TOwner _Owner;
    private readonly Action<TOwner, IStateDefinition<TStateMessage>> _Action;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyStateDefinitionBuilder{TStateMessage, TOwner}"/> struct.
    /// </summary>
    /// <param name="owner">The owner object that contains the property to set.</param>
    /// <param name="action">The action that sets the property on the owner object.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="owner"/> or <paramref name="action"/> is null.</exception>
    public PropertyStateDefinitionBuilder(TOwner owner, Action<TOwner, IStateDefinition<TStateMessage>> action) {
        this._Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        this._Action = action ?? throw new ArgumentNullException(nameof(action));
    }

    /// <summary>
    /// Adds a state definition by setting it on the owner object using the configured action.
    /// </summary>
    /// <param name="value">The state definition to add.</param>
    public readonly void AddStateDefinition(IStateDefinition<TStateMessage> value) {
        this._Action(this._Owner, value);
    }
}
