using OrderTrackingApp.Resx;
using OrderTrackingApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderTrackingApp.Models
{
    public class OrderItemModel : ViewModelBase
    {
        decimal price;
        int amount;
        string name;

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
                    OnPropertyChanged(nameof(PriceString));
                }
            }
        }

        public string PriceString
        {
            get
            {
                return Price.ToString() + AppResources.CurrencySymbol;
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
    }
}
