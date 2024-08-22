using Infrustructure;
using Infrustructure.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();

        // Add services to the container.
        builder.Services.AddDbContext<TestingContext>(o =>
        {
            o.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        // Configure Redis cache
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = "localhost:6379"; // Redis server address
            options.InstanceName = "SampleInstance:";
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        app.UseHttpsRedirection();

        app.MapGet("/product", async (TestingContext db, IDistributedCache cache) =>
        {
            //db.Database.EnsureCreated();

            //var product = db.Products.Add(new Product
            //{
            //    Name = "test",
            //    Price = 10
            //});

            //var produc1 = db.Products.Add(new Product
            //{
            //    Name = "test",
            //    Price = 10
            //});

            //db.SaveChanges();

            var cacheKey = "myKey";
            var cachedValue = await cache.GetStringAsync(cacheKey);

            if (cachedValue == null)
            {
                cachedValue = "Value not in cache";
                await cache.SetStringAsync(cacheKey, cachedValue);

                cachedValue = await cache.GetStringAsync(cacheKey);
            }

            return cachedValue;//product.Entity.Id;
        });

        app.Run();
    }
}


