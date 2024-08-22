using Infrustructure.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrustructure
{
    public class TestingContext : DbContext
    {
        public TestingContext(DbContextOptions<TestingContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer("Server=CTC-LAPT-128\\MSSQLSERVER01;Database=Testing;Trusted_Connection=True;TrustServerCertificate=True");
        }
    }
}
