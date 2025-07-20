namespace SampleWebApp.Playwright.Pages.UIEbbes;

public class UIEbbesIndexPage : ThisPageDefinition {
    public UIEbbesIndexPage(
        ) : base(
            urlTemplate: UrlTemplate.Parse("/UIEbbes")
        ) {
    }
}

public class UIEbbesCreatePage : ThisPageDefinition {
    public UIEbbesCreatePage(
        ) : base(
            urlTemplate: UrlTemplate.Parse("/UIEbbes/Create")
        ) {
    }
}

public class UIEbbesDetailsPage : ThisPageDefinition {
    public UIEbbesDetailsPage(
        ) : base(
            urlTemplate: UrlTemplate.Parse("/UIEbbes/Details?id={EbbesId}")
        ) {
    }
}

public class UIEbbesEditPage : ThisPageDefinition {
    public UIEbbesEditPage(
        ) : base(
            urlTemplate: UrlTemplate.Parse("/UIEbbes/Edit?id={EbbesId}")
        ) {
    }
}

public class UIEbbesDeletePage : ThisPageDefinition {
    public UIEbbesDeletePage(
        ) : base(
            urlTemplate: UrlTemplate.Parse("/UIEbbes/Edit?id={EbbesId}")
        ) {
    }
}
