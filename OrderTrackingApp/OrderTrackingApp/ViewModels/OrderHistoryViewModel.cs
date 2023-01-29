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
    public class OrderHistoryViewModel : ViewModelBase
    {
        List<Order> orders;
        ObservableCollection<OrderModel> historyItems = new ObservableCollection<OrderModel>();
        bool isShowingPayed = false;
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

        public ICommand DeleteItemButtonClick { protected set; get; }
        public ICommand PayItemButtonClick { protected set; get; }
        public ICommand ShowOpenButtonClick { protected set; get; }
        public ICommand ShowClosedButtonClick { protected set; get; }

        private void Initialize()
        {
            DeleteItemButtonClick = new Command<int>((id) =>
            {
                Order item = orders.First(p => p.Id == id);
                DAL.DAL.DeleteOrder(item);
                RefreshItems();
            });
            PayItemButtonClick = new Command<int>((id) =>
            {
                Order item = orders.First(p => p.Id == id);
                DAL.DAL.SetOrderStatus(item, true);
                RefreshItems();
            });
            ShowOpenButtonClick = new Command(() =>
            {
                IsShowingPayed= false;
                RefreshItems();
            });
            ShowClosedButtonClick = new Command(() =>
            {
                IsShowingPayed = true;
                RefreshItems();
            });
            MessagingCenter.Subscribe<ClientOrderViewModel>(this, "ProductChange", (sender) =>
            {
                RefreshItems();
            });
        }

        private void RefreshItems()
        {
            HistoryItems.Clear();
            orders = DAL.DAL.GetClientOrders(isShowingPayed);
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
