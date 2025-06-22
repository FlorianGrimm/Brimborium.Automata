namespace Brimborium.Automata;

public class IdeaTests {
    [Test]
    public async Task Idea() {
        var builder = new StateMachineBuilder();
        var initialState = builder.State("InitialState");
        var state1 = builder.State("State1");
        var state2 = builder.State("State2");
        initialState.WithTransition("Event1").To(state1);
        initialState.WithTransition("Event2").To(state2);
        state1.WithTransition("Event2").To(state2);

        var stateMachine = new DeterministicStateMachine(builder);

        stateMachine.Next("Event1");
        await Assert.That(stateMachine.State.Name).IsEqualTo("State1");

        stateMachine.Next("Event2");
        await Assert.That(stateMachine.State.Name).IsEqualTo("State2");
    }
}