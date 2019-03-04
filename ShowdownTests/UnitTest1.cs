using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Threading;
using System.Threading.Tasks;
using Memstate;
using Memstate.Configuration;
using MemstateEfShowdown;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShowdownTests
{
    [TestClass]
    public class UnitTest1
    {
        private string _connectionString =
            "Data Source=lenin;Initial Catalog=Ikea;Integrated Security=True;Pooling=False";

        private IkeaContext _context;
        private Client<InmemoryModel> _memstate;

        private static InmemoryModel _model;

        static UnitTest1()
        {
            _model = InmemoryModel.BuildAndSeedDemo();
            Config.Current.SerializerName = "Wire";

        }


        private void Time(string name, int iterations, Func<Task> func)
        {
            Console.WriteLine("================= " + name + " ========================");
            Console.WriteLine("Iterations: " + iterations);
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            for (int i = 0; i < iterations; i++)
            {
                func.Invoke().GetAwaiter().GetResult();
            }
            stopWatch.Stop();
            var elapsed = stopWatch.ElapsedMilliseconds;
            var actionsPerSecond = iterations / (elapsed / 1000.0);
            Console.WriteLine("Requests per second: " + actionsPerSecond);
            Console.WriteLine("================= Done ========================");

        }

        [TestInitialize]
        public void Setup()
        {
            //Database.SetInitializer(new IkeaContextInitializer());
            _context = new IkeaContext(_connectionString);
            Config.Current.GetSettings<EngineSettings>().StreamName = Guid.NewGuid().ToString();
            var engine = new EngineBuilder().Build(_model).GetAwaiter().GetResult();
            _memstate = new LocalClient<InmemoryModel>(engine);

        }

        [TestMethod]
        public void GetProductsEf()
        {
            var service = new EntityFrameworkECommerceService(_context);
            Time(nameof(GetProductsEf), 10000,  () => service.GetProductsPage());
        }


        [TestMethod]
        public void MixedWorkloadMemstate()
        {
            var rng = new Random(0);
            var service = new InmemoryECommerceService(_memstate);
            Time(nameof(MixedWorkloadMemstate), 10000, () => MixedWorkload(service,rng));
            var topProducts = _model.GetTopProducts();
            topProducts.ForEach(p => Console.WriteLine(p.Id + ": " + p.QuantitySold));
            Assert.IsTrue(topProducts.First().QuantitySold > 0);
        }

        [TestMethod]
        public void MixedWorkloadEf()
        {
            var rng = new Random(0);
            var service = new EntityFrameworkECommerceService(_context);
            Time(nameof(MixedWorkloadEf), 10000, () => MixedWorkload(service, rng));
        }

        private Task MixedWorkload(IEcommerceService service, Random rng)
        {
            return Task.Run(async () =>
            {
                //90% of workload is queries
                if (rng.NextDouble() < 0.9)
                {
                    int productId = rng.Next(1,100000);
                    await service.GetProductById(productId);
                }
                else
                {
                    var customer = rng.Next(1, 1000);
                    await service.PlaceOrder(customer, DateTimeOffset.Now, await RandomItems(service, rng));
                }
            });
        }

        private async Task<List<OrderItem>> RandomItems(IEcommerceService service, Random rng)
        {
            var numItems = rng.Next(1, 5);
            var items = new List<OrderItem>();
            for (int i = 0; i < numItems; i++)
            {
                var productId = rng.Next(1, 100000);
                var randomItem = new OrderItem
                {
                    Quantity = rng.Next(1, 5),
                    UnitPrice = rng.Next(39, 21399),
                    Product = await service.GetProductById(productId)
                };
                items.Add(randomItem);
            }

            return items;
        }


        [TestMethod]
        public void GetProductsMemstate()
        {
            var service = new InmemoryECommerceService(_memstate);
            Time(nameof(GetProductsMemstate), 10000, () => service.GetProductsPage());
        }
    }
}
