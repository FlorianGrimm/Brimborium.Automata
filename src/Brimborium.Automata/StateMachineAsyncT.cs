namespace Brimborium.Automata;
#if true
/// <summary>
/// Defines the kind of state in a state machine.
/// </summary>
public enum StateKind {
    /// <summary>The initial state of the state machine.</summary>
    Initial,
    /// <summary>A final state indicating the state machine has completed successfully.</summary>
    Final,
    /// <summary>An error state indicating the state machine has encountered an error.</summary>
    Error,
    /// <summary>A normal processing state.</summary>
    Normal
}

/// <summary>
/// Represents a state in a state machine with transitions triggered by events of type <typeparamref name="TEvent"/>.
/// </summary>
/// <typeparam name="TEvent">The type of events that can trigger state transitions.</typeparam>
public class State<TEvent> {
    private readonly List<StateTransition<TEvent>> _ListTransitions = new();
    private readonly string _Name;
    private readonly StateKind _StateKind;
    internal protected StateMachineBuilder<TEvent>? _Builder;

    /// <summary>
    /// Gets the name of the state.
    /// </summary>
    public string Name => this._Name;

    /// <summary>
    /// Gets the kind of the state (Initial, Final, Error, or Normal).
    /// </summary>
    public StateKind StateKind => this._StateKind;

    /// <summary>
    /// Initializes a new instance of the <see cref="State{TEvent}"/> class.
    /// </summary>
    /// <param name="name">The name of the state.</param>
    /// <param name="stateKind">The kind of the state. Defaults to Normal.</param>
    public State(string name, StateKind stateKind = StateKind.Normal) {
        this._Name = name;
        this._StateKind = stateKind;
    }

    /// <summary>
    /// Adds an existing transition to this state.
    /// </summary>
    /// <param name="transition">The transition to add.</param>
    /// <returns>The added transition for further configuration.</returns>
    public StateTransition<TEvent> WithTransition(StateTransition<TEvent> transition) {
        this._ListTransitions.Add(transition);
        return transition;
    }

    /// <summary>
    /// Gets the transition that should be triggered by the specified event.
    /// </summary>
    /// <param name="currentEvent">The event to check against available transitions.</param>
    /// <returns>The matching transition, or null if no transition matches the event.</returns>
    public StateTransition<TEvent>? GetTransition(TEvent currentEvent) {
        // Iterate through all transitions and return the first one that matches the event
        foreach (var transition in this._ListTransitions) {
            if (transition.CheckEvent(currentEvent)) {
                return transition;
            }
        }
        return null;
    }

    /// <summary>
    /// Called when entering this state.
    /// </summary>
    /// <param name="currentEvent">The event that triggered the transition to this state.</param>
    /// <param name="previousState">The previous state, or null if this is the initial state.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual ValueTask OnEnterAsync(TEvent currentEvent, State<TEvent>? previousState) {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Called when exiting this state.
    /// </summary>
    /// <param name="nextEvent">The event that triggered the transition from this state.</param>
    /// <param name="nextState">The next state to transition to.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual ValueTask OnExitAsync(TEvent nextEvent, State<TEvent> nextState) {
        return ValueTask.CompletedTask;
    }
}

public record class StatePayload<TEvent, TPayload> (
    State<TEvent> State,
    TPayload Payload
);

/// <summary>
/// Defines an interface for checking if an event should trigger a state transition.
/// </summary>
/// <typeparam name="TEvent">The type of events that can trigger state transitions.</typeparam>
public interface IStateTransitionChecker<TEvent> {
    /// <summary>
    /// Determines whether the specified event should trigger this transition.
    /// </summary>
    /// <param name="currentEvent">The event to check.</param>
    /// <returns>true if the event should trigger this transition; otherwise, false.</returns>
    bool CheckEvent(TEvent currentEvent);
}

/// <summary>
/// Represents a transition between states in a state machine.
/// </summary>
/// <typeparam name="TEvent">The type of events that can trigger this transition.</typeparam>
public abstract class StateTransition<TEvent> : IStateTransitionChecker<TEvent> {
    private readonly string _EventName;
    private State<TEvent>? _ToState;

    /// <summary>
    /// Gets the target state of this transition.
    /// </summary>
    public State<TEvent>? ToState => this._ToState;

    /// <summary>
    /// Gets the name of the event that triggers this transition.
    /// </summary>
    public string EventName => this._EventName;

    /// <summary>
    /// Initializes a new instance of the <see cref="StateTransition{TEvent}"/> class.
    /// </summary>
    /// <param name="eventName">The name of the event that triggers this transition.</param>
    public StateTransition(string eventName) {
        this._EventName = eventName;
    }

