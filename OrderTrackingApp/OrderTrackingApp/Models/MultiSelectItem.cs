using OrderTrackingApp.Objects;
using OrderTrackingApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderTrackingApp.Models
{
    public class MultiSelectItem : ViewModelBase
    {
        string name;
        bool isChecked = false;

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

        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                if (isChecked != value)
                {
                    isChecked = value;
                    OnPropertyChanged(nameof(IsChecked));
                }
            }
        }

        public DefaultItem InventoryItem { get; set; }
    }
}
