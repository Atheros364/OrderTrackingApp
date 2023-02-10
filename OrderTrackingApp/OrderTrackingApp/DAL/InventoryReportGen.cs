using OrderTrackingApp.DAL.ReportObjects;
using OrderTrackingApp.Objects;
using OrderTrackingApp.Resx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace OrderTrackingApp.DAL
{
    internal static class InventoryReportGen
    {
        public static string CreateInventoryReport(DateTime startDate, DateTime endDate, List<DefaultItem> products)
        {
            List<IRProduct> dataPoints = GetInventoryReportData(startDate, endDate, products);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("{0} - ({1} - {2})", AppResources.InventoryReport, startDate.ToShortDateString(), endDate.ToShortDateString()));
            sb.AppendLine("-----------------");
            foreach (IRProduct data in dataPoints)
            {
                sb.AppendLine(string.Format("{0}: ", data.Name));
                sb.AppendLine(string.Format("{0}: {1}", AppResources.InInventory, data.InventoryCount));
                sb.AppendLine(string.Format("{0}: {1}", AppResources.Purchased, data.PurchasedCount));
                sb.AppendLine(string.Format("{0}: {1}", AppResources.Sold, data.SoldCount));
                sb.AppendLine(string.Format("{0}: {1}{2}", AppResources.Revenue, data.Revenue, AppResources.CurrencySymbol));
                sb.AppendLine(string.Format("{0}: {1}{2}", AppResources.Profit, data.Profit, AppResources.CurrencySymbol));
                sb.AppendLine("-----------------");
            }

            string result = sb.ToString();

            return result;
        }

        private static Dictionary<int, IRProduct> InitializeProducts(List<DefaultItem> products)
        {
            List<InventoryItem> items = DAL.GetInventoryItems();

            if (products.Count == 0)
            {
                products = DAL.GetDefaultItems();
            }

            Dictionary<int, IRProduct> productDict = new Dictionary<int, IRProduct>();
            foreach (DefaultItem product in products)
            {
                int inventoryCount = items.Where(i => i.DefaultItemId == product.Id).Select(i => i.Count).Sum();
                productDict.Add(product.Id, new IRProduct() { Name = product.Name, InventoryCount = inventoryCount });
            }
            return productDict;
        }

        private static Dictionary<int, IRProduct> FillInData(DateTime startDate, DateTime endDate, List<DefaultItem> products)
        {
            Dictionary<int, IRProduct> productDict = InitializeProducts(products);
            List<Order> orders = DAL.GetOrders(startDate, endDate);
            foreach (Order order in orders)
            {
                foreach (OrderItem item in order.Items)
                {
                    if (productDict.ContainsKey(item.DefaultItemId))
                    {
                        IRProduct iRProduct = productDict[item.DefaultItemId];
                        if (order.IsClientOrder)
                        {
                            iRProduct.Revenue += item.Price;
                            iRProduct.Profit += item.Price;
                            iRProduct.SoldCount += item.Count;
                        }
                        else
                        {
                            iRProduct.Profit -= item.Price;
                            iRProduct.PurchasedCount += item.Count;
                        }
                    }
                }
            }
            return productDict;
        }

        private static List<IRProduct> GetInventoryReportData(DateTime startDate, DateTime endDate, List<DefaultItem> products)
        {
            Dictionary<int, IRProduct> productDict = FillInData(startDate, endDate, products);

            IRProduct totals = new IRProduct() { Name = AppResources.Total };
            List<IRProduct> result = new List<IRProduct>();

            foreach (IRProduct data in productDict.Values)
            {
                data.UnitProfit = data.SoldCount > 0 ? data.Profit / data.SoldCount : 0;
                totals.Profit += data.Profit;
                totals.Revenue += data.Revenue;
                totals.SoldCount += data.SoldCount;
                totals.PurchasedCount += data.PurchasedCount;
                totals.InventoryCount += data.InventoryCount;
                result.Add(data);
            }
            totals.UnitProfit = totals.SoldCount > 0 ? totals.Profit / totals.SoldCount : 0;
            result.Add(totals);

            return result;
        }
    }
}
