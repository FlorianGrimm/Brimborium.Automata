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
    public class IndexModel : PageModel
    {
        private readonly SampleWebApp.DatabaseContext _DatabaseContext;

        public IndexModel(SampleWebApp.DatabaseContext databaseContext)
        {
            _DatabaseContext = databaseContext;
        }

        public IList<Ebbes> Ebbes { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Ebbes = await _DatabaseContext.Ebbes.ToListAsync();
        }
    }
}
