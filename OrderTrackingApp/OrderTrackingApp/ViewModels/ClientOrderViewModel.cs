using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace OrderTrackingApp.ViewModels
{
    public class ClientOrderViewModel : ViewModelBase
    {
        INavigation Nav;
        public ClientOrderViewModel(INavigation nav)
        {
            Nav = nav;
        }
    }
}
