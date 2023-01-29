using OrderTrackingApp.Resx;
using OrderTrackingApp.ViewModels;
using System;
using System.Linq;

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
            inventoryPage.Title = AppResources.InventoryTab;
            Children.Add(inventoryPage);

            NavigationPage historyPage = new NavigationPage(new InventoryHistoryView());
            historyPage.Title = AppResources.HistoryTab;
            Children.Add(historyPage);

            NavigationPage orderPage = new NavigationPage(new InventoryOrderView());
            orderPage.Title = AppResources.OrderTab;
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