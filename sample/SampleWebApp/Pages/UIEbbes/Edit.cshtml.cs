using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SampleWebApp;

namespace SampleWebApp.Pages.UIEbbes;

public class EditModel : PageModel
{
    private readonly SampleWebApp.DatabaseContext _DatabaseContext;

    public EditModel(SampleWebApp.DatabaseContext databaseContext)
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

        var ebbes =  await this._DatabaseContext.Ebbes.FirstOrDefaultAsync(m => m.Id == id);
        if (ebbes == null)
        {
            return this.NotFound();
        }
        this.Ebbes = ebbes;
        return this.Page();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more information, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!this.ModelState.IsValid)
        {
            return this.Page();
        }

        this._DatabaseContext.Attach(this.Ebbes).State = EntityState.Modified;

        try
        {
            await this._DatabaseContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!this.EbbesExists(this.Ebbes.Id))
            {
                return this.NotFound();
            }
            else
            {
                throw;
            }
        }

        return this.RedirectToPage("./Index");
    }

    private bool EbbesExists(Guid id)
    {
        return this._DatabaseContext.Ebbes.Any(e => e.Id == id);
    }
}
