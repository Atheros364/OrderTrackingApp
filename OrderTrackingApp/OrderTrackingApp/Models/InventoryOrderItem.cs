using OrderTrackingApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderTrackingApp.Models
{
    public class InventoryOrderItem : ViewModelBase
    {
        decimal price { get; set; }
        string name { get; set; }
        int count { get; set; }

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

        public int Count
        {
            get
            {
                return count;
            }
            set
            {
                if (count != value)
                {
                    count = value;
                    OnPropertyChanged(nameof(Count));
                }
            }
        }
    }
}
