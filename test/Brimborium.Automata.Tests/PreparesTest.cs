// Here you could define global logic that would affect all tests

// You can use attributes at the assembly level to apply to all tests in the assembly
//[assembly: Retry(3)]

namespace Brimborium.Automata.Tests;

public class PreparesTest {
    [Test]
    public Task VerifyChecks() => VerifyTUnit.VerifyChecks.Run();
}