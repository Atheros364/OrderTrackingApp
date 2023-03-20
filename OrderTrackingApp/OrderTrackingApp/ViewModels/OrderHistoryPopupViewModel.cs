using OrderTrackingApp.Models;
using OrderTrackingApp.Objects;
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
    public class OrderHistoryPopupViewModel : ViewModelBase
    {

        INavigation Nav;
        List<OrderModel> items = new List<OrderModel>();
        OrderModel comboItem;
        List<int> ids = new List<int>();

        public OrderHistoryPopupViewModel(INavigation nav, List<OrderModel> items) 
        {
            this.Nav = nav;
            this.items = items;
            Initialize();
        }

        public OrderModel ComboItem
        {
            get
            {
                return comboItem;
            }
            set
            {
                if (comboItem != value)
                {
                    comboItem = value;
                    OnPropertyChanged(nameof(ComboItem));
                }
            }
        }

        public ICommand PayItemButtonClick { protected set; get; }
        public ICommand CancelButtonClick { protected set; get; }

        private void Initialize()
        {
            PayItemButtonClick = new Command(() =>
            {
                PayItems();
                Nav.PopPopupAsync();
            });
            CancelButtonClick = new Command(() =>
            {
                Nav.PopPopupAsync();
            });
            SetupItem();
        }

        private void SetupItem()
        {
            ids = new List<int>();
            OrderModel model = new OrderModel();
            foreach (OrderModel item in items)
            {
                ids.Add(item.Id);
                model.TotalPrice += item.TotalPrice;

                foreach (OrderItemModel subItem in item.Items)
                {
                    OrderItemModel subModel = model.Items.FirstOrDefault(p => p.Name == subItem.Name);
                    if (subModel == null)
                    {
                        model.Items.Add(subItem);
                    }
                    else
                    {
                        subModel.Amount += subItem.Amount;
                        subModel.Price += subItem.Price;
                    }
                }
            }
            ComboItem = model;
        }

        private void PayItems()
        {
            List<Order> orders = DAL.DAL.GetClientOrders(false);

            foreach (int id in ids)
            {
                Order item = orders.First(p => p.Id == id);
                DAL.DAL.SetOrderStatus(item, true);
            }            
            MessagingCenter.Send(this, "OrderPayed");
            MessagingCenter.Send(this, "ProductChange");
        }
    }
}
