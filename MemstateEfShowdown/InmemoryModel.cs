using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;

namespace MemstateEfShowdown
{
    public class InmemoryModel
    {
        private int _nextOrderId = 1;

        private IDictionary<int, Customer> _customers 
            = new SortedDictionary<int, Customer>();

        private SortedDictionary<int,Order> _orders 
            = new SortedDictionary<int, Order>();

        private SortedDictionary<int, Warehouse> _warehouses
            = new SortedDictionary<int, Warehouse>();

        private SortedDictionary<int, Product> _products
            = new SortedDictionary<int, Product>();

        private const int TopProductCount = 10;
        private List<Product> _topProducts = new List<Product>(TopProductCount);

        public List<Product> GetProducts(int skip, int take)
        {
            return _products
                .Skip(skip)
                .Take(take)
                .Select(kvp => kvp.Value.DisconnectedClone())
                .ToList();
        }

        public Product GetProductById(int id)
        {
            _products.TryGetValue(id, out var result);
            return result;
        }

        public List<Product> GetTopProducts()
        {
            return _topProducts.Select(p => p.DisconnectedClone()).ToList();
        }

        public List<StockLevel> GetStockLevelsByProduct(int productId)
        {
            if (!_products.TryGetValue(productId, out var product)) return new List<StockLevel>();
            return product.StockLevels.OrderByDescending( stockLevel => stockLevel.UnitsInStock).Take(10).ToList();
        }

        public int PlaceOrder(int customerId, DateTimeOffset placed, List<OrderItem> orderItems)
        {
            UpdateMostSoldProducts(orderItems);

            if (!_customers.TryGetValue(customerId, out var customer)) throw new ArgumentException("No such customer " + customerId);
            var order = new Order
            {
                Id = _nextOrderId++,
                Customer = customer,
                Placed = placed,
                Items = new List<OrderItem>(orderItems)
            };
            orderItems.ForEach(item => item.Order = order);
            customer.Orders.Add(order);
            _orders[order.Id] = order;
            return order.Id;
        }

        private void UpdateMostSoldProducts(List<OrderItem> orderItems)
        {
            foreach (var item in orderItems)
            {
                var product = _products[item.Product.Id];
                product.QuantitySold += item.Quantity;
                _topProducts.Remove(product);
                _topProducts.Add(product);
                _topProducts.Sort((a,b) => b.QuantitySold - a.QuantitySold);
                if (_topProducts.Count > TopProductCount)
                {
                    _topProducts.Remove(_topProducts.Last());
                }
            }
        }

        public static InmemoryModel BuildAndSeedDemo()
        {
            var model = new InmemoryModel();
            var rng = new Random(0);

            for (int i = 1; i <= 1000; i++)
            {
                var customer = new Customer()
                {
                    Id = i,
                    Name = "Customer " + i
                };

                model._customers.Add(i, customer);
            }

            for (int i = 1; i <= 1000000; i++)
            {
                var product = new Product()
                {
                    Id = i,
                    Name = "Product " + i,
                    Description = "Description of product " + i,
                    UnitPrice = rng.Next(10, 1000)
                };
                model._products.Add(i, product);
            }

            for (int i = 1; i <= 10000; i++)
            {
                var warehouse = new Warehouse
                {
                    Id = i,
                    Location = "Warehouse " + i
                };
                model._warehouses.Add(i, warehouse);
            }

            for (int i = 1; i <= 900; i++)
            {
                var stockLevels = new SortedDictionary<int, int>();

                for (int j = 0; j < 1000; j++)
                {
                    int randomWarehouse = rng.Next(1, 10000);
                    int level = rng.Next(0, 1200);
                    if (rng.NextDouble() < 0.1) level = 0; //10% chance out of stock
                    stockLevels[randomWarehouse] = level;
                }

                var product = model._products[i];
                foreach (var warehouseId in stockLevels.Keys)
                {
                    var warehouse = model._warehouses[warehouseId];
                    product.StockLevels.Add(new StockLevel
                    {
                        Product = product,
                        Warehouse = warehouse,
                        UnitsInStock =  stockLevels[warehouseId]
                    });
                }
                
            }
            return model;
        }
    }
}