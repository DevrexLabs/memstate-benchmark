using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Memstate;

namespace MemstateEfShowdown
{
    public class InmemoryECommerceService : IEcommerceService
    {
        private readonly Client<InmemoryModel> _memstate;

        public InmemoryECommerceService(Client<InmemoryModel> memstate)
        {
            _memstate = memstate;
        }

        public Task<List<Product>> GetProductsPage(int skip = 0, int take = 10)
        {
            var query = new GetProductsPageQuery(skip, take);
            return _memstate.Execute(query);
        }

        public Task<Product> GetProductById(int id)
        {
            var query = new GetProductByIdQuery(id);
            return _memstate.Execute(query);
        }

        public Task<int> PlaceOrder(int customerId, DateTimeOffset placed, List<OrderItem> items)
        {
            var command = new PlaceOrderCommand
            {
                CustomerId = customerId,
                Placed = placed,
                Items = items
            };
            return _memstate.Execute(command);
        }


        public Task<List<StockLevel>> GetStockLevelsByProduct(int productId)
        {
            var query = new GetStockLevelsByProductQuery(productId);
            return _memstate.Execute(query);
        }
    }


    public class PlaceOrderCommand : Command<InmemoryModel, int>
    {
        public int CustomerId { get; set; }
        public DateTimeOffset Placed { get; set; }
        public List<OrderItem> Items { get; set; }

        public override int Execute(InmemoryModel model)
        {
            return model.PlaceOrder(CustomerId, Placed, Items);
        }
    }

    public class GetStockLevelsByProductQuery : Query<InmemoryModel, List<StockLevel>>
    {
        public int ProductId { get; set; }

        public GetStockLevelsByProductQuery(int productId)
        {
            ProductId = productId;
        }

        public override List<StockLevel> Execute(InmemoryModel model)
        {
            return model.GetStockLevelsByProduct(ProductId);
        }
    }

    public class GetProductByIdQuery : Query<InmemoryModel, Product>
    {
        public int ProductId { get; set; }

        public GetProductByIdQuery(int id)
        {
            ProductId = id;
        }

        public override Product Execute(InmemoryModel model)
        {
            return model.GetProductById(ProductId);
        }
    }

    public class GetProductsPageQuery : Query<InmemoryModel, List<Product>>
    {
        public int Skip { get; set; }
        public int Take { get; set; }

        public GetProductsPageQuery(int skip, int take)
        {
            Skip = skip;
            Take = take;
        }

        public override List<Product> Execute(InmemoryModel model)
        {
            return model.GetProducts(Skip, Take);
        }
    }
}