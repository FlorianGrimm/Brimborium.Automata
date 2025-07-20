namespace Brimborium.AutomataCode;

public class StateMaschineTests {
    [Test]
    public async Task ThinkOf() {
        var stateMaschine = new StateMaschine<int>();
        await Assert.That(stateMaschine).IsNotNull();
        var state1 = stateMaschine.AddMatchOne(new("1"), stateMaschine.CreateDelegateCondition((value) => value == 1));
        var state2 = state1.TrueCase.AddMatchOne(new("2"), stateMaschine.CreateDelegateCondition((value) => value == 2));
        var state3 = state2.TrueCase.AddMatchOne(new("3"), stateMaschine.CreateDelegateCondition((value) => value == 3));
        state3.TrueCase.AddReturn(new("4"));
        stateMaschine.Start();
        await Assert.That(stateMaschine.HandleIncoming(1)).IsNull();
        await Assert.That(stateMaschine.GetListCurrentState().Count).IsEqualTo(1);
        await Assert.That(stateMaschine.HandleIncoming(2)).IsNull();
        await Assert.That(stateMaschine.GetListCurrentState().Count).IsEqualTo(1);
        await Assert.That(stateMaschine.HandleIncoming(3)).IsNotNull();
        //await Assert.That(stateMaschine.GetListCurrentState().Count).IsEqualTo(0);
    }


    private class ProcessorTextInt(StateMaschine<int> stateMaschine) : Processor<string, int>(stateMaschine) {
        public override void HandleIncoming(string incoming) {
            if (int.TryParse(incoming, out var value)) {
                this._StateMaschine.HandleIncoming(0);
            }
        }
    }
}
