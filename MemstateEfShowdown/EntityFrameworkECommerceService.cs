using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace MemstateEfShowdown
{
    public class EntityFrameworkECommerceService : IEcommerceService
    {
        private readonly IkeaContext _context;


        public EntityFrameworkECommerceService(IkeaContext context)
        {
            _context = context;
        }
        public Task<List<Product>> GetProductsPage( int skip = 0, int take = 10)
        {
            return _context.Products
                .OrderBy(p => p.Id)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public Task<Product> GetProductById(int id)
        {
            return _context.Products.SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task<int> PlaceOrder(int customerId, DateTimeOffset placed, List<OrderItem> items)
        {
            var order = new Order();
            items.ForEach(item => order.Items.Add(item));
            order.Placed = placed;
            order.Customer = await _context.Customers.SingleAsync(c => c.Id == customerId);
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order.Id;
        }


        public Task<List<StockLevel>> GetStockLevelsByProduct(int productId)
        {
            return _context.StockLevels.Where(stockLevel => stockLevel.Product.Id == productId)
                .OrderByDescending(stockLevel => stockLevel.UnitsInStock).Take(20).ToListAsync();
        }
    }
}