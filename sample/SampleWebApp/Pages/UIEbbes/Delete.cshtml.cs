using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SampleWebApp;

namespace SampleWebApp.Pages.UIEbbes
{
    public class DeleteModel : PageModel
    {
        private readonly SampleWebApp.DatabaseContext _DatabaseContext;

        public DeleteModel(SampleWebApp.DatabaseContext databaseContext)
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

            var ebbes = await _DatabaseContext.Ebbes.FirstOrDefaultAsync(m => m.Id == id);

            if (ebbes is not null)
            {
                Ebbes = ebbes;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ebbes = await _DatabaseContext.Ebbes.FindAsync(id);
            if (ebbes != null)
            {
                Ebbes = ebbes;
                _DatabaseContext.Ebbes.Remove(Ebbes);
                await _DatabaseContext.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
