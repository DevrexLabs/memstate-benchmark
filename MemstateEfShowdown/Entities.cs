using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemstateEfShowdown
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public virtual ICollection<StockLevel> StockLevels { get; set; } 
            = new List<StockLevel>();

        public Product DisconnectedClone()
        {
            var clone = (Product) MemberwiseClone();
            clone.StockLevels = new List<StockLevel>();
            return clone;
        }

        [NotMapped]
        public int QuantitySold { get; set; }

    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
            = new List<Order>();
    }

    public class Order
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public DateTimeOffset Placed { get; set; }

        public virtual ICollection<OrderItem> Items { get; set; }
            = new List<OrderItem>();
    }

    public class StockLevel
    {
        public int Id { get; set; }

        public int WarehouseId { get; set; }
        public int ProductId { get; set; }

        public Warehouse Warehouse { get; set; }

        public Product Product { get; set; }

        public int UnitsInStock { get; set; }
    }

    public class Warehouse
    {
        public int Id { get; set; }
        public string Location { get; set; }
    }


    public class OrderItem
    {
        public int Id { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
    }
}
