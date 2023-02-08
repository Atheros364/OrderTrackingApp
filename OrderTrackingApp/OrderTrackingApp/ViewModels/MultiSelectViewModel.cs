using OrderTrackingApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace OrderTrackingApp.ViewModels
{
    public class MultiSelectViewModel : ViewModelBase
    {
        List<MultiSelectItem> items = new List<MultiSelectItem>();
        INavigation Nav;
        public MultiSelectViewModel(INavigation nav, List<MultiSelectItem> items)
        {
            Nav = nav;
            Items = items;
            Initialize();
        }

        public List<MultiSelectItem> Items
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

        public ICommand SaveButtonClick { protected set; get; }
        public ICommand ItemButtonClick { protected set; get; }

        private void Initialize()
        {
            SaveButtonClick = new Command(() =>
            {
                Save();
            });
            ItemButtonClick = new Command<MultiSelectItem>((item) =>
            {
                item.IsChecked = !item.IsChecked;
            });
        }

        private void Save()
        {
            MultiSelectItems package = new MultiSelectItems();
            package.Items = items.Where(i => i.IsChecked).ToList();
            MessagingCenter.Send(this, "ChooseItems", package);
            Nav.PopAsync();
        }
    }
}
