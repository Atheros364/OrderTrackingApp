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
    public class InventoryHistoryViewModel : ViewModelBase
    {
        INavigation Nav;
        List<Order> orders;
        ObservableCollection<OrderModel> historyItems = new ObservableCollection<OrderModel>();

        public InventoryHistoryViewModel(INavigation nav)
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

        public ICommand DeleteItemButtonClick { protected set; get; }

        private void Initialize()
        {
            DeleteItemButtonClick = new Command<int>((id) =>
            {
                Order item = orders.First(p => p.Id == id);
                DAL.DAL.DeleteOrder(item);
                RefreshItems();
            });
            MessagingCenter.Subscribe<InventoryOrderViewModel>(this, "ProductChange", (sender) =>
            {
                RefreshItems();
            });
        }

        private void RefreshItems()
        {
            HistoryItems.Clear();
            orders = DAL.DAL.GetInventoryOrders();
            orders.Reverse();
            foreach (Order item in orders)
            {
                OrderModel model = new OrderModel();
                model.Id = item.Id;
                model.Date = item.OrderDate.ToShortDateString();
                model.Items = new ObservableCollection<OrderItemModel>();
                decimal price = 0;

                foreach(OrderItem subitem in item.Items)
                {
                    OrderItemModel orderItemModel= new OrderItemModel();
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
