using System;
using System.Data.Entity;

namespace MemstateEfShowdown
{
    public class IkeaContextInitializer : DropCreateDatabaseIfModelChanges<IkeaContext>
    {
        protected override void Seed(IkeaContext context)
        {
            Random rng = new Random(0);
            Console.WriteLine("Inserting 1000 customers");
            for (int i = 0; i < 1000; i++)
            {
                context.Customers.Add(new Customer() {Name = "Customer " + i});
            }
            context.SaveChanges();

            Console.WriteLine("Inserting 1000000 products");
            for (int i = 0; i < 1000000; i++)
            {
                var randomPrice = rng.Next(50, 10000);
                context.Database.ExecuteSqlCommand(
                    $"INSERT Product(name, description,unitprice) Values('{"Product " + i}','{"Product description " + i}',{randomPrice})");
            }

            Console.WriteLine("Creating 10000 warehouses");
            for (int i = 0; i < 10000; i++)
            {
                context.Database.ExecuteSqlCommand(
                    $"INSERT Warehouse(Location) Values('{"Location " + i}')");
            }

            context.SaveChanges();

            Console.WriteLine("Generating random stock");
            for (int i = 0; i < 1000000; i++)
            {
                for (int j = 0; j < 1000; j++)
                {
                    int level = rng.Next(3, 1200);
                    var sql = $"INSERT StockLevel(WarehouseId, ProductId, UnitsInStock) VALUES({j},{i},{level})";
                    context.Database.ExecuteSqlCommand(sql);
                }
            }
        }
    }
}