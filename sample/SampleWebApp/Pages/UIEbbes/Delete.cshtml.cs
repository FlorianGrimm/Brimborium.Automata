using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SampleWebApp;

namespace SampleWebApp.Pages.UIEbbes;

public class DeleteModel : PageModel
{
    private readonly SampleWebApp.DatabaseContext _DatabaseContext;

    public DeleteModel(SampleWebApp.DatabaseContext databaseContext)
    {
        this._DatabaseContext = databaseContext;
    }

    [BindProperty]
    public Ebbes Ebbes { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (id == null)
        {
            return this.NotFound();
        }

        var ebbes = await this._DatabaseContext.Ebbes.FirstOrDefaultAsync(m => m.Id == id);

        if (ebbes is not null)
        {
            this.Ebbes = ebbes;

            return this.Page();
        }

        return this.NotFound();
    }

    public async Task<IActionResult> OnPostAsync(Guid? id)
    {
        if (id == null)
        {
            return this.NotFound();
        }

        var ebbes = await this._DatabaseContext.Ebbes.FindAsync(id);
        if (ebbes != null)
        {
            this.Ebbes = ebbes;
            this._DatabaseContext.Ebbes.Remove(this.Ebbes);
            await this._DatabaseContext.SaveChangesAsync();
        }

        return this.RedirectToPage("./Index");
    }
}
