using OrderTrackingApp.Objects;
using OrderTrackingApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace OrderTrackingApp.ViewModels
{
    public class InventoryViewModel : ViewModelBase
    {
        ObservableCollection<InventoryItem> products = new ObservableCollection<InventoryItem>();
        INavigation Nav;
        public InventoryViewModel(INavigation nav)
        {
            Nav = nav;
            InitializeButtons();
            LoadProducts();
        }
        public ObservableCollection<InventoryItem> Products
        {
            get
            {
                return products;
            }
            set
            {
                if (products != value)
                {
                    products = value;
                    OnPropertyChanged(nameof(Products));
                }
            }
        }
        public ICommand EditProductButtonClick { protected set; get; }

        private void InitializeButtons()
        {
            EditProductButtonClick = new Command<int>(async (id) =>
            {
                InventoryItem item = products.First(p => p.Id == id);
                await Nav.PushModalAsync(new EditInventoryView(item));
            });

            MessagingCenter.Subscribe<AddProductViewModel>(this, "ProductChange", (sender) =>
            {
                LoadProducts();
            });
            MessagingCenter.Subscribe<InventoryOrderViewModel>(this, "ProductChange", (sender) =>
            {
                LoadProducts();
            });
            MessagingCenter.Subscribe<InventoryHistoryViewModel>(this, "ProductChange", (sender) =>
            {
                LoadProducts();
            });
            MessagingCenter.Subscribe<EditInventoryViewModel>(this, "ProductChange", (sender) =>
            {
                LoadProducts();
            });
        }

        private void LoadProducts()
        {
            List<InventoryItem> productsList = DAL.DAL.GetInventoryItems();
            Products.Clear();
            foreach (var product in productsList)
            {
                Products.Add(product);
            }
        }
    }
}
