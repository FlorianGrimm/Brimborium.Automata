namespace Brimborium.AutomataCode2;

public class StateMaschineTest {
    [Test]
    public async Task Start01Test() {
        var stateMaschine = new Brimborium.AutomataCode2.StateMaschine<int>();
        var runningStartMaschine = stateMaschine.Start();
        await Assert.That(stateMaschine).IsNotNull();
    }

    [Test]
    public async Task StateMaschine02Test() {
        var stateMaschine = new Brimborium.AutomataCode2.StateMaschine<int>();
        var state1 = stateMaschine.CreateMatchOneState(new ("State1"), "State 1");
        var state2 = stateMaschine.CreateMatchOneState(new("State2"), "State 2");
        var state3 = stateMaschine.CreateMatchOneState(new("State3"), "State 3");
        stateMaschine.StartState.Builder().WithCondition(
            new FuncCondition<int>((m) => m == 1)
            ).ToState(state1);
        state1.AddTransition(new(new FuncCondition<int>((m) => m == 2), state2));
        state2.AddTransition(new(new FuncCondition<int>((m) => m == 3), state3));
        state3.AddTransition(new(new EpsilonCondition<int>(), stateMaschine.StartState));

        var runningStartMaschine = stateMaschine.Start();
        runningStartMaschine = runningStartMaschine.HandleMessage(1);
        runningStartMaschine = runningStartMaschine.HandleMessage(2);
        runningStartMaschine = runningStartMaschine.HandleMessage(3);

        await Assert.That(stateMaschine).IsNotNull();
    }

}
