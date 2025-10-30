using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.eShopWeb.PublicApi;
using Microsoft.eShopWeb.Infrastructure;
using Microsoft.eShopWeb.Infrastructure.Data;
using System.Net.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PublicApiIntegrationTests;

[TestClass]
public class ProgramTest
{
    private static WebApplicationFactory<Program> _application = null!;

    public static HttpClient NewClient => _application.CreateClient();

    [AssemblyInitialize]
    public static async Task AssemblyInitialize(TestContext _)
    {
        _application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    using var scope = services.BuildServiceProvider().CreateScope();
                    var scopedServices = scope.ServiceProvider;

                    try
                    {
                        var db = scopedServices.GetRequiredService<CatalogContext>();
                        var logger = scopedServices.GetRequiredService<ILogger<CatalogContextSeed>>();

                        db.Database.EnsureCreated();

                        // ✅ Seed the database asynchronously
                        Task.Run(async () => await CatalogContextSeed.SeedAsync(db, logger)).GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Database initialization failed: {ex.Message}");
                        throw;
                    }
                });
            });

        await Task.CompletedTask;
    }
}
