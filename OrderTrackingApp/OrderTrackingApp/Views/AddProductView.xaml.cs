using OrderTrackingApp.Objects;
using OrderTrackingApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OrderTrackingApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddProductView : ContentPage
    {
        public AddProductView(DefaultItem item)
        {
            InitializeComponent();
            BindingContext = new AddProductViewModel(Navigation, item);
        }
    }
}