using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SampleWebApp;

namespace SampleWebApp.Pages.UIEbbes
{
    public class EditModel : PageModel
    {
        private readonly SampleWebApp.DatabaseContext _DatabaseContext;

        public EditModel(SampleWebApp.DatabaseContext databaseContext)
        {
            _DatabaseContext = databaseContext;
        }

        [BindProperty]
        public Ebbes Ebbes { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ebbes =  await _DatabaseContext.Ebbes.FirstOrDefaultAsync(m => m.Id == id);
            if (ebbes == null)
            {
                return NotFound();
            }
            Ebbes = ebbes;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _DatabaseContext.Attach(Ebbes).State = EntityState.Modified;

            try
            {
                await _DatabaseContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EbbesExists(Ebbes.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool EbbesExists(Guid id)
        {
            return _DatabaseContext.Ebbes.Any(e => e.Id == id);
        }
    }
}
