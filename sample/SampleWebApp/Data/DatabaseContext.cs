using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SampleWebApp;

public class DatabaseContext : DbContext
{
    public DatabaseContext (DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public DbSet<SampleWebApp.Ebbes> Ebbes { get; set; } = default!;
}
