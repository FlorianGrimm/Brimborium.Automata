namespace SampleWebApp.Playwright;

public class TestsIntegration {
    [ClassDataSource<Bridge>(Shared = SharedType.PerTestSession)]
    public required Bridge Bridge { get; init; }

    [Test]
    public async Task PingTest() {
        var client = this.Bridge.CreateClient();

        var response = await client.GetAsync("/ping");

        var stringContent = await response.Content.ReadAsStringAsync();

        await Assert.That(stringContent).StartsWith("pong ");
        await Task.CompletedTask;
    }
}