﻿using OrderTrackingApp.ViewModels;
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
    public partial class InventoryHistoryView : ContentPage
    {
        public InventoryHistoryView()
        {
            InitializeComponent();
            BindingContext = new InventoryHistoryViewModel(Navigation);
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}