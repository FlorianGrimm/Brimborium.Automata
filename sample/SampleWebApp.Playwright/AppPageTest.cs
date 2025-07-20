namespace SampleWebApp.Playwright;

public class AppPageTest : PageTest {
    [ClassDataSource<Bridge>(Shared = SharedType.PerTestSession)]
    public required Bridge WebApplicationFactory { get; init; }

    [ClassDataSource<ThisAppDefinition>(Shared = SharedType.PerTestSession)]
    public required ThisAppDefinition ThisAppDefinition { get; init; }

    public override BrowserNewContextOptions ContextOptions(TestContext testContext) {
        return new() {
            Locale = "en-US",
            ColorScheme = ColorScheme.Light,
        };
    }

    public async Task GotoHomeAsync(PageGotoOptions? options = default) {
        var baseAddress = this.WebApplicationFactory.GetBaseAddress();
        await this.Page.GotoAsync(baseAddress, options);

        this.ThisAppDefinition.GetPageDefinitionFromUrl(this.Page.Url);
        //await this.ValidateAsync("/");
    }
    /*
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
    */
}
