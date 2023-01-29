using SQLite;
using SQLiteNetExtensions.Attributes;

namespace OrderTrackingApp.Objects
{
    [Table("InventoryItems")]
    public class InventoryItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public DefaultItem ItemType { get; set; }

        [ForeignKey(typeof(DefaultItem))]
        public int DefaultItemId { get; set; }

        public int Count { get; set; }
    }
}