namespace Brimborium.AutomataCode;
    
public sealed class MatchRepeatStateDefinition<TStateMessage> : IStateDefinition<TStateMessage> {
    private readonly HierachicalName _Name;

    /// <summary>
    /// Initializes a new instance of the <see cref="MatchRepeatStateDefinition{TStateMessage}"/> class.
    /// </summary>
    /// <param name="name">The hierarchical name of this state definition.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null.</exception>
    public MatchRepeatStateDefinition(HierachicalName name) {
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

    public IStateRunning<TStateMessage> CreateStateRunning(IStateRunning<TStateMessage>? previous) 
        => new MatchRepeatStateRunning(this, previous);

    public sealed class MatchRepeatStateRunning : IStateRunning<TStateMessage> {
        private readonly MatchRepeatStateDefinition<TStateMessage> _Definition;
        private readonly IStateRunning<TStateMessage>? _Previous;
        private readonly List<TStateMessage> _Messages = new List<TStateMessage>();

        public MatchRepeatStateRunning(MatchRepeatStateDefinition<TStateMessage> definition, IStateRunning<TStateMessage>? previous) {
            this._Definition = definition;
            this._Previous = previous;
        }

        IStateRunning<TStateMessage>? IStateRunning<TStateMessage>.Previous => this._Previous;

        IStateDefinition<TStateMessage> IStateRunning<TStateMessage>.Definition => this._Definition;

        public List<TStateMessage> Messages => this._Messages;

        public void HandleIncoming(TStateMessage message, StateTransitionControl<TStateMessage> stateTransition) {
            bool doesMatch = this._Definition.Condition.DoesMatch(message);
            if (doesMatch) {
                stateTransition.AddStay(this);
            } else {
                stateTransition.AddNextOrStayOrTerminate(this, this._Definition.FalseCase);
            }
        }
    }
}

public class MatchRepeatStateDefinitionBuilder<TStateMessage> {
    private readonly MatchRepeatStateDefinition<TStateMessage> _StateDefinition;

    /// <summary>
    /// Initializes a new instance of the <see cref="MatchRepeatStateDefinitionBuilder{TStateMessage}"/> class.
    /// </summary>
    /// <param name="stateDefinition">The state definition to configure.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="stateDefinition"/> is null.</exception>
    public MatchRepeatStateDefinitionBuilder(
        MatchRepeatStateDefinition<TStateMessage> stateDefinition
        ) {
        this._StateDefinition = stateDefinition ?? throw new ArgumentNullException(nameof(stateDefinition));
    }

    /// <summary>
    /// Gets a builder for configuring the true case state definition.
    /// </summary>
    public IStateDefinitionBuilder<TStateMessage> TrueCase
        => new PropertyStateDefinitionBuilder<TStateMessage, MatchRepeatStateDefinition<TStateMessage>>(
            this._StateDefinition,
            static (that, value) => that.TrueCase = value);

    /// <summary>
    /// Gets a builder for configuring the false case state definition.
    /// </summary>
    public IStateDefinitionBuilder<TStateMessage> FalseCase
        => new PropertyStateDefinitionBuilder<TStateMessage, MatchRepeatStateDefinition<TStateMessage>>(
            this._StateDefinition,
            static (that, value) => that.FalseCase = value);
}


public static partial class StateMaschineExtensions {
    public static MatchRepeatStateDefinitionBuilder<TStateMessage> AddMatchRepeat<TStateMessage>(
            this IStateDefinitionBuilder<TStateMessage> builder,
            HierachicalName name,
            Condition<TStateMessage>? condition = default,
            IStateDefinition<TStateMessage>? trueCase = default,
            IStateDefinition<TStateMessage>? falseCase = default
        ) {

        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(name);

        var result = new MatchRepeatStateDefinition<TStateMessage>(name);
        if (condition is { }) { result.Condition = condition; }
        if (trueCase is { }) { result.TrueCase = trueCase; }
        if (falseCase is { }) { result.FalseCase = falseCase; }
        if (builder is IStateDefinitionBuilder<TStateMessage> stateMaschine) {
            stateMaschine.AddStateDefinition(result);
        }
        return new(result);
    }
}