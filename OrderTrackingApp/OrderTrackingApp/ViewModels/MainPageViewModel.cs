using OrderTrackingApp.Models;
using OrderTrackingApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace OrderTrackingApp.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        ObservableCollection<string> dashboardTextItems = new ObservableCollection<string>();
        INavigation Nav;
        public MainPageViewModel(INavigation navigation) 
        {
            Nav = navigation;
            UpdateDashboard();
            InitializeButtons();
        }

        public ObservableCollection<string> DashboardTextItems
        {
            get
            {
                return dashboardTextItems;
            }
            set
            {
                if (dashboardTextItems != value)
                {
                    dashboardTextItems = value;
                    OnPropertyChanged(nameof(DashboardTextItems));
                }
            }
        }

        public ICommand InventoryButtonClick { protected set; get; }
        public ICommand NewOrderButtonClick { protected set; get; }
        public ICommand OrderHistoryButtonClick { protected set; get; }
        public ICommand ReportsButtonClick { protected set; get; }
        public ICommand ProductsButtonClick { protected set; get; }

        private void InitializeButtons()
        {
            InventoryButtonClick = new Command(async () =>
            {
                await Nav.PushAsync(new InventoryMainView());
            });
            NewOrderButtonClick = new Command(async () =>
            {
                await Nav.PushAsync(new ClientOrderView());
            });
            OrderHistoryButtonClick = new Command(async () =>
            {
                await Nav.PushAsync(new OrderHistoryView());
            });
            ReportsButtonClick = new Command(async () =>
            {
                await Nav.PushAsync(new ReportsView());
            });
            ProductsButtonClick = new Command(async () =>
            {
                await Nav.PushAsync(new ProductsView());
            });
            MessagingCenter.Subscribe<ClientOrderViewModel>(this, "ProductChange", (sender) =>
            {
                UpdateDashboard();
            });
            MessagingCenter.Subscribe<OrderHistoryViewModel>(this, "OrderPayed", (sender) =>
            {
                UpdateDashboard();
            });
        }

        private void UpdateDashboard()
        {
            DashboardTextItems = new ObservableCollection<string>(DAL.DAL.GetDashboardItems());
        }
    }
}
