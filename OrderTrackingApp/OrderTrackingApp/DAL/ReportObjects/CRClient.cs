using System;
using System.Collections.Generic;
using System.Text;

namespace OrderTrackingApp.DAL.ReportObjects
{
    internal class CRClient
    {
        public string Name { get; set; }
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
        public decimal Profit { get; set; }
        public Dictionary<int, CRProduct> Products { get; set; } = new Dictionary<int, CRProduct>();
    }
}
