namespace Brimborium.AutomataCode;

public sealed class ReturnDefinition<TStateMessage> : IStateDefinition<TStateMessage> {
    private readonly HierachicalName _Name;

    public ReturnDefinition(HierachicalName name) {
        this._Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public HierachicalName Name => this._Name;

    public IStateRunning<TStateMessage> CreateStateRunning(IStateRunning<TStateMessage>? previous)
        => new ReturnStateRunning(this, previous);

    public sealed class ReturnStateRunning : IStateRunning<TStateMessage>, IStateRunningEnter<TStateMessage> {
        private readonly ReturnDefinition<TStateMessage> _Definition;
        private readonly IStateRunning<TStateMessage>? _Previous;

        public ReturnStateRunning(ReturnDefinition<TStateMessage> definition, IStateRunning<TStateMessage>? previous) {
            this._Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            this._Previous = previous;
        }
        public IStateRunning<TStateMessage>? Previous => this._Previous;

        public IStateDefinition<TStateMessage> Definition => this._Definition;

        public void Enter(HandleTransactionsResult<TStateMessage> handleTransactionsResult) {
            if (handleTransactionsResult.ListReturn is { } listReturn) {
            } else { 
                listReturn = new();
                handleTransactionsResult = handleTransactionsResult with { ListReturn = listReturn };
            }
            listReturn.Add(this);
            handleTransactionsResult.ListReturn.Remove(this);
        }

        public void HandleIncoming(TStateMessage message, StateTransitionControl<TStateMessage> stateTransition) {
            if (this._Previous is { } previous) {
                stateTransition.AddTerminate(this);
            }
        }
    }
}

/// <summary>
/// A builder for configuring ReturnDefinition instances with fluent syntax.
/// </summary>
/// <typeparam name="TStateMessage">The type of messages that trigger state transitions.</typeparam>
public class ReturnDefinitionBuilder<TStateMessage> {
    private readonly ReturnDefinition<TStateMessage> _StateDefinition;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReturnDefinitionBuilder{TStateMessage}"/> class.
    /// </summary>
    /// <param name="stateDefinition">The state definition to configure.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="stateDefinition"/> is null.</exception>
    public ReturnDefinitionBuilder(
        ReturnDefinition<TStateMessage> stateDefinition
        ) {
        this._StateDefinition = stateDefinition ?? throw new ArgumentNullException(nameof(stateDefinition));
    }

#if false
    /// <summary>
    /// Gets a builder for configuring the true case state definition.
    /// </summary>
    public IStateDefinitionBuilder<TStateMessage> TrueCase
        => new PropertyStateDefinitionBuilder<TStateMessage, ReturnDefinition<TStateMessage>>(
            this._StateDefinition,
            static (that, value) => that.TrueCase = value);
#endif
   
}

/// <summary>
/// Extension methods for state machine functionality.
/// </summary>
public static partial class StateMaschineExtensions {
    /// <summary>
    /// Adds a ReturnDefinition to the state definition builder with optional configuration.
    /// </summary>
    /// <typeparam name="TStateMessage">The type of messages that trigger state transitions.</typeparam>
    /// <param name="builder">The state definition builder to add the state to.</param>
    /// <param name="name">The hierarchical name of the state.</param>
    /// <returns>A builder for further configuration of the ReturnDefinition.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> or <paramref name="name"/> is null.</exception>
    public static ReturnDefinitionBuilder<TStateMessage> AddReturn<TStateMessage>(
            this IStateDefinitionBuilder<TStateMessage> builder,
            HierachicalName name
        ) {
        var result = new ReturnDefinition<TStateMessage>(name);
        if (builder is IStateDefinitionBuilder<TStateMessage> stateMaschine) {
            stateMaschine.AddStateDefinition(result);
        }
        return new(result);
    }
}