using System;
using JavascriptForms.Events;
using JavascriptForms.Models;
using Newtonsoft.Json;
using Prism.Events;

namespace JavascriptForms.ViewModels
{
    public class BrowserDetailsViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;

        private string _browserJson;
        public string BrowserJson
        {
            get => _browserJson;
            set => SetProperty(ref _browserJson, value);
        }

        public BrowserDetailsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            Title = "Browser Details";

            BrowserJson = JsonConvert.SerializeObject(
                new BrowserInvocation()
                {
                    BrowserInfo = new BrowserInfo(),
                    BrowserUrl = string.Empty,
                    Data = string.Empty,
                    DisplayDimensions = new Coordinates(),
                    ElementCoordinates = new Coordinates()
                },
                Formatting.Indented
            );

            _eventAggregator.GetEvent<BrowserInvokedEvent>().Subscribe(OnBrowserInfo);
        }

        private void OnBrowserInfo(IBrowserInvocation invocation)
        {
            BrowserJson = JsonConvert.SerializeObject(invocation, Formatting.Indented);
        }
    }
}