    /// <summary>
    /// Sets the target state for this transition.
    /// </summary>
    /// <param name="state">The state to transition to when this transition is triggered.</param>
    /// <returns>This transition instance for method chaining.</returns>
    public StateTransition<TEvent> To(State<TEvent> state) {
        this._ToState = state;
        return this;
    }

    /// <summary>
    /// Called when this transition is executed.
    /// </summary>
    /// <param name="currentEvent">The event that triggered this transition.</param>
    /// <param name="nextState">The state being transitioned to.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual ValueTask OnExecuteAsync(TEvent currentEvent, State<TEvent> nextState) {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Determines whether the specified event should trigger this transition.
    /// Base implementation always returns false; derived classes should override this.
    /// </summary>
    /// <param name="currentEvent">The event to check.</param>
    /// <returns>true if the event should trigger this transition; otherwise, false.</returns>
    public abstract bool CheckEvent(TEvent currentEvent);
}

/// <summary>
/// A state transition that uses an external checker to determine if an event should trigger the transition.
/// </summary>
/// <typeparam name="TEvent">The type of events that can trigger this transition.</typeparam>
public class StateTransitionWithChecker<TEvent> : StateTransition<TEvent> {
    private readonly IStateTransitionChecker<TEvent> _Checker;

    /// <summary>
    /// Initializes a new instance of the <see cref="StateTransitionWithChecker{TEvent}"/> class.
    /// </summary>
    /// <param name="eventName">The name of the event that triggers this transition.</param>
    /// <param name="checker">The checker that determines if an event should trigger this transition.</param>
    public StateTransitionWithChecker(
        string eventName,
        IStateTransitionChecker<TEvent> checker) : base(eventName) {
        this._Checker = checker;
    }

    /// <summary>
    /// Determines whether the specified event should trigger this transition by delegating to the checker.
    /// </summary>
    /// <param name="currentEvent">The event to check.</param>
    /// <returns>true if the checker determines the event should trigger this transition; otherwise, false.</returns>
    public override bool CheckEvent(TEvent currentEvent) {
        return this._Checker.CheckEvent(currentEvent);
    }
}

/// <summary>
/// A state transition that uses delegate functions to check events and execute actions.
/// </summary>
/// <typeparam name="TEvent">The type of events that can trigger this transition.</typeparam>
public class StateTransitionDelegate<TEvent> : StateTransition<TEvent> {
    private readonly Func<TEvent, bool> _OnCheckEvent;
    private readonly Func<TEvent, State<TEvent>, ValueTask>? _OnExecuteAsync;

    /// <summary>
    /// Initializes a new instance of the <see cref="StateTransitionDelegate{TEvent}"/> class.
    /// </summary>
    /// <param name="eventName">The name of the event that triggers this transition.</param>
    /// <param name="checkEvent">A function that determines if an event should trigger this transition.</param>
    /// <param name="executeAsync">An optional function to execute when this transition is triggered.</param>
    public StateTransitionDelegate(
        string eventName,
        Func<TEvent, bool> checkEvent,
        Func<TEvent, State<TEvent>, ValueTask>? executeAsync
        ) : base(eventName) {
        this._OnCheckEvent = checkEvent;
        this._OnExecuteAsync = executeAsync;
    }

    /// <summary>
    /// Determines whether the specified event should trigger this transition by calling the check function.
    /// </summary>
    /// <param name="currentEvent">The event to check.</param>
    /// <returns>true if the check function returns true; otherwise, false.</returns>
    public override bool CheckEvent(TEvent currentEvent) {
        return this._OnCheckEvent(currentEvent);
    }

    /// <summary>
    /// Executes the transition by calling the execute function if provided.
    /// </summary>
    /// <param name="currentEvent">The event that triggered this transition.</param>
    /// <param name="nextState">The state being transitioned to.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async ValueTask OnExecuteAsync(TEvent currentEvent, State<TEvent> nextState) {
        if (this._OnExecuteAsync is not null) {
            await this._OnExecuteAsync(currentEvent, nextState);
        }
    }
}


/// <summary>
/// A builder for creating state machines with states and transitions.
/// </summary>
/// <typeparam name="TEvent">The type of events that can trigger state transitions.</typeparam>
public class StateMachineBuilder<TEvent> {
    private readonly List<State<TEvent>> _ListState = new();
    private readonly State<TEvent> _InitialState;
    private readonly State<TEvent> _FinalState;
    private readonly State<TEvent> _ErrorState;

    /// <summary>
    /// Gets the list of all states in the state machine.
    /// </summary>
    public List<State<TEvent>> ListState => this._ListState;

    /// <summary>
    /// Gets the initial state of the state machine.
    /// </summary>
    public State<TEvent> InitialState => _InitialState;

    /// <summary>
    /// Gets the final state of the state machine.
    /// </summary>
    public State<TEvent> FinalState => _FinalState;

