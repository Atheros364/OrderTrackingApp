using OrderTrackingApp.Objects;
using OrderTrackingApp.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using OrderTrackingApp.DAL;
using System.Linq;
using System.Collections.ObjectModel;
using OrderTrackingApp.Resx;

namespace OrderTrackingApp.ViewModels
{
    public class ProductsViewModel : ViewModelBase
    {
        ObservableCollection<DefaultItem> products = new ObservableCollection<DefaultItem>();
        INavigation Nav;
        public ProductsViewModel(INavigation nav)
        {
            Nav = nav;
            InitializeButtons();
            LoadProducts();
        }

        public ObservableCollection<DefaultItem> Products
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

        public ICommand AddProductButtonClick { protected set; get; }
        public ICommand EditProductButtonClick { protected set; get; }
        public ICommand DeleteProductButtonClick { protected set; get; }

        private void InitializeButtons()
        {
            AddProductButtonClick = new Command(async () =>
            {

                await Nav.PushModalAsync(new AddProductView(new DefaultItem()));
            });
            EditProductButtonClick = new Command<int>(async (id) =>
            {
                DefaultItem item = products.First(p => p.Id == id);
                await Nav.PushModalAsync(new AddProductView(item));
            });
            DeleteProductButtonClick = new Command<int>(async (id) =>
            {
                var confirmed = await Application.Current.MainPage.DisplayAlert(AppResources.ConfirmTtile, AppResources.DeleteConfirmationMsg, AppResources.Yes, AppResources.No);
                if (confirmed)
                {
                    DefaultItem item = products.First(p => p.Id == id);
                    DAL.DAL.DeleteDefaualtItem(item);
                    LoadProducts();
                }
            });

            MessagingCenter.Subscribe<AddProductViewModel>(this, "ProductChange", (sender) =>
            {
                LoadProducts();
            });
        }

        private void LoadProducts()
        {
            List<DefaultItem> productsList = DAL.DAL.GetDefaultItems();
            Products.Clear();
            foreach(var product in productsList)
            {
                Products.Add(product);
            }
        }
    }
}
