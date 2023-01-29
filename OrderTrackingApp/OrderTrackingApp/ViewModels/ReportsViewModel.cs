using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace OrderTrackingApp.ViewModels
{
    public class ReportsViewModel : ViewModelBase
    {
        INavigation Nav;
        public ReportsViewModel(INavigation nav)
        {
            Nav = nav;
        }
    }
}
