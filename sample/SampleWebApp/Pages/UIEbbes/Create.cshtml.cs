using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SampleWebApp;

namespace SampleWebApp.Pages.UIEbbes
{
    public class CreateModel : PageModel
    {
        private readonly SampleWebApp.DatabaseContext _DatabaseContext;

        public CreateModel(SampleWebApp.DatabaseContext databaseContext)
        {
            _DatabaseContext = databaseContext;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Ebbes Ebbes { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _DatabaseContext.Ebbes.Add(Ebbes);
            await _DatabaseContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
