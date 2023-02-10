using OrderTrackingApp.DAL.ReportObjects;
using OrderTrackingApp.Objects;
using OrderTrackingApp.Resx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrderTrackingApp.DAL
{
    internal static class ClientReportGen
    {
        public static string CreateClientReport(DateTime startDate, DateTime endDate, List<string> clients)
        {
            List<CRClient> dataPoints = GetClientData(startDate, endDate, clients);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("{0} - ({1} - {2})", AppResources.ClientReport, startDate.ToShortDateString(), endDate.ToShortDateString()));
            sb.AppendLine(string.Format("{0} {1}, {2} {3}",dataPoints.Count - 1, AppResources.Clients2, dataPoints.Where(d => d.Name != AppResources.Total).Select(d => d.OrderCount).Sum(),AppResources.Orders));
            sb.AppendLine("-----------------");
            foreach (CRClient data in dataPoints)
            {
                sb.AppendLine(string.Format("{0}: ", data.Name));
                sb.AppendLine(string.Format("{0}: {1}", AppResources.Orders, data.OrderCount));
                foreach(CRProduct product in data.Products.Values)
                {
                    sb.AppendLine(string.Format("   {0}: {1}{2} ({3})", product.Name, AppResources.CurrencySymbol, product.Spent, product.PurchasedCount));
                }
                sb.AppendLine(string.Format("{0}: {1}{2}", AppResources.Revenue, data.Revenue, AppResources.CurrencySymbol));
                sb.AppendLine(string.Format("{0}: {1}{2}", AppResources.Profit, data.Profit, AppResources.CurrencySymbol));
                sb.AppendLine("-----------------");
            }

            string result = sb.ToString();

            return result;
        }

        private static List<CRClient> GetClientData(DateTime startDate, DateTime endDate, List<string> clients)
        {
            Dictionary<string, CRClient> clientDict = FillInData(startDate, endDate, clients);

            CRClient totals = new CRClient() { Name = AppResources.Total };
            List < CRClient > result = new List<CRClient >();

            foreach(CRClient client in clientDict.Values)
            {
                totals.OrderCount += client.OrderCount;
                totals.Revenue += client.Revenue;
                totals.Profit += client.Profit;
                foreach(CRProduct prod in client.Products.Values)
                {
                    CRProduct totalProd;
                    totals.Products.TryGetValue(prod.Id, out totalProd);
                    if (totalProd == null)
                    {
                        totalProd = new CRProduct();
                        totalProd.Id = prod.Id;
                        totalProd.Name = prod.Name;
                    }
                    totalProd.PurchasedCount += prod.PurchasedCount;
                    totalProd.Spent += prod.Spent;
                    totals.Products[totalProd.Id] = totalProd;
                }
                result.Add(client);
            }
            result.Add(totals);

            return result;
        }

        private static Dictionary<string,CRClient> FillInData(DateTime startDate, DateTime endDate, List<string> clients)
        {
            Dictionary<string, CRClient> clientDict = InitializeClients(clients);
            Dictionary<int, decimal> prices = GetProductPrices();

            List<Order> orders = DAL.GetOrders(startDate, endDate);
            foreach (Order order in orders)
            {
                if(order.IsClientOrder && clientDict.ContainsKey(order.ClientName))
                {
                    CRClient client = clientDict[order.ClientName];
                    client.OrderCount++;

                    foreach (OrderItem item in order.Items)
                    {
                        CRProduct clientProd;
                        client.Products.TryGetValue(item.DefaultItemId, out clientProd);
                        if(clientProd == null)
                        {
                            clientProd = new CRProduct();
                            clientProd.Id = item.DefaultItemId;
                            clientProd.Name = item.ItemName;
                        }                        
                        clientProd.PurchasedCount += item.Count;
                        clientProd.Spent += item.Price;
                        client.Revenue += item.Price;
                        client.Profit += item.Price - (item.Count * prices[item.DefaultItemId]);
                        client.Products[item.DefaultItemId] = clientProd;
                    }
                }
            }

            return clientDict;
        }

        private static Dictionary<string, CRClient> InitializeClients(List<string> clients)
        {
            if(clients.Count== 0)
            {
                clients = DAL.GetClientNames();
            }

            Dictionary<string, CRClient> clientDict = new Dictionary<string, CRClient>();

            foreach(string client in clients)
            {
                clientDict.Add(client, new CRClient() { Name = client});
            }
            return clientDict;
        }

        private static Dictionary<int,decimal> GetProductPrices()
        {
            List<DefaultItem> products = DAL.GetDefaultItems();
            Dictionary<int,decimal> prices = new Dictionary<int,decimal>();
            foreach (DefaultItem item in products)
            {
                prices[item.Id] = item.DefaultBuyPrice;
            }
            return prices;
        }
    }
}
