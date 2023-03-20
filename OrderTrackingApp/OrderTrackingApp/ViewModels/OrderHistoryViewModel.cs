using OrderTrackingApp.Models;
using OrderTrackingApp.Objects;
using OrderTrackingApp.Resx;
using OrderTrackingApp.Views;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace OrderTrackingApp.ViewModels
{
    public class OrderHistoryViewModel : ViewModelBase
    {
        List<Order> orders;
        ObservableCollection<OrderModel> historyItems = new ObservableCollection<OrderModel>();
        bool isShowingPayed = false;
        bool isInMultiSelect = false;
        string searchText = string.Empty;
        INavigation Nav;
        public OrderHistoryViewModel(INavigation nav)
        {
            Nav = nav;
            Initialize();
            RefreshItems();
        }
        public ObservableCollection<OrderModel> HistoryItems
        {
            get
            {
                return historyItems;
            }
            set
            {
                if (historyItems != value)
                {
                    historyItems = value;
                    OnPropertyChanged(nameof(HistoryItems));
                }
            }
        }

        public bool IsShowingPayed
        {
            get
            {
                return isShowingPayed;
            }
            set
            {
                if (isShowingPayed != value)
                {
                    isShowingPayed = value;
                    OnPropertyChanged(nameof(IsShowingPayed));
                }
            }
        }

        public bool IsInMultiSelect
        {
            get
            {
                return isInMultiSelect;
            }
            set
            {
                if (isInMultiSelect != value)
                {
                    isInMultiSelect = value;
                    OnPropertyChanged(nameof(IsInMultiSelect));
                }
            }
        }

        public string SearchText
        {
            get
            {
                return searchText;
            }
            set
            {
                if (searchText != value)
                {
                    searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    RefreshItems();
                }
            }
        }

        public ICommand DeleteItemButtonClick { protected set; get; }
        public ICommand PayItemButtonClick { protected set; get; }
        public ICommand ShowOpenButtonClick { protected set; get; }
        public ICommand ShowClosedButtonClick { protected set; get; }
        public ICommand SearchButtonClick { protected set; get; }
        public ICommand ClearSearchButtonClick { protected set; get; }
        public ICommand CancelMultiSelectButtonClick { protected set; get; }
        public ICommand ShowSelectedScreenButtonClick { protected set; get; }
        public ICommand EnableMultiSelect { protected set; get; }

        private void Initialize()
        {
            DeleteItemButtonClick = new Command<int>(async (id) =>
            {
                var confirmed = await Application.Current.MainPage.DisplayAlert(AppResources.ConfirmTtile, AppResources.DeleteConfirmationMsg, AppResources.Yes, AppResources.No);
                if (confirmed)
                {
                    Order item = orders.First(p => p.Id == id);
                    DAL.DAL.DeleteOrder(item);
                    RefreshItems();
                }
            });
            PayItemButtonClick = new Command<int>((id) =>
            {
                Order item = orders.First(p => p.Id == id);
                DAL.DAL.SetOrderStatus(item, true);
                RefreshItems();
                MessagingCenter.Send(this, "OrderPayed");
            });
            ShowOpenButtonClick = new Command(() =>
            {
                IsShowingPayed= false;
                RefreshItems();
            });
            ShowClosedButtonClick = new Command(() =>
            {
                CancelMultiSelect();
                IsShowingPayed = true;
                RefreshItems();
            });
            SearchButtonClick = new Command(() =>
            {
                RefreshItems();
            });
            ClearSearchButtonClick = new Command(() =>
            {
                SearchText = string.Empty;
            });
            CancelMultiSelectButtonClick = new Command(() =>
            {
                CancelMultiSelect();
            });
            ShowSelectedScreenButtonClick = new Command(() =>
            {
                ShowPopup();
            });
            EnableMultiSelect = new Command(() =>
            {
                if(!IsShowingPayed)
                {
                    IsInMultiSelect = true;
                }                
            });
            MessagingCenter.Subscribe<ClientOrderViewModel>(this, "ProductChange", (sender) =>
            {
                RefreshItems();
            });
            MessagingCenter.Subscribe<OrderHistoryPopupViewModel>(this, "OrderPayed", (sender) =>
            {
                RefreshItems();
            });
        }

        private void ShowPopup()
        {
            List<OrderModel> items = new List<OrderModel>();
            foreach(OrderModel item in HistoryItems)
            {
                if (item.IsSelected)
                {
                    items.Add(item);
                }
            }

            CancelMultiSelect();
            Nav.PushPopupAsync(new OrderHistoryPopupView(items));
        }

        private void CancelMultiSelect()
        {
            IsInMultiSelect = false;
            foreach (OrderModel item in HistoryItems)
            {
                item.IsSelected = false;
            }
        }

        private void RefreshItems()
        {
            HistoryItems.Clear();
            orders = DAL.DAL.GetClientOrders(isShowingPayed);

            if (!String.IsNullOrEmpty(SearchText))
            {
                orders = orders.Where(o => o.ClientName.ToLower().Contains(SearchText.ToLower())).ToList();
            }

            orders.Reverse();
            foreach (Order item in orders)
            {
                OrderModel model = new OrderModel();
                model.Id = item.Id;
                model.Date = item.OrderDate.ToShortDateString();
                model.Name = item.ClientName;
                model.Items = new ObservableCollection<OrderItemModel>();
                model.IsOpen = !item.IsPayed;
                decimal price = 0;

                foreach (OrderItem subitem in item.Items)
                {
                    OrderItemModel orderItemModel = new OrderItemModel();
                    orderItemModel.Amount = subitem.Count;
                    orderItemModel.Price = subitem.Price;
                    orderItemModel.Name = subitem.ItemName;
                    price += subitem.Price;
                    model.Items.Add(orderItemModel);
                }
                model.TotalPrice = price;

                HistoryItems.Add(model);
            }
        }
    }
}
