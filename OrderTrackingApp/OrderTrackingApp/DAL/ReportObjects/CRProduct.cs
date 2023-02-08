using System;
using System.Collections.Generic;
using System.Text;

namespace OrderTrackingApp.DAL.ReportObjects
{
    internal class CRProduct
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int PurchasedCount { get; set; }
        public decimal Spent { get; set; }
    }
}
