using SQLite;

namespace OrderTrackingApp.Objects
{
    [Table("DefaultItems")]
    public class DefaultItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public decimal DefaultBuyPrice { get; set; }
        public decimal DefaultSellPrice { get; set; }
        public int DefaultOrderSize { get; set; }
    }
}