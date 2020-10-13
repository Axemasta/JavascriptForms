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
using System.Linq;

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

            string destination = GetNavigationPath();

            INavigationResult result = await NavigationService.NavigateAsync(destination);

            if (!result.Success)
            {
                Debugger.Break();
            }
        }

        private string GetNavigationPath()
        {
            string path = nameof(TabbedPage);

            string[] tabs = new string[]
            {
                $"createTab={nameof(NavigationPage)}|{nameof(LocalPage)}",
                $"createTab={nameof(NavigationPage)}|{nameof(HostedPage)}",
                $"createTab={nameof(NavigationPage)}|{nameof(ExternalPage)}"
            };

            if (tabs.Count() > 0)
                path += "?" + string.Join("&", tabs);

            return path;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<TabbedPage>();
            containerRegistry.RegisterForNavigation<LocalPage, LocalPageViewModel>();
            containerRegistry.RegisterForNavigation<HostedPage, HostedPageViewModel>();
            containerRegistry.RegisterForNavigation<ExternalPage, HostedPageViewModel>();
        }
    }
}
