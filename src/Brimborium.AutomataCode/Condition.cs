namespace Brimborium.AutomataCode;

/// <summary>
/// Abstract base class for conditions that determine whether a state message matches specific criteria.
/// </summary>
/// <typeparam name="TStateMessage">The type of state message to evaluate.</typeparam>
public abstract class Condition<TStateMessage> {
    /// <summary>
    /// Determines whether the specified message matches this condition.
    /// </summary>
    /// <param name="message">The state message to evaluate.</param>
    /// <returns>true if the message matches the condition; otherwise, false.</returns>
    public abstract bool DoesMatch(TStateMessage message);
}

/// <summary>
/// A condition that always returns false, regardless of the input message.
/// This class implements the singleton pattern for efficiency.
/// </summary>
/// <typeparam name="TStateMessage">The type of state message to evaluate.</typeparam>
public sealed class FalseCondition<TStateMessage> : Condition<TStateMessage> {
    private static FalseCondition<TStateMessage>? _GetInstance;

    /// <summary>
    /// Gets the singleton instance of the FalseCondition for the specified message type.
    /// </summary>
    /// <returns>The singleton instance of FalseCondition.</returns>
    public static FalseCondition<TStateMessage> GetInstance()
        => (_GetInstance ??= new FalseCondition<TStateMessage>());

    /// <summary>
    /// Always returns false, regardless of the input message.
    /// </summary>
    /// <param name="message">The state message to evaluate (ignored).</param>
    /// <returns>Always returns false.</returns>
    public override bool DoesMatch(TStateMessage message) => false;
}

/// <summary>
/// A condition that uses a delegate function to determine whether a state message matches.
/// </summary>
/// <typeparam name="TStateMessage">The type of state message to evaluate.</typeparam>
public sealed class DelegateCondition<TStateMessage> : Condition<TStateMessage> {
    private readonly Func<TStateMessage, bool> _FuncMatch;

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateCondition{TStateMessage}"/> class.
    /// </summary>
    /// <param name="funcMatch">The function that determines whether a message matches the condition.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="funcMatch"/> is null.</exception>
    public DelegateCondition(Func<TStateMessage, bool> funcMatch) {
        this._FuncMatch = funcMatch ?? throw new ArgumentNullException(nameof(funcMatch));
    }

    /// <summary>
    /// Determines whether the specified message matches this condition by calling the delegate function.
    /// </summary>
    /// <param name="message">The state message to evaluate.</param>
    /// <returns>The result of calling the delegate function with the specified message.</returns>
    public override bool DoesMatch(TStateMessage message) => this._FuncMatch(message);
}

/// <summary>
/// Extension methods for creating conditions in state machine builders.
/// </summary>
public static partial class StateMaschineExtensions {
    /// <summary>
    /// Creates a FalseCondition instance that always returns false.
    /// Note: The funcMatch parameter is ignored as this method always returns a FalseCondition.
    /// </summary>
    /// <typeparam name="TStateMessage">The type of state message to evaluate.</typeparam>
    /// <param name="builder">The state definition builder (used for extension method syntax).</param>
    /// <param name="funcMatch">This parameter is ignored for FalseCondition.</param>
    /// <returns>A singleton instance of FalseCondition that always returns false.</returns>
    public static FalseCondition<TStateMessage> CreateFalseCondition<TStateMessage>(
            this IStateDefinitionBuilder<TStateMessage> builder,
            Func<TStateMessage, bool> funcMatch) {
        return FalseCondition<TStateMessage>.GetInstance();
    }

    /// <summary>
    /// Creates a DelegateCondition that uses the specified function to evaluate state messages.
    /// </summary>
    /// <typeparam name="TStateMessage">The type of state message to evaluate.</typeparam>
    /// <param name="builder">The state definition builder (used for extension method syntax).</param>
    /// <param name="funcMatch">The function that determines whether a message matches the condition.</param>
    /// <returns>A new DelegateCondition instance that uses the specified function.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="funcMatch"/> is null.</exception>
    public static DelegateCondition<TStateMessage> CreateDelegateCondition<TStateMessage>(
            this IStateDefinitionBuilder<TStateMessage> builder,
            Func<TStateMessage, bool> funcMatch) {
        return new DelegateCondition<TStateMessage>(funcMatch);
    }
}