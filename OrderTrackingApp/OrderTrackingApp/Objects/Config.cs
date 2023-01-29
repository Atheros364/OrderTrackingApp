using SQLite;


namespace OrderTrackingApp.Objects
{
    [Table("Configs")]
    public class Config
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Value { get; set; }
    }
}
