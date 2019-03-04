using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MemstateEfShowdown
{
    public interface IEcommerceService
    {
        Task<List<Product>> GetProductsPage( int skip = 0, int take = 10);

        Task<Product> GetProductById(int id);

        //Create an order, return the order id
        Task<int> PlaceOrder(int customerId, DateTimeOffset placed, List<OrderItem> items);

        Task<List<StockLevel>> GetStockLevelsByProduct(int productId);

    }
}