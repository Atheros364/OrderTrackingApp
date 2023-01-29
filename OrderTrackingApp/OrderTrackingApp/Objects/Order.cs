using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;

namespace OrderTrackingApp.Objects
{
    [Table("Order")]
    public class Order
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        public bool IsClientOrder { get; set; }

        public bool IsPayed { get; set; }

        [MaxLength(200)]
        public string ClientName { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<OrderItem> Items { get; set; }
    }
}