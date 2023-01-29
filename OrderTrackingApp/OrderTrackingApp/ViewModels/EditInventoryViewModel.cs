using OrderTrackingApp.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace OrderTrackingApp.ViewModels
{
    public class EditInventoryViewModel : ViewModelBase
    {
        InventoryItem loadedItem;
        INavigation Nav;

        string name = "";
        int count = 0;

        public EditInventoryViewModel(INavigation nav, InventoryItem item)
        {
            Nav = nav;
            loadedItem = item;
            InitializeButtons();
            InitializeItem();
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

        public ICommand SaveButtonClick { protected set; get; }
        public ICommand CancelButtonClick { protected set; get; }

        private void InitializeButtons()
        {
            SaveButtonClick = new Command(async () =>
            {
                await SaveItem();
            });
            CancelButtonClick = new Command(async () =>
            {
                await Nav.PopModalAsync();
            });
        }

        private async Task<bool> SaveItem()
        {
            bool result = false;

            result = DAL.DAL.UpdateInventoryItemCount(loadedItem.DefaultItemId, Count);

            MessagingCenter.Send(this, "ProductChange");

            await Nav.PopModalAsync();
            return result;
        }

        private void InitializeItem()
        {
            Name = loadedItem.ItemType.Name;
            Count = loadedItem.Count;
        }
    }
}
