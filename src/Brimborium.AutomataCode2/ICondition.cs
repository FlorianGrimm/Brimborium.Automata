namespace Brimborium.AutomataCode2;

public interface ICondition<TMessage> {
    bool DoesMatch(TMessage message);
}

public sealed class EpsilonCondition<TMessage> : ICondition<TMessage> {
    public bool DoesMatch(TMessage message) {
        return true; // Epsilon transition always matches
    }
}

public class AlwaysTrueCondition<TMessage> : ICondition<TMessage> {
    public bool DoesMatch(TMessage message) {
        return true; // Always matches
    }
}

public class AlwaysFalseCondition<TMessage> : ICondition<TMessage> {
    public bool DoesMatch(TMessage message) {
        return false; // Never matches
    }
}

public class FuncCondition<TMessage> : ICondition<TMessage> {
    private readonly Func<TMessage, bool> _FuncDoesMatch;

    public FuncCondition(
        Func<TMessage, bool> funcDoesMatch) {
        this._FuncDoesMatch = funcDoesMatch;
    }

    public bool DoesMatch(TMessage message) {
        return this._FuncDoesMatch(message);
    }
}