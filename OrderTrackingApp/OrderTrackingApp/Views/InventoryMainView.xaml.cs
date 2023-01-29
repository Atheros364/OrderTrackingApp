using OrderTrackingApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OrderTrackingApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventoryMainView : TabbedPage
    {
        public InventoryMainView()
        {
            NavigationPage inventoryPage = new NavigationPage(new InventoryView());
            inventoryPage.Title = "Inventory";
            Children.Add(inventoryPage);

            NavigationPage historyPage = new NavigationPage(new InventoryHistoryView());
            historyPage.Title = "Order History";
            Children.Add(historyPage);

            NavigationPage orderPage = new NavigationPage(new InventoryOrderView());
            orderPage.Title = "Order";
            Children.Add(orderPage);

            MessagingCenter.Subscribe<InventoryOrderViewModel>(this, "GoToInventory", (sender) =>
            {
                SwitchToFirstPage();
            });
        }

        private void SwitchToFirstPage()
        {
            this.CurrentPage = this.Children.First();
        }
    }
}