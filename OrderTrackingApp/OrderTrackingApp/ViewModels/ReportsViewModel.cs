using OrderTrackingApp.Models;
using OrderTrackingApp.Objects;
using OrderTrackingApp.Resx;
using OrderTrackingApp.Views;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Threading.Tasks;

namespace OrderTrackingApp.ViewModels
{
    public class ReportsViewModel : ViewModelBase
    {
        DateTime startDate;
        DateTime endDate;
        List<string> reportTypes = new List<string>();
        string selectedReportType = "";
        string reportOutput = "";
        bool isProductsEnabled = false;
        bool isClientsEnabled = false;
        string productsText = "";
        string clientsText = "";
        List<DefaultItem> products = new List<DefaultItem>();
        List<string> clients = new List<string>();
        INavigation Nav;
        public ReportsViewModel(INavigation nav)
        {
            Nav = nav;
            Initialize();
        }

        public DateTime StartDate
        {
            get
            {
                return startDate;
            }
            set
            {
                if (startDate != value)
                {
                    startDate = value;
                    OnPropertyChanged(nameof(StartDate));
                }
            }
        }

        public DateTime EndDate
        {
            get
            {
                return endDate;
            }
            set
            {
                if (endDate != value)
                {
                    endDate = value;
                    OnPropertyChanged(nameof(EndDate));
                }
            }
        }

        public bool IsProductsEnabled
        {
            get
            {
                return isProductsEnabled;
            }
            set
            {
                if (isProductsEnabled != value)
                {
                    isProductsEnabled = value;
                    OnPropertyChanged(nameof(IsProductsEnabled));
                }
            }
        }

        public bool IsClientsEnabled
        {
            get
            {
                return isClientsEnabled;
            }
            set
            {
                if (isClientsEnabled != value)
                {
                    isClientsEnabled = value;
                    OnPropertyChanged(nameof(IsClientsEnabled));
                }
            }
        }

        public List<string> ReportTypes
        {
            get
            {
                return reportTypes;
            }
            set
            {
                if (reportTypes != value)
                {
                    reportTypes = value;
                    OnPropertyChanged(nameof(ReportTypes));
                }
            }
        }

        public string SelectedReportType
        {
            get
            {
                return selectedReportType;
            }
            set
            {
                if (selectedReportType != value)
                {
                    selectedReportType = value;
                    OnPropertyChanged(nameof(SelectedReportType));
                    ClientsText = string.Empty;
                    ProductsText = string.Empty;
                    if (SelectedReportType == AppResources.InventoryReport)
                    {
                        IsProductsEnabled = true;
                        IsClientsEnabled = false;
                    }
                    else if (SelectedReportType == AppResources.ClientReport)
                    {
                        IsProductsEnabled = false;
                        IsClientsEnabled = true;
                    }
                }
            }
        }

        public string ReportOutput
        {
            get
            {
                return reportOutput;
            }
            set
            {
                if (reportOutput != value)
                {
                    reportOutput = value;
                    OnPropertyChanged(nameof(ReportOutput));
                }
            }
        }

        public string ProductsText
        {
            get
            {
                return productsText;
            }
            set
            {
                if (productsText != value)
                {
                    productsText = value;
                    OnPropertyChanged(nameof(ProductsText));
                }
            }
        }

        public string ClientsText
        {
            get
            {
                return clientsText;
            }
            set
            {
                if (clientsText != value)
                {
                    clientsText = value;
                    OnPropertyChanged(nameof(ClientsText));
                }
            }
        }

        public ICommand SaveButtonClick { protected set; get; }
        public ICommand RunButtonClick { protected set; get; }
        public ICommand OpenProductsCommand { protected set; get; }
        public ICommand OpenClientsCommand { protected set; get; }

        private void Initialize()
        {
            SaveButtonClick = new Command( async () =>
            {
                await SaveReport();
            });
            RunButtonClick = new Command(() =>
            {
                RunReport();
            });
            OpenProductsCommand = new Command(() =>
            {
                OpenProducts();
            });
            OpenClientsCommand = new Command(() =>
            {
                OpenClients();
            });

            MessagingCenter.Subscribe<MultiSelectViewModel,MultiSelectItems>(this, "ChooseItems", (sender, items) =>
            {
                if(items.Items.Count > 0)
                {
                    if (items.Items[0].InventoryItem != null)
                    {
                        UpdateProducts(items);
                    }
                    else
                    {
                        UpdateClients(items);
                    }
                }
                else
                {
                    ClientsText = string.Empty;
                    clients = new List<string>();
                    ProductsText = string.Empty;
                    products = new List<DefaultItem>();
                }
                
            });

            ReportTypes = new List<string>() { AppResources.InventoryReport, AppResources.ClientReport };
            SelectedReportType = ReportTypes[0];
            StartDate = new DateTime(2023, 1, 1);
            EndDate = DateTime.Now;
            ReportOutput = string.Empty;
        }

        private void UpdateProducts(MultiSelectItems items)
        {
            string text = "";
            products = new List<DefaultItem>();
            foreach(MultiSelectItem item in items.Items)
            {
                text += item.Name + ", ";
                products.Add(item.InventoryItem);
            }
            ProductsText = text;
        }

        private void UpdateClients(MultiSelectItems items)
        {
            string text = "";
            clients = new List<string>();
            foreach (MultiSelectItem item in items.Items)
            {
                text += item.Name + ", ";
                clients.Add(item.Name);
            }
            ClientsText = text;
        }

        private async void OpenProducts()
        {
            List<MultiSelectItem> productItems = new List<MultiSelectItem>();

            List<DefaultItem> defItems = DAL.DAL.GetDefaultItems();
            foreach (var def in defItems)
            {
                MultiSelectItem item = new MultiSelectItem();
                item.Name = def.Name;
                item.InventoryItem = def;
                if (ProductsText.Contains(item.Name + ","))
                {
                    item.IsChecked = true;
                }
                productItems.Add(item);
            }

            await Nav.PushAsync(new MultiSelectView(productItems));
        }

        private async void OpenClients()
        {
            List<MultiSelectItem> productItems = new List<MultiSelectItem>();

            List<string> defItems = DAL.DAL.GetClientNames();
            foreach (var def in defItems)
            {
                MultiSelectItem item = new MultiSelectItem();
                item.Name = def;
                if (clients.Contains(def))
                {
                    item.IsChecked = true;
                }
                productItems.Add(item);
            }

            await Nav.PushAsync(new MultiSelectView(productItems));
        }

        private void RunReport()
        {
            if(SelectedReportType == AppResources.InventoryReport)
            {
                ReportOutput = DAL.DAL.CreateInventoryReport(StartDate, EndDate, products);
            }
            else if(SelectedReportType == AppResources.ClientReport)
            {
                ReportOutput = DAL.DAL.CreateClientReport(StartDate, EndDate, clients);
            }
        }

        private async Task SaveReport()
        {
            if (!String.IsNullOrEmpty(ReportOutput))
            {
                try
                {
                    var message = new EmailMessage
                    {
                        Subject = AppResources.ReportSubject,
                        Body = ReportOutput
                    };
                    await Email.ComposeAsync(message);
                }
                catch (Exception ex) 
                {

                }
            }
        }
    }
}
