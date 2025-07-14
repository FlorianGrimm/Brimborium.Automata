namespace SampleWebApp.Pages.UIEbbes;

public class CreateModel : PageModel
{
    private readonly SampleWebApp.DatabaseContext _DatabaseContext;

    public CreateModel(SampleWebApp.DatabaseContext databaseContext)
    {
        this._DatabaseContext = databaseContext;
    }

    public IActionResult OnGet()
    {
        return this.Page();
    }

    [BindProperty]
    public Ebbes Ebbes { get; set; } = default!;

    // For more information, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!this.ModelState.IsValid)
        {
            return this.Page();
        }

        this._DatabaseContext.Ebbes.Add(this.Ebbes);
        await this._DatabaseContext.SaveChangesAsync();

        return this.RedirectToPage("./Index");
    }
}
