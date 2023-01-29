using System;
using System.Collections.Generic;
using SQLite;
using OrderTrackingApp.Objects;
using System.IO;
using SQLiteNetExtensions.Extensions;
using System.Linq;

namespace OrderTrackingApp.DAL
{
    public static class DAL
    {

        #region HelperMethods
        private static string getDBPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),"orderAppDB.db3");
        }

        public static bool TableExists(SQLiteConnection connection, string tableName)
        {
            const string cmdText = "SELECT name FROM sqlite_master WHERE type='table' AND name=?";
            var cmd = connection.CreateCommand(cmdText, tableName);
            return cmd.ExecuteScalar<string>() != null;
        }

        private static SQLiteConnection InitializeDatabase()
        {
            var db = new SQLiteConnection(getDBPath());
            //db = TestReplaceDB();//TEST
            if (!TableExists(db, "DefaultItems"))
            {
                db.CreateTable(typeof(DefaultItem));
                db.CreateTable(typeof(InventoryItem));
                db.CreateTable(typeof(Order));
                db.CreateTable(typeof(OrderItem));
                db.CreateTable(typeof(Config));
            }
            
            return db;
        }

        private static SQLiteConnection TestReplaceDB()
        {
            var db = new SQLiteConnection(getDBPath());
            db.DropTable<DefaultItem>();
            db.DropTable<InventoryItem>();
            db.DropTable<Order>();
            db.DropTable<OrderItem>();
            db.DropTable<Config>();
            return db;
        }

        private static void WriteDBError(string message)
        {
            Console.WriteLine(message);
        }

        #endregion HelperMethods

        #region DefaultItems
        public static bool CreateDefaultItem(DefaultItem item)
        {
            bool saved = false;
            try
            {
                using (var db = InitializeDatabase())
                {
                    int row = db.Insert(item);
                    if (row > 0)
                    {
                        bool inventoryInsert = CreateInventoryItem(item);
                        if (inventoryInsert)
                        {
                            saved = true;
                        }
                    }
                    else
                    {
                        WriteDBError("Failed inserting DefaultItem: " + item.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDBError(ex.ToString());
            }

            return saved;
        }

        public static bool UpdateDefaultItem(DefaultItem item)
        {
            bool saved = false;
            try
            {
                using (var db = InitializeDatabase())
                {
                    int row = db.Update(item);
                    if (row > 0)
                    {
                        saved = true;
                    }
                    else
                    {
                        WriteDBError("Failed updating DefaultItem: " + item.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDBError(ex.ToString());
            }
            
            return saved;
        }

        public static bool DeleteDefaualtItem(DefaultItem item)
        {
            bool saved = false;
            try
            {
                using (var db = InitializeDatabase())
                {
                    int row = db.Delete(item);
                    if (row > 0)
                    {
                        InventoryItem invItem = db.Get<InventoryItem>(i => i.DefaultItemId == item.Id);
                        db.Delete(invItem);
                        saved = true;
                    }
                    else
                    {
                        WriteDBError("Failed deleting DefaultItem: " + item.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDBError(ex.ToString());
            }

            return saved;
        }

        public static List<DefaultItem> GetDefaultItems()
        {
            List<DefaultItem> items = new List<DefaultItem>();
            try
            {
                using (var db = InitializeDatabase())
                {
                    items = db.GetAllWithChildren<DefaultItem>();
                }
            }
            catch (Exception ex)
            {
                WriteDBError(ex.ToString());
            }

            return items;
        }

        #endregion DefaultItems

        #region InventoryItems
        public static bool CreateInventoryItem(DefaultItem defaultItem)
        {
            bool saved = false;
            try
            {
                using (var db = InitializeDatabase())
                {
                    InventoryItem item = new InventoryItem();
                    item.Count = 0;
                    item.DefaultItemId = defaultItem.Id;

                    int row = db.Insert(item);
                    if (row > 0)
                    {
                        saved = true;
                    }
                    else
                    {
                        WriteDBError("Failed inserting InventoryItem: " + item.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDBError(ex.ToString());
            }
            return saved;
        }

        public static bool UpdateInventoryItemCount(int defaultItemId, int count)
        {
            bool saved = false;
            try
            {
                using (var db = InitializeDatabase())
                {
                    InventoryItem item = db.Find<InventoryItem>(i => i.DefaultItemId == defaultItemId);
                    item.Count = count;

                    int row = db.Update(item);
                    if (row > 0)
                    {
                        saved = true;
                    }
                    else
                    {
                        WriteDBError("Failed updating InventoryItem: " + defaultItemId.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDBError(ex.ToString());
            }

            return saved;
        }

        public static bool IncrementInventoryItemCount(int defaultItemId, int count)
        {
            bool saved = false;
            try
            {
                using (var db = InitializeDatabase())
                {
                    InventoryItem item = db.Find<InventoryItem>(i => i.DefaultItemId == defaultItemId);
                    item.Count += count;

                    int row = db.Update(item);
                    if (row > 0)
                    {
                        saved = true;
                    }
                    else
                    {
                        WriteDBError("Failed updating InventoryItem: " + defaultItemId.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDBError(ex.ToString());
            }

            return saved;
        }

        public static List<InventoryItem> GetInventoryItems()
        {
            List<InventoryItem> items = new List<InventoryItem>();
            try
            {
                using (var db = InitializeDatabase())
                {
                    items = db.GetAllWithChildren<InventoryItem>();
                }
            }
            catch (Exception ex)
            {
                WriteDBError(ex.ToString());
            }

            return items;
        }

        #endregion InventoryItems

        #region Orders
        public static bool CreateOrder(Order item)
        {
            bool saved = false;
            try
            {
                using (var db = InitializeDatabase())
                {
                    List<OrderItem> tempItems = new List<OrderItem>(item.Items);
                    item.Items.Clear();
                    int row = db.Insert(item);
                    if (row > 0)
                    {
                        foreach(OrderItem orderItem in tempItems)
                        {
                            db.Insert(orderItem);
                            int modifier = item.IsClientOrder ? -1 : 1;
                            IncrementInventoryItemCount(orderItem.DefaultItemId, modifier * orderItem.Count);
                        }
                        item.Items = tempItems;
                        db.UpdateWithChildren(item);
                        saved = true;
                    }
                    else
                    {
                        WriteDBError("Failed inserting Order: " + item.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDBError(ex.ToString());
            }

            return saved;
        }

        public static bool UpdateOrder(Order item)
        {
            bool saved = false;
            try
            {
                using (var db = InitializeDatabase())
                {
                    //TODO need to either edit old orderitems or just delete and re-add
                }
            }
            catch (Exception ex)
            {
                WriteDBError(ex.ToString());
            }

            return saved;
        }

        public static bool SetOrderStatus(Order item, bool payed)
        {
            bool saved = false;
            try
            {
                using (var db = InitializeDatabase())
                {
                    item = db.Find<Order>(i => i.Id == item.Id);
                    item.IsPayed = payed;
                    int row = db.Update(item);
                    if (row > 0)
                    {
                        saved = true;
                    }
                    else
                    {
                        WriteDBError("Failed updating Order: " + item.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDBError(ex.ToString());
            }

            return saved;
        }

        public static bool DeleteOrder(Order item)
        {
            bool saved = false;
            try
            {
                using (var db = InitializeDatabase())
                {
                    int row = db.Delete(item);
                    if (row > 0)
                    {
                        foreach (OrderItem orderItem in item.Items)
                        {
                            int modifier = item.IsClientOrder ? 1 : -1;
                            IncrementInventoryItemCount(orderItem.DefaultItemId, modifier * orderItem.Count);
                        }
                        saved = true;
                    }
                    else
                    {
                        WriteDBError("Failed inserting Order: " + item.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDBError(ex.ToString());
            }

            return saved;
        }

        public static List<Order> GetClientOrders(bool isPayed)
        {
            List<Order> items = new List<Order>();
            try
            {
                using (var db = InitializeDatabase())
                {
                    items = db.GetAllWithChildren<Order>(o => o.IsClientOrder && o.IsPayed == isPayed);
                }
            }
            catch (Exception ex)
            {
                WriteDBError(ex.ToString());
            }

            return items;
        }

        public static List<Order> GetInventoryOrders()
        {
            List<Order> items = new List<Order>();
            try
            {
                using (var db = InitializeDatabase())
                {
                    items = db.GetAllWithChildren<Order>(o => !o.IsClientOrder);
                }
            }
            catch (Exception ex)
            {
                WriteDBError(ex.ToString());
            }

            return items;
        }

        public static List<string> GetClientNames()
        {
            List<string> items = new List<string>();
            try
            {
                using (var db = InitializeDatabase())
                {
                    var temp = db.Query<SingleString>("SELECT DISTINCT clientname as value FROM [Order]");
                    foreach(SingleString item in temp)
                    {
                        if (!String.IsNullOrEmpty(item.value))
                        {
                            items.Add(item.value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDBError(ex.ToString());
            }

            return items;
        }

        #endregion Orders

        #region Reports
        //TODO Read for reports
        #endregion Reports

        #region Dashboard
        //TODO Read for dashboard
        #endregion Dashboard

        #region Config

        public static bool SetLanguage(string language)
        {
            bool saved = false;
            try
            {
                using (var db = InitializeDatabase())
                {
                    int row = 0;
                    string sql = "SELECT * FROM [Configs] WHERE name == 'CurrentLanguage'";
                    Config item = db.Query<Config>(sql).FirstOrDefault();
                    if(item == null)
                    {
                        item = new Config();
                        item.Name = "CurrentLanguage";
                        item.Value = language;
                        row = db.Insert(item);
                    }
                    else
                    {
                        item.Value = language;
                        row = db.Update(item);
                    }
                    
                    if (row > 0)
                    {
                        saved = true;
                    }
                    else
                    {
                        WriteDBError("Failed inserting Config: " + item.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDBError(ex.ToString());
            }
            return saved;
        }

        public static string GetLanguage()
        {
            string result = "Tiếng Việt";
            try
            {
                using (var db = InitializeDatabase())
                {
                    var temp = db.Query<SingleString>("SELECT value as value FROM [Configs] WHERE name == 'CurrentLanguage'").FirstOrDefault();
                    if (temp != null && !string.IsNullOrEmpty(temp.value)) 
                    {
                        result = temp.value;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDBError(ex.ToString());
            }

            return result;
        }

        #endregion Config


    }

    internal class SingleString
    {
        public string value { get; set; }
    }
}