namespace SampleWebApp.Playwright;

public class Tests : AppPageTest {

    [Test]
    public async Task Test001() {
        await this.GotoHomeAsync();

        // await Expect(Page)
        /*
        await Expect(Page).ToHaveURLAsync("");

        await this.ThisAppDefinition.HomePage.ExpectedState(this);

        var currentState = this.GetCurrentState();

        await Assert.That(this).IsCurrentPage(this.ThisAppDefinition.HomePage);
        */
        //// Expect a title "to contain" a substring.
        //await Expect(Page).ToHaveTitleAsync(new Regex("Playwright"));

        //// create a locator
        //var getStarted = Page.Locator("text=Get Started");

        //// Expect an attribute "to be strictly equal" to the value.
        //await Expect(getStarted).ToHaveAttributeAsync("href", "/docs/intro");

        //// Click the get started link.
        //await getStarted.ClickAsync();

        //// Expects the URL to contain intro.
        //await Expect(Page).ToHaveURLAsync(new Regex(".*intro"));
    }
}
