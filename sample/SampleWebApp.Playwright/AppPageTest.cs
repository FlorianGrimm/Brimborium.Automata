using Microsoft.Playwright;

using TUnit.Playwright;

namespace SampleWebApp.Playwright;

public class AppPageTest : PageTest {
    [ClassDataSource<WebApplicationFactoryIntegration>(Shared = SharedType.PerTestSession)]
    public required WebApplicationFactoryIntegration WebApplicationFactory { get; init; }

    public override BrowserNewContextOptions ContextOptions(TestContext testContext) {
        return new() {
            Locale = "en-US",
            ColorScheme = ColorScheme.Light,
        };
    }

    public async Task GotoHomeAsync(PageGotoOptions? options = default) {
        var baseAddress = this.WebApplicationFactory.GetBaseAddress();
        await this.Page.GotoAsync(baseAddress, options);

        await this.ValidateAsync("/");
    }

    public async Task GotoPageAndValidateAsync(
        string url,
        PageGotoOptions? options = default) {
        await this.Page.GotoAsync(url, options);
    }

    public async Task ValidateAsync(
        string template,
        PageGotoOptions? options = default) {
        await Task.CompletedTask;
    }
}
