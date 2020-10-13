using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace JavascriptForms.Pages
{
    public partial class InvokeNamePage : ContentPage
    {
        public InvokeNamePage()
        {
            InitializeComponent();

            this.BindingContextChanged += InvokeNamePage_BindingContextChanged;            
        }

        private void InvokeNamePage_BindingContextChanged(object sender, EventArgs e)
        {
            var vm = ((ViewModels.InvokeNameViewModel)this.BindingContext);

            if (vm == null)
                return;

            HybridWebView.RegisterAction(data => vm.InvokeNameCommand.Execute(data));
        }
    }
}
