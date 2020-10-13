using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace JavascriptForms.Pages
{
    public partial class ExternalPage : ContentPage
    {
        public ExternalPage()
        {
            InitializeComponent();

            this.BindingContextChanged += InvokeNamePage_BindingContextChanged;
        }

        private void InvokeNamePage_BindingContextChanged(object sender, EventArgs e)
        {
            var vm = ((ViewModels.ExternalPageViewModel)this.BindingContext);

            if (vm == null)
                return;

            HybridWebView.RegisterAction(data => vm.InvokeKeywordCommand.Execute(data));
        }
    }
}
