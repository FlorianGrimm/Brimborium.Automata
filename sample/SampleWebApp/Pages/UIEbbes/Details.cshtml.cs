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
    public class DetailsModel : PageModel
    {
        private readonly SampleWebApp.DatabaseContext _DatabaseContext;

        public DetailsModel(SampleWebApp.DatabaseContext databaseContext)
        {
            _DatabaseContext = databaseContext;
        }

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
    }
}
