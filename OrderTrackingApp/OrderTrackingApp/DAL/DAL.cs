using System;
using System.Collections.Generic;
using SQLite;
using OrderTrackingApp.Objects;
using System.IO;
using SQLiteNetExtensions.Extensions;
using System.Linq;
using OrderTrackingApp.Resx;
using System.Text;
using OrderTrackingApp.DAL.ReportObjects;

namespace OrderTrackingApp.DAL
{
    public static class DAL
    {

        #region HelperMethods
        private static string getDBPath()
        {
            return Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "orderAppDB.db3");
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
                    Order oldItem = db.GetAllWithChildren<Order>(o => o.Id == item.Id).FirstOrDefault();
                    if (oldItem != null && oldItem.Id == item.Id) 
                    {
                        DeleteOrder(oldItem);
                        item.Id = 0;
                        saved = CreateOrder(item);
                    }                    
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

        public static List<Order> GetOrders(DateTime start, DateTime end)
        {
            List<Order> items = new List<Order>();
            try
            {
                using (var db = InitializeDatabase())
                {
                    items = db.GetAllWithChildren<Order>(o => o.OrderDate >= start && o.OrderDate <= end);
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

        private static List<Order> GetFilteredOrders(DateTime start, DateTime end, bool IsClient)
        {
            List<Order> orders = new List<Order>();

            try
            {
                using (var db = InitializeDatabase())
                {
                    orders = db.GetAllWithChildren<Order>(o => o.IsClientOrder == IsClient && o.OrderDate >= start && o.OrderDate <= end);
                }
            }
            catch (Exception ex)
            {
                WriteDBError(ex.ToString());
            }

            return orders;
        }

        public static string CreateInventoryReport(DateTime startDate, DateTime endDate, List<DefaultItem> products)
        {
            return InventoryReportGen.CreateInventoryReport(startDate, endDate, products);
        }

        public static string CreateClientReport(DateTime startDate, DateTime endDate, List<string> clients)
        {
            return ClientReportGen.CreateClientReport(startDate, endDate, clients);
        }

        #endregion Reports

        #region Dashboard
        public static List<string> GetDashboardItems()
        {
            List<string> items = new List<string>();
            items.Add(GetMonthRevenue());
            items.Add(GetMonthOrderCount());
            items.Add(GetOpenOrderCount());
            items.Add(GetOpenMoneyCount());

            return items;
        }

        private static string GetMonthRevenue()
        {
            DateTime start = DateTime.Now - new TimeSpan(31, 0, 0, 0);
            List<Order> orders = GetFilteredOrders(start, DateTime.Now, true);

            decimal money = 0;
            foreach (Order order in orders)
            {
                if (order.IsPayed)
                {
                    foreach (OrderItem item in order.Items)
                    {
                        money += item.Price;
                    }
                }                
            }

            string result =  money.ToString() + AppResources.CurrencySymbol + AppResources.DashboardMonthRevenue;

            return result;
        }

        private static string GetMonthOrderCount()
        {
            DateTime start = DateTime.Now - new TimeSpan(31, 0, 0, 0);
            int count = GetFilteredOrders(start, DateTime.Now, true).Count();
            string result = count.ToString() + AppResources.DashboardMonthOrders;

            return result;
        }

        private static string GetOpenOrderCount()
        {
            int count = GetClientOrders(false).Count();
            string result = count.ToString() + AppResources.DashboardOpenOrders;

            return result;
        }

        private static string GetOpenMoneyCount()
        {
            decimal money = 0;
            List<Order> orders = GetClientOrders(false);

            foreach (Order order in orders)
            {
                foreach(OrderItem item in order.Items)
                {
                     money += item.Price;
                }
            }

            string result = money.ToString() + AppResources.CurrencySymbol + AppResources.DashboardOpenMoney;

            return result;
        }
        #endregion Dashboard

    }

    internal class SingleString
    {
        public string value { get; set; }
    }

    internal class SingleInt
    {
        public int value { get; set; }
    }
}