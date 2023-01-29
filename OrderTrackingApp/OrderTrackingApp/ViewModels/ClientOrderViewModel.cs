using OrderTrackingApp.Models;
using OrderTrackingApp.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace OrderTrackingApp.ViewModels
{
    public class ClientOrderViewModel : ViewModelBase
    {
        List<DefaultItem> itemTypeObjects = new List<DefaultItem>();
        List<string> itemTypes = new List<string>();
        string selectedItemType;
        decimal price = 0;
        int amount = 0;
        ObservableCollection<InventoryOrderItem> addedItems = new ObservableCollection<InventoryOrderItem>();
        decimal totalCost = 0;
        string name = string.Empty;
        INavigation Nav;
        public ClientOrderViewModel(INavigation nav)
        {
            Nav = nav;
            Initialize();
            ResetPage();
        }

        #region Bindings

        public List<string> ItemTypes
        {
            get
            {
                return itemTypes;
            }
            set
            {
                if (itemTypes != value)
                {
                    itemTypes = value;
                    OnPropertyChanged(nameof(ItemTypes));
                }
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string SelectedItemType
        {
            get
            {
                return selectedItemType;
            }
            set
            {
                if (selectedItemType != value)
                {
                    selectedItemType = value;
                    OnPropertyChanged(nameof(SelectedItemType));
                    SetDefaults();
                }
            }
        }

        public decimal Price
        {
            get
            {
                return price;
            }
            set
            {
                if (price != value)
                {
                    price = value;
                    OnPropertyChanged(nameof(Price));
                }
            }
        }

        public int Amount
        {
            get
            {
                return amount;
            }
            set
            {
                if (amount != value)
                {
                    amount = value;
                    OnPropertyChanged(nameof(Amount));
                }
            }
        }

        public decimal TotalCost
        {
            get
            {
                return totalCost;
            }
            set
            {
                if (totalCost != value)
                {
                    totalCost = value;
                    OnPropertyChanged(nameof(TotalCost));
                }
            }
        }

        public ObservableCollection<InventoryOrderItem> AddedItems
        {
            get
            {
                return addedItems;
            }
            set
            {
                if (addedItems != value)
                {
                    addedItems = value;
                    OnPropertyChanged(nameof(AddedItems));
                }
            }
        }

        public ICommand SaveButtonClick { protected set; get; }
        public ICommand CancelButtonClick { protected set; get; }
        public ICommand AddItemButtonClick { protected set; get; }
        public ICommand DeleteItemButtonClick { protected set; get; }

        #endregion Bindings

        private void Initialize()
        {
            AddItemButtonClick = new Command(() =>
            {
                ProcessItem();
            });
            DeleteItemButtonClick = new Command<string>((name) =>
            {
                InventoryOrderItem item = addedItems.First(p => p.Name == name);
                AddedItems.Remove(item);
            });
            SaveButtonClick = new Command(() =>
            {
                SaveOrder();
                ResetPage();
                MessagingCenter.Send(this, "ProductChange");
                Nav.PopAsync();
            });
            CancelButtonClick = new Command(() =>
            {
                ResetPage();
            });
        }

        private void ResetPage()
        {
            AddedItems.Clear();
            TotalCost = 0;
            Name = string.Empty;
            itemTypeObjects = DAL.DAL.GetDefaultItems();
            List<string> tempTypes = new List<string>();
            foreach (var item in itemTypeObjects)
            {
                tempTypes.Add(item.Name);
            }
            SelectedItemType = null;
            ItemTypes = tempTypes;
            if (itemTypeObjects.Count > 0)
            {
                SelectedItemType = ItemTypes[0];
            }
        }

        private void SetDefaults()
        {
            if (SelectedItemType != null)
            {
                DefaultItem selected = itemTypeObjects.First(p => p.Name == SelectedItemType);
                Price = selected.DefaultBuyPrice;
                Amount = selected.DefaultOrderSize;
            }
        }

        private void ProcessItem()
        {
            InventoryOrderItem item = addedItems.FirstOrDefault(p => p.Name == SelectedItemType);
            if (item == null)
            {
                item = new InventoryOrderItem();
                DefaultItem defaultItem = itemTypeObjects.First(p => p.Name == SelectedItemType);
                item.Name = defaultItem.Name;
                item.Count = Amount;
                item.Price = Amount * Price;
                addedItems.Add(item);
            }
            else
            {
                item.Count = Amount;
                item.Price = Amount * Price;
            }
            SetPrice();
        }

        private void SetPrice()
        {
            decimal price = 0;
            foreach (InventoryOrderItem item in addedItems)
            {
                price += item.Price;
            }
            TotalCost = price;
        }

        private void SaveOrder()
        {
            Order order = new Order();
            order.OrderDate = DateTime.Now;
            order.IsClientOrder = true;
            order.ClientName = Name;
            order.Items = new List<OrderItem>();
            foreach (InventoryOrderItem item in addedItems)
            {
                DefaultItem defaultItem = itemTypeObjects.First(p => p.Name == item.Name);
                OrderItem orderItem = new OrderItem();
                orderItem.DefaultItemId = defaultItem.Id;
                orderItem.ItemName = defaultItem.Name;
                orderItem.Count = item.Count;
                orderItem.Price = item.Price;
                order.Items.Add(orderItem);
            }

            DAL.DAL.CreateOrder(order);
        }
    }
}
