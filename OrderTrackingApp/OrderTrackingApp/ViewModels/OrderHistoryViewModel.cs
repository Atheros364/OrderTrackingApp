using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace OrderTrackingApp.ViewModels
{
    public class OrderHistoryViewModel : ViewModelBase
    {
        INavigation Nav;
        public OrderHistoryViewModel(INavigation nav)
        {
            Nav = nav;
        }
    }
}
