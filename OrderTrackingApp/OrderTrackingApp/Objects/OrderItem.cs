using SQLite;
using SQLiteNetExtensions.Attributes;

namespace OrderTrackingApp.Objects
{
    [Table("OrderItem")]
    public class OrderItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string ItemName { get; set; }

        [ForeignKey(typeof(DefaultItem))]
        public int DefaultItemId { get; set; }

        public int Count { get; set; }

        public decimal Price { get; set; }

        [ForeignKey(typeof(Order))]
        public int OrderId { get; set; }
    }
}