using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using JavascriptForms.Pages;
using Prism.DryIoc;
using Prism.Ioc;
using System.Diagnostics;
using Prism.Navigation;
using Prism;
using JavascriptForms.ViewModels;

namespace JavascriptForms
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {

        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            string destination = $"NavigationPage/{nameof(Pages.InvokeNamePage)}";

            INavigationResult result = await NavigationService.NavigateAsync(destination);

            if (!result.Success)
            {
                Debugger.Break();
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<InvokeNamePage, InvokeNameViewModel>();
        }
    }
}
