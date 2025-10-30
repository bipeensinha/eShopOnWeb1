using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
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

                        // Ensure DB is created and seeded
                        db.Database.EnsureCreated();
                        if (!db.CatalogBrands.Any())
                        {
                            SeedData.PopulateTestData(db);
                            db.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] DB initialization failed: {ex.Message}");
                        throw;
                    }
                });
            });

        await Task.CompletedTask;
    }
}
