namespace SampleWebApp.Playwright.Utility;

public class PageDefinition {

    public PageDefinition(
        UrlTemplate urlTemplate
        ) {
        this.UrlTemplate = urlTemplate;
    }

    public UrlTemplate UrlTemplate { get; init; }
}


public class AppDefinition {
    public List<PageDefinition> ListPageDefinition { get; } = [];
    public UrlMatcher UrlMatcher { get; } = new UrlMatcher();
    protected T AddPageDefinition<T>(T page)
        where T : PageDefinition {
        this.ListPageDefinition.Add(page);
        this.UrlMatcher.Add(page.UrlTemplate, page);
        return page;
    }

    public UrlMatch GetPageDefinitionFromUrl(string url) {
        return this.UrlMatcher.GetPageDefinitionFromUrl(url);
    }
}
