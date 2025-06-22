namespace Brimborium.Automata;

public class IdeaTests {

#if false
    internal class StateTransitionString : StateTransition<string> {
        public StateTransitionString(string eventName) : base(eventName) { }

        public override bool CheckEvent(string currentEvent) {
            return this.EventName == currentEvent;
        }
    }

    [Test]
    public async Task Idea() {
        var builder = new StateMachineBuilder<string>();        
        var state1 = builder.State("State1");
        var state2 = builder.State("State2");
        builder.InitialState.WithTransition(new StateTransitionString("Event1")).To(state1);
        builder.InitialState.WithTransition(new StateTransitionString("Event2")).To(state2);
        state1.WithTransition(new StateTransitionString("Event2")).To(state2);

        var stateMachine = new DeterministicStateMachine<string>(builder);

        stateMachine.Next("Event1");
        await Assert.That(stateMachine.State.Name).IsEqualTo("State1");

        stateMachine.Next("Event2");
        await Assert.That(stateMachine.State.Name).IsEqualTo("State2");
    }
#endif


#if true
    internal class StateTransitionString : StateTransition<string> {
        public StateTransitionString(string eventName) : base(eventName) { }

        public override bool CheckEvent(string currentEvent) {
            return this.EventName == currentEvent;
        }
    }

    [Test]
    public async Task Idea() {
        var builder = new StateMachineBuilder<string>();        
        var state1 = builder.State("State1");
        var state2 = builder.State("State2");
        builder.InitialState.WithTransition(new StateTransitionString("Event1")).To(state1);
        builder.InitialState.WithTransition(new StateTransitionString("Event2")).To(state2);
        state1.WithTransition(new StateTransitionString("Event2")).To(state2);

        var stateMachine = new StateMachine<string>(builder);

        await stateMachine.NextAsync("Event1");
        await Assert.That(stateMachine.State.Name).IsEqualTo("State1");

        await stateMachine.NextAsync("Event2");
        await Assert.That(stateMachine.State.Name).IsEqualTo("State2");
    }
#endif
}