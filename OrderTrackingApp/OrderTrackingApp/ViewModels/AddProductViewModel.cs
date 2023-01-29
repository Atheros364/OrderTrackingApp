using OrderTrackingApp.Objects;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace OrderTrackingApp.ViewModels
{
    public class AddProductViewModel : ViewModelBase
    {
        INavigation Nav;
        DefaultItem loadedItem;

        string name = "";
        string description = "";
        decimal buyPrice = 0;
        decimal sellPrice = 0;
        int purchaseSize = 0;


        public AddProductViewModel(INavigation nav, DefaultItem item)
        {
            Nav = nav;
            loadedItem = item;
            InitializeButtons();
            if(item.Id > 0)
            {
                InitializeItem();
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

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                if (description != value)
                {
                    description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public decimal BuyPrice
        {
            get
            {
                return buyPrice;
            }
            set
            {
                if (buyPrice != value)
                {
                    buyPrice = value;
                    OnPropertyChanged(nameof(BuyPrice));
                }
            }
        }

        public decimal SellPrice
        {
            get
            {
                return sellPrice;
            }
            set
            {
                if (sellPrice != value)
                {
                    sellPrice = value;
                    OnPropertyChanged(nameof(SellPrice));
                }
            }
        }

        public int PurchaseSize
        {
            get
            {
                return purchaseSize;
            }
            set
            {
                if (purchaseSize != value)
                {
                    purchaseSize = value;
                    OnPropertyChanged(nameof(PurchaseSize));
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

            loadedItem.Name = Name;
            loadedItem.Description =  Description;
            loadedItem.DefaultBuyPrice = BuyPrice;
            loadedItem.DefaultSellPrice = SellPrice;
            loadedItem.DefaultOrderSize = PurchaseSize;

            if (loadedItem.Id == 0)
            {
                result = DAL.DAL.CreateDefaultItem(loadedItem);
            }
            else
            {
                result = DAL.DAL.UpdateDefaultItem(loadedItem);
            }

            MessagingCenter.Send(this, "ProductChange");

            await Nav.PopModalAsync();
            return result;
        }

        private void InitializeItem()
        {
             Name = loadedItem.Name;
             Description = loadedItem.Description;
             BuyPrice = loadedItem.DefaultBuyPrice;
             SellPrice = loadedItem.DefaultSellPrice;
             PurchaseSize = loadedItem.DefaultOrderSize;
        }
    }
}