    /// <summary>
    /// Gets the error state of the state machine.
    /// </summary>
    public State<TEvent> ErrorState => this._ErrorState;

    /// <summary>
    /// Initializes a new instance of the <see cref="StateMachineBuilder{TEvent}"/> class.
    /// </summary>
    /// <param name="initialState">Optional custom initial state. If null, a default one is created.</param>
    /// <param name="finalState">Optional custom final state. If null, a default one is created.</param>
    /// <param name="errorState">Optional custom error state. If null, a default one is created.</param>
    public StateMachineBuilder(
        State<TEvent>? initialState = default,
        State<TEvent>? finalState = default,
        State<TEvent>? errorState = default
        ) {
        // Create and add the standard states (initial, final, error)
        this._ListState.Add(this._InitialState = (initialState ?? new State<TEvent>("InitialState", StateKind.Initial)));
        this._ListState.Add(this._FinalState = (finalState ?? new State<TEvent>("FinalState", StateKind.Final)));
        this._ListState.Add(this._ErrorState = (errorState ?? new State<TEvent>("ErrorState", StateKind.Error)));
        this._InitialState._Builder = this; 
        this._FinalState._Builder = this; 
        this._ErrorState._Builder = this;
    }

    /// <summary>
    /// Creates a new state with the specified name and adds it to the state machine.
    /// </summary>
    /// <param name="name">The name of the state.</param>
    /// <returns>The created state for further configuration.</returns>
    public State<TEvent> State(string name) {
        var state = new State<TEvent>(name, StateKind.Normal);
        state._Builder = this; // Set the builder reference for the state
        this._ListState.Add(state);
        return state;
    }

    /// <summary>
    /// Adds an existing state to the state machine.
    /// </summary>
    /// <param name="state">The state to add.</param>
    /// <returns>Fluent this</returns>
    public StateMachineBuilder<TEvent> State(params State<TEvent>[] states) {
        foreach (var state in states) {
            if (state._Builder is { }) {
                // skip adding if the state already has a builder reference
            } else {
                this._ListState.Add(state);
                state._Builder = this; // Set the builder reference for each state
            }
        }
        return this;
    }
}

/// <summary>
/// An asynchronous state machine that processes events of type <typeparamref name="TEvent"/>.
/// </summary>
/// <typeparam name="TEvent">The type of events that can trigger state transitions.</typeparam>
public class StateMachine<TEvent> {
    private readonly StateMachineBuilder<TEvent> _Builder;
    private State<TEvent>? _State;

    /// <summary>
    /// Initializes a new instance of the <see cref="StateMachine{TEvent}"/> class.
    /// </summary>
    /// <param name="builder">The builder containing the states and transitions for this state machine.</param>
    public StateMachine(StateMachineBuilder<TEvent> builder) {
        this._Builder = builder;
    }

    /// <summary>
    /// Gets the current state of the state machine.
    /// If no state has been set yet, returns the initial state.
    /// </summary>
    public State<TEvent> State {
        get => this._State ?? this._Builder.InitialState;
        set => this._State = value;
    }

    public async Task<State<TEvent>> GotoAsync(TEvent currentEvent, State<TEvent> nextState , StateTransition<TEvent>? transition) {
        var currentState = this._State ?? nextState;
        if (ReferenceEquals(currentState, nextState)) {
            // Self-transition (staying in the same state)
            if (transition is { }) {
                await transition.OnExecuteAsync(currentEvent, nextState);
            }
            return nextState;
        } else {
            // Transition to a different state
            await currentState.OnExitAsync(currentEvent, nextState);
            if (transition is { }) {
                await transition.OnExecuteAsync(currentEvent, nextState);
            }
            this._State = nextState;
            await nextState.OnEnterAsync(currentEvent, currentState);
            return nextState;
        }
    }

    /// <summary>
    /// Gets the builder used to create this state machine.
    /// </summary>
    public StateMachineBuilder<TEvent> Builder => this._Builder;

    /// <summary>
    /// Processes an event and transitions to the next state if applicable.
    /// </summary>
    /// <param name="currentEvent">The event to process.</param>
    /// <returns>A task that represents the asynchronous operation, containing the resulting state.</returns>
    public async Task<State<TEvent>> NextAsync(TEvent currentEvent) {
        var currentState = this._State;
        if (currentState is null) {
            // Initialize the state machine if this is the first event
            currentState = this._Builder.InitialState;
            this._State = currentState;
            await this._State.OnEnterAsync(currentEvent, default);
        }

        // Find the transition for this event, or use error state if no transition is found
        var transition = currentState.GetTransition(currentEvent);
        var nextState = transition?.ToState ?? this._Builder.ErrorState;

        return await this.GotoAsync(currentEvent, nextState, transition).ConfigureAwait(false);
    }
}
#endif
