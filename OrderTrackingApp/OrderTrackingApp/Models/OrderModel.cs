using OrderTrackingApp.Resx;
using OrderTrackingApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace OrderTrackingApp.Models
{
    public class OrderModel : ViewModelBase
    {
        int id;
        string date;
        string name;
        decimal totalPrice;
        ObservableCollection<OrderItemModel> items = new ObservableCollection<OrderItemModel>();
        bool isOpen;
        bool isSelected;

        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public decimal TotalPrice
        {
            get
            {
                return totalPrice;
            }
            set
            {
                if (totalPrice != value)
                {
                    totalPrice = value;
                    OnPropertyChanged(nameof(TotalPrice));
                    OnPropertyChanged(nameof(TotalPriceString));
                }
            }
        }

        public string TotalPriceString
        {
            get
            {
                return TotalPrice.ToString() + AppResources.CurrencySymbol;
            }
        }

        public string Date
        {
            get
            {
                return date;
            }
            set
            {
                if (date != value)
                {
                    date = value;
                    OnPropertyChanged(nameof(Date));
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

        public ObservableCollection<OrderItemModel> Items
        {
            get
            {
                return items;
            }
            set
            {
                if (items != value)
                {
                    items = value;
                    OnPropertyChanged(nameof(Items));
                }
            }
        }

        public bool IsOpen
        {
            get
            {
                return isOpen;
            }
            set
            {
                if (isOpen != value)
                {
                    isOpen = value;
                    OnPropertyChanged(nameof(IsOpen));
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }
    }
}
