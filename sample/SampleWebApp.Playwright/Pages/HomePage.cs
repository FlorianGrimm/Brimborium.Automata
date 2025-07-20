namespace SampleWebApp.Playwright.Pages;

public class HomePage : ThisPageDefinition {
    public HomePage(
        ) : base(
        urlTemplate: UrlTemplate.Parse("/")
        ) {
    }
}
public class PrivacyPage : ThisPageDefinition {
    public PrivacyPage(
        ) : base(
        urlTemplate: UrlTemplate.Parse("/")
        ) {
    }
}
