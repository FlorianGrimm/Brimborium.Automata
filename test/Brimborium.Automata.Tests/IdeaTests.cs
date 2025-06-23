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

        var stateMachine = new StateMachine<string>(builder);

        await stateMachine.NextAsync("Event1");
        await Assert.That(stateMachine.State.Name).IsEqualTo("State1");

        await stateMachine.NextAsync("Event2");
        await Assert.That(stateMachine.State.Name).IsEqualTo("State2");
    }
#endif


#if true
    internal class TestEvent {
        public string Name { get; }
        public string Target { get; }
        public int Payload { get; }

        public TestEvent(string name, string target, int payload) {
            this.Name = name;
            this.Target = target;
            this.Payload = payload;
        }
    }

    internal abstract class StateTestEvent : State<TestEvent> {
        public StateTestEvent(string name, StateKind stateKind = StateKind.Normal)
            : base(name, stateKind) {
        }
        public abstract void WithTransition(Pages pages);
    }

    internal record Pages(
        StatePageInitial StatePageInitial,
        StatePageIndex StatePageIndex,
        StatePageCreate StatePageCreate
        ) {
        public StateTestEvent[] ToArray()
            => [this.StatePageInitial,
                this.StatePageIndex,
                this.StatePageCreate];

        public StateTestEvent[] WithTransition() {
            var pages = this.ToArray();
            foreach (var page in pages) {
                page.WithTransition(this);
            }
            return pages;
        }
    }

    internal class StatePageInitial()
      : StateTestEvent("about:blank", StateKind.Normal) {
        public async ValueTask Initialize(StateMachineTestEvent stateMachine) {
            await Assert.That(stateMachine.State).IsSameReferenceAs(this);
            await stateMachine.NextAsync(new TestEvent("Goto", "/Index", 1));
        }

        public override void WithTransition(Pages pages) {
            this.WithTransition(
                new StateTransitionTestEvent("Goto", "/Index"))
                .To(pages.StatePageIndex);
        }
    }

    internal class StatePageIndex()
        : StateTestEvent("/Index", StateKind.Normal) {
        public async ValueTask ButtonHome(StateMachineTestEvent stateMachine) {
            await Assert.That(stateMachine.State).IsSameReferenceAs(this);
            await stateMachine.NextAsync(new TestEvent("Button-Home", "/Index", 1));
        }
        public async ValueTask ButtonCreate(StateMachineTestEvent stateMachine) {
            await Assert.That(stateMachine.State).IsSameReferenceAs(this);
            await stateMachine.NextAsync(new TestEvent("Button-Create", "/Create", 2));
        }
        public override void WithTransition(Pages pages) {
            this.WithTransition(
                new StateTransitionTestEvent("Button-Home", "/Index"))
                .To(pages.StatePageIndex);
            this.WithTransition(
                new StateTransitionTestEvent("Button-Create", "/Create"))
                .To(pages.StatePageCreate);

        }
    }

    internal class StatePageCreate()
        : StateTestEvent("/Create", StateKind.Normal) {
        public async ValueTask ButtonHome(StateMachineTestEvent stateMachine) {
            await Assert.That(stateMachine.State).IsSameReferenceAs(this);
            await stateMachine.NextAsync(new TestEvent("Button-Home", "/Index", 3));
        }
        public override void WithTransition(Pages pages) {
            this.WithTransition(
                new StateTransitionTestEvent("Button-Home", "/Index"))
                .To(pages.StatePageIndex);
        }
    }

    internal class StateTransitionTestEvent : StateTransition<TestEvent> {
        private readonly string _Target;

        public StateTransitionTestEvent(string eventName, string target)
            : base(eventName) {
            this._Target = target;
        }

        public string Target => this._Target;

        public override bool CheckEvent(TestEvent currentEvent) {
            return (this.EventName == currentEvent.Name);
        }

        public override ValueTask OnExecuteAsync(TestEvent currentEvent, State<TestEvent> nextState) {
            return base.OnExecuteAsync(currentEvent, nextState);
        }
    }

    internal class StateMachineTestEvent(StateMachineBuilder<TestEvent> builder)
        : StateMachine<TestEvent>(builder) {
    }


    [Test]
    public async Task Idea() {
        var pages = new Pages(
            new StatePageInitial(),
            new StatePageIndex(),
            new StatePageCreate()
        );
        var builder = new StateMachineBuilder<TestEvent>(
            initialState: pages.StatePageInitial);
        builder.State(pages.WithTransition());
        var stateMachine = new StateMachineTestEvent(builder);

        await Assert.That(stateMachine.State.Name).IsEqualTo("about:blank");

        await pages.StatePageInitial.Initialize(stateMachine);
        await Assert.That(stateMachine.State.Name).IsEqualTo("/Index");

        await pages.StatePageIndex.ButtonHome(stateMachine);
        await Assert.That(stateMachine.State.Name).IsEqualTo("/Index");

        await pages.StatePageIndex.ButtonCreate(stateMachine);
        await Assert.That(stateMachine.State.Name).IsEqualTo("/Create");

        await pages.StatePageCreate.ButtonHome(stateMachine);
        await Assert.That(stateMachine.State.Name).IsEqualTo("/Index");
    }
#endif
}