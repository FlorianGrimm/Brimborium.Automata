namespace Brimborium.AutomataCode;

/// <summary>
/// Defines the different kinds of state transitions that can occur in the state machine.
/// </summary>
public enum StateRunningTransitionKind {
    /// <summary>Starting a new state execution.</summary>
    Start,

    /// <summary>Staying in the current state.</summary>
    Stay,

    /// <summary>Moving to the next state.</summary>
    Next,

    /// <summary>Creating a parallel execution path (fork).</summary>
    Fork,

    /// <summary>Terminating the current state execution.</summary>
    Terminate,

    /// <summary>Return the current state as a result.</summary>
    Return
};
