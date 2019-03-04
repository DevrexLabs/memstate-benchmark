using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace MemstateEfShowdown
{
    public class IkeaContext : DbContext
    {

        public IkeaContext(string connectionString) 
            : base(connectionString)
        {
            
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<StockLevel> StockLevels { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}