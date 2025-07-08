/*
dotnet ef migrations add InitialCreate
dotnet ef database update
*/

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using SampleWebApp;

using System.Runtime.CompilerServices;
using System.Threading.Tasks;
namespace SampleWebApp;

public class Program {

    public static async Task Main(string[] args)
        => await RunAsync(true, args).ConfigureAwait(false);

    public static Task RunAsync(
        bool runtimeOrTesttime,
        string[] args,
        Action<WebApplicationBuilder>? configureWebApplicationBuilder = default,
        Action<WebApplication>? configureWebApplication = default,
        Action<WebApplication, Task>? runningWebApplication = default
        ) {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<DatabaseContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseContext") ?? throw new InvalidOperationException("Connection string 'DatabaseContext' not found.")));

        // Add services to the container.
        builder.Services.AddRazorPages();

        if (configureWebApplicationBuilder is { }) { configureWebApplicationBuilder(builder); }

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment()) {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        
        app.MapGet("/ping", () => $"pong {System.DateTimeOffset.Now:u}").AllowAnonymous();

        // StaticAssets
        {
            var cfgStaticAssets = app.Configuration.GetValue<string>("StaticAssets");
            app.MapStaticAssets(cfgStaticAssets);
            app.MapRazorPages().WithStaticAssets();
        }

        
        if (configureWebApplication is { }) { configureWebApplication(app); }
        var taskRun = app.RunAsync();

        if (runningWebApplication is { }) { runningWebApplication(app, taskRun); }
        return taskRun;
    }

    // for test
    public static string GetContentRoot() {
        return _GetContentRoot();

        static string _GetContentRoot([CallerFilePath] string callerFilePath = "") {
            if (System.IO.Path.GetDirectoryName(callerFilePath) is { Length: > 0 } result) {
                return result;
            } else {
                throw new InvalidOperationException("GetContentRoot failed");
            }
        }
    }
}
