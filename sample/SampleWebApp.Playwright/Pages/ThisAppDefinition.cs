
using SampleWebApp.Playwright.Pages.UIEbbes;

namespace SampleWebApp.Playwright.Pages;

public class ThisAppDefinition : AppDefinition {
    public ThisAppDefinition() {
        this.HomePage = this.AddPageDefinition(new HomePage());
        this.PrivacyPage = this.AddPageDefinition(new PrivacyPage());
    
        this.UIEbbesIndexPage = this.AddPageDefinition(new UIEbbesIndexPage());
        this.UIEbbesCreatePage = this.AddPageDefinition(new UIEbbesCreatePage());
        this.UIEbbesDetailsPage = this.AddPageDefinition(new UIEbbesDetailsPage());
        this.UIEbbesEditPage = this.AddPageDefinition(new UIEbbesEditPage());
        this.UIEbbesDeletePage = this.AddPageDefinition(new UIEbbesDeletePage());
    }

    public HomePage HomePage { get; }
    public PrivacyPage PrivacyPage { get; }

    public UIEbbes.UIEbbesIndexPage UIEbbesIndexPage { get; }
    public UIEbbes.UIEbbesCreatePage UIEbbesCreatePage { get; }
    public UIEbbes.UIEbbesDetailsPage UIEbbesDetailsPage { get; }
    public UIEbbes.UIEbbesEditPage UIEbbesEditPage { get; }
    public UIEbbes.UIEbbesDeletePage UIEbbesDeletePage { get; }

    
}
