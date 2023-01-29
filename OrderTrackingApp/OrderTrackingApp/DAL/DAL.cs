using System;
using System.Collections.Generic;
using SQLite;
using OrderTrackingApp.Objects;
using System.IO;
using SQLiteNetExtensions.Extensions;

namespace OrderTrackingApp.DAL
{
    public static class DAL
    {

        #region HelperMethods
        private static string getDBPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),"orderAppDB.db3");
        }

        public static bool TableExists<T>(SQLiteConnection connection)
        {
            const string cmdText = "SELECT name FROM sqlite_master WHERE type='table' AND name=?";
            var cmd = connection.CreateCommand(cmdText, typeof(T).Name);
            return cmd.ExecuteScalar<string>() != null;
        }

        private static SQLiteConnection InitializeDatabase()
        {
            var db = new SQLiteConnection(getDBPath());
            if (!TableExists<DefaultItem>(db))
            {
                db.CreateTable(typeof(DefaultItem));
                db.CreateTable(typeof(InventoryItem));
                db.CreateTable(typeof(Order));
                db.CreateTable(typeof(OrderItem));
            }
            //db = TestReplaceDB();//TEST
            return db;
        }

        private static SQLiteConnection TestReplaceDB()
        {
            var db = new SQLiteConnection(getDBPath());
            db.DropTable<DefaultItem>();
            db.DropTable<InventoryItem>();
            db.DropTable<Order>();
            db.DropTable<OrderItem>();
            db.CreateTable(typeof(DefaultItem));
            db.CreateTable(typeof(InventoryItem));
            db.CreateTable(typeof(Order));
            db.CreateTable(typeof(OrderItem));
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

        #endregion Orders

        //TODO Read for reports
        //TODO Read for dashboard
    }
}