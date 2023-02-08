using System;
using System.Collections.Generic;
using System.Text;

namespace OrderTrackingApp.DAL.ReportObjects
{
    internal class IRProduct
    {
        public string Name { get; set; }
        public int InventoryCount { get; set; }
        public int PurchasedCount { get; set; }
        public int SoldCount { get; set; }
        public decimal Revenue { get; set; }
        public decimal Profit { get; set; }
        public decimal UnitProfit { get; set; }
    }
}
