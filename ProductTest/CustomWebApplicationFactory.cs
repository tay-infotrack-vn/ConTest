using Infrustructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using Testcontainers.MsSql;
using Testcontainers.Redis;

namespace ProductTest
{
    public class CustomWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        private readonly string _connectionString;
        private readonly string _redisConnectionString;
        public CustomWebApplicationFactory()
        {
            var sqlContainer = new MsSqlBuilder().WithPassword("yourStrong(!)Password").Build();

            sqlContainer.StartAsync().Wait();
            _connectionString = sqlContainer.GetConnectionString();

            var redisContainer = new RedisBuilder()
                                      .WithImage("redis:7.0")
                                      .Build();

            redisContainer.StartAsync().Wait();
            _redisConnectionString = redisContainer.GetConnectionString();
        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                // Clear default configuration sources
                config.Sources.Clear();

                // Add the custom appsettings.Test.json file
                config.AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true);

                // Optionally add environment variables and command line args
                config.AddEnvironmentVariables();
            });

            builder.ConfigureServices(services =>
            {
                services.Remove(services.SingleOrDefault(service => typeof(DbContextOptions<TestingContext>) == service.ServiceType));
                services.Remove(services.SingleOrDefault(service => typeof(DbConnection) == service.ServiceType));

                services.AddDbContext<TestingContext>(o =>
                {
                    o.UseSqlServer(_connectionString);
                });

                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = _redisConnectionString;
                    options.InstanceName = "SampleInstance:";
                });
            });
        }
    }
}
