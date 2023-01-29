using OrderTrackingApp.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace OrderTrackingApp.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        INavigation Nav;
        List<string> languageItems = new List<string>();
        string selectedLanguage = null;
        public SettingsViewModel(INavigation nav)
        {
            Nav = nav;
            InitializeLanguages();
            InitializeButtons();
        }

        public List<string> LanguageItems { get { return languageItems; } }
        public string SelectedLanguage 
        { get 
            { 
                return selectedLanguage; 
            } 
            set 
            { 
                if(selectedLanguage != value) 
                {
                    selectedLanguage = value;
                    OnPropertyChanged(nameof(SelectedLanguage));
                    SetLanguage();
                }                
            } 
        }

        private void InitializeLanguages()
        {
            languageItems.Clear();
            languageItems.Add("English");
            languageItems.Add("Tiếng Việt");
            selectedLanguage= LanguageItems[0];
            SetLanguage();
        }

        private void SetLanguage()
        {
            //TODO change localization file
        }

        public ICommand ProductsButtonClick { protected set; get; }

        private void InitializeButtons()
        {
            ProductsButtonClick = new Command(async () =>
            {
                await Nav.PushAsync(new ProductsView());
            });
        }
    }
}
