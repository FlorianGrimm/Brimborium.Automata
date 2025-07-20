namespace Brimborium.AutomataCode;

/// <summary>
/// Abstract base class for processors that handle incoming raw messages and convert them to state messages for processing by a state machine.
/// This class provides a bridge between external message formats and the internal state machine message format.
/// </summary>
/// <typeparam name="TRawMessage">The type of raw messages received from external sources.</typeparam>
/// <typeparam name="TStateMessage">The type of state messages processed by the state machine.</typeparam>
public abstract class Processor<TRawMessage, TStateMessage> {
    /// <summary>
    /// The state machine that processes the converted state messages.
    /// </summary>
    protected readonly StateMaschine<TStateMessage> _StateMaschine;

    /// <summary>
    /// Initializes a new instance of the <see cref="Processor{TRawMessage, TStateMessage}"/> class.
    /// </summary>
    /// <param name="stateMaschine">The state machine that will process the converted state messages.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="stateMaschine"/> is null.</exception>
    public Processor(StateMaschine<TStateMessage> stateMaschine) {
        this._StateMaschine = stateMaschine ?? throw new ArgumentNullException(nameof(stateMaschine));
    }

    /// <summary>
    /// Gets the state machine associated with this processor.
    /// </summary>
    public StateMaschine<TStateMessage> StateMaschine => this._StateMaschine;

    /// <summary>
    /// Handles an incoming raw message by converting it to the appropriate state message format and processing it through the state machine.
    /// Derived classes must implement this method to define how raw messages are processed and converted.
    /// </summary>
    /// <param name="incoming">The incoming raw message to process.</param>
    /// <example>
    /// <code>
    /// public override void HandleIncoming(string incoming) {
    ///     var extracted = this.ExtractInformation(incoming);
    ///     this._StateMaschine.HandleIncoming(extracted);
    /// }
    /// </code>
    /// </example>
    public abstract void HandleIncoming(TRawMessage incoming);
}
