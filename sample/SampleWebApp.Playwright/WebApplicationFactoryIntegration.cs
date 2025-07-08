using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using TUnit.Core.Interfaces;

namespace SampleWebApp.Playwright;
public class WebApplicationFactoryIntegration : IAsyncInitializer {
    private WebApplication? _Application;
    public WebApplication GetApplication() => this._Application ?? throw new InvalidOperationException("Application yet is not set");

    private string? _AddressHttpQ = null;
    private string? _AddressHttpsQ = null;

    public string GetBaseAddress() {
        if (this._Application is null) { throw new Exception("InitializeAsync was not called"); }
        {
            if ((this._AddressHttpsQ ?? this._AddressHttpQ) is { Length: > 0 } baseAddress) {
                return baseAddress;
            }
        }
        {
            var server = this.GetApplication().Services.GetRequiredService<Microsoft.AspNetCore.Hosting.Server.IServer>();
            if (this._AddressHttpsQ is null || this._AddressHttpQ is null) {
                var serverAddressesFeature = server.Features.Get<IServerAddressesFeature>();
                if (serverAddressesFeature is { Addresses: { } addresses }) {
                    foreach (var addressQ in addresses) {
                        if (addressQ is { Length: > 0 } address) {
                            if (address.StartsWith("http:")) {
                                this._AddressHttpQ = address;
                            }
                            if (address.StartsWith("https:")) {
                                this._AddressHttpsQ = address;
                                break;
                            }
                        }
                    }
                }
            }
            if ((this._AddressHttpsQ ?? this._AddressHttpQ) is { Length: > 0 } baseAddress) {
                return baseAddress;
            }
            return string.Empty;
        }
    }

    public HttpClient CreateClient() {
        if (this._Application is null) { throw new Exception("InitializeAsync was not called"); }

        var socketsHandler = new SocketsHttpHandler();
        socketsHandler.Credentials = CredentialCache.DefaultCredentials;
        var result = new HttpClient(socketsHandler, true);
        if ((this.GetBaseAddress() is { Length: > 0 } baseAddress)) {
            result.BaseAddress = new Uri(baseAddress);
        }
        return result;
    }

    public async Task InitializeAsync() {
        string pathStaticAssets = GetPathStaticAssets();
        var contentRoot = Program.GetContentRoot();
        var tsc = new TaskCompletionSource<WebApplication>();
        var taskServer = Program.RunAsync(
            runtimeOrTesttime: false,
            args: new string[] {
                @"--environment=Development",
                $"--contentRoot={contentRoot}",
                @"--applicationName=SampleWebApp",
                $"--StaticAssets={pathStaticAssets}"
            },
            configureWebApplicationBuilder: null,
            configureWebApplication: null,
            runningWebApplication: (app, task) => {
                tsc.SetResult(app);
            });
        await Task.Delay(100);
        this._Application = await tsc.Task;
    }

    private static string GetPathStaticAssets() {
        var result = System.IO.Path.Combine(
            (System.IO.Path.GetDirectoryName(
                typeof(WebApplicationFactoryIntegration).Assembly.Location ?? throw new Exception("")
                ) ?? throw new Exception("")
                ).Replace(@"\sample\SampleWebApp.Playwright\bin\", @"\src\SampleWebApp\bin\"),
            "SampleWebApp.staticwebassets.endpoints.json");
        return result;
    }
}