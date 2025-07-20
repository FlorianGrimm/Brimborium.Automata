namespace Brimborium.AutomataCode;
/// <summary>
/// A state definition that matches a single condition and transitions based on the result.
/// This state evaluates a condition against incoming messages and transitions to either the true case or false case state.
/// </summary>
/// <typeparam name="TStateMessage">The type of messages that trigger state transitions.</typeparam>
public sealed class MatchOneStateDefinition<TStateMessage> : IStateDefinition<TStateMessage> {
    private readonly HierachicalName _Name;

    /// <summary>
    /// Initializes a new instance of the <see cref="MatchOneStateDefinition{TStateMessage}"/> class.
    /// </summary>
    /// <param name="name">The hierarchical name of this state definition.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null.</exception>
    public MatchOneStateDefinition(HierachicalName name) {
        this._Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    /// <summary>
    /// Gets the hierarchical name of this state definition.
    /// </summary>
    public HierachicalName Name => this._Name;

    /// <summary>
    /// Gets or sets the condition that determines which case to transition to.
    /// Defaults to a FalseCondition that always returns false.
    /// </summary>
    public Condition<TStateMessage> Condition { get; set; } = FalseCondition<TStateMessage>.GetInstance();

    /// <summary>
    /// Gets or sets the state definition to transition to when the condition evaluates to true.
    /// </summary>
    public IStateDefinition<TStateMessage>? TrueCase { get; set; }

    /// <summary>
    /// Gets or sets the state definition to transition to when the condition evaluates to false.
    /// </summary>
    public IStateDefinition<TStateMessage>? FalseCase { get; set; }

    /// <summary>
    /// Creates a running instance of this state definition.
    /// </summary>
    /// <param name="previous">The previous running state, or null if this is an initial state.</param>
    /// <returns>A new MatchOneStateRunning instance.</returns>
    public IStateRunning<TStateMessage> CreateStateRunning(IStateRunning<TStateMessage>? previous)
        => new MatchOneStateRunning(this, previous);

    /// <summary>
    /// A running instance of a MatchOneStateDefinition that processes incoming messages.
    /// </summary>
    public sealed class MatchOneStateRunning : IStateRunning<TStateMessage> {
        private readonly MatchOneStateDefinition<TStateMessage> _Definition;
        private readonly IStateRunning<TStateMessage>? _Previous;

        /// <summary>
        /// Initializes a new instance of the <see cref="MatchOneStateRunning"/> class.
        /// </summary>
        /// <param name="definition">The state definition that created this running instance.</param>
        /// <param name="previous">The previous running state, or null if this is an initial state.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="definition"/> is null.</exception>
        public MatchOneStateRunning(MatchOneStateDefinition<TStateMessage> definition, IStateRunning<TStateMessage>? previous) {
            this._Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            this._Previous = previous;
        }

        /// <summary>
        /// Gets the previous running state that led to this state, or null if this is an initial state.
        /// </summary>
        IStateRunning<TStateMessage>? IStateRunning<TStateMessage>.Previous => this._Previous;

        /// <summary>
        /// Gets the state definition that created this running state.
        /// </summary>
        IStateDefinition<TStateMessage> IStateRunning<TStateMessage>.Definition => this._Definition;

        /// <summary>
        /// Handles an incoming message by evaluating the condition and determining the appropriate transition.
        /// </summary>
        /// <param name="message">The incoming message to process.</param>
        /// <param name="stateTransition">The state transition context for recording transition decisions.</param>
        public void HandleIncoming(TStateMessage message, StateTransitionControl<TStateMessage> stateTransition) {
            bool doesMatch = this._Definition.Condition.DoesMatch(message);
            var matchingCase = doesMatch ? this._Definition.TrueCase : this._Definition.FalseCase;
            stateTransition.AddNextOrStayOrTerminate(this, matchingCase);
        }
    }
}

/// <summary>
/// A builder for configuring MatchOneStateDefinition instances with fluent syntax.
/// </summary>
/// <typeparam name="TStateMessage">The type of messages that trigger state transitions.</typeparam>
public class MatchOneStateDefinitionBuilder<TStateMessage> {
    private readonly MatchOneStateDefinition<TStateMessage> _StateDefinition;

    /// <summary>
    /// Initializes a new instance of the <see cref="MatchOneStateDefinitionBuilder{TStateMessage}"/> class.
    /// </summary>
    /// <param name="stateDefinition">The state definition to configure.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="stateDefinition"/> is null.</exception>
    public MatchOneStateDefinitionBuilder(
        MatchOneStateDefinition<TStateMessage> stateDefinition
        ) {
        this._StateDefinition = stateDefinition ?? throw new ArgumentNullException(nameof(stateDefinition));
    }

    /// <summary>
    /// Gets a builder for configuring the true case state definition.
    /// </summary>
    public IStateDefinitionBuilder<TStateMessage> TrueCase
        => new PropertyStateDefinitionBuilder<TStateMessage, MatchOneStateDefinition<TStateMessage>>(
            this._StateDefinition,
            static (that, value) => that.TrueCase = value);

    /// <summary>
    /// Gets a builder for configuring the false case state definition.
    /// </summary>
    public IStateDefinitionBuilder<TStateMessage> FalseCase
        => new PropertyStateDefinitionBuilder<TStateMessage, MatchOneStateDefinition<TStateMessage>>(
            this._StateDefinition,
            static (that, value) => that.FalseCase = value);
}

/// <summary>
/// Extension methods for state machine functionality.
/// </summary>
public static partial class StateMaschineExtensions {
    /// <summary>
    /// Adds a MatchOneStateDefinition to the state definition builder with optional configuration.
    /// </summary>
    /// <typeparam name="TStateMessage">The type of messages that trigger state transitions.</typeparam>
    /// <param name="builder">The state definition builder to add the state to.</param>
    /// <param name="name">The hierarchical name of the state.</param>
    /// <param name="condition">The condition to evaluate for state transitions. If null, defaults to FalseCondition.</param>
    /// <param name="trueCase">The state definition to transition to when the condition is true.</param>
    /// <param name="falseCase">The state definition to transition to when the condition is false.</param>
    /// <returns>A builder for further configuration of the MatchOneStateDefinition.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> or <paramref name="name"/> is null.</exception>
    public static MatchOneStateDefinitionBuilder<TStateMessage> AddMatchOne<TStateMessage>(
            this IStateDefinitionBuilder<TStateMessage> builder,
            HierachicalName name,
            Condition<TStateMessage>? condition = default,
            IStateDefinition<TStateMessage>? trueCase = default,
            IStateDefinition<TStateMessage>? falseCase = default
        ) {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(name);

        var result = new MatchOneStateDefinition<TStateMessage>(name);
        if (condition is { }) { result.Condition = condition; }
        if (trueCase is { }) { result.TrueCase = trueCase; }
        if (falseCase is { }) { result.FalseCase = falseCase; }
        if (builder is IStateDefinitionBuilder<TStateMessage> stateMaschine) {
            stateMaschine.AddStateDefinition(result);
        }
        return new(result);
    }
}