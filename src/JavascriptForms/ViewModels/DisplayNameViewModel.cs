using System.Diagnostics;
using JavascriptForms.Events;
using JavascriptForms.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Services;

namespace JavascriptForms.ViewModels
{
    public class DisplayNameViewModel : ViewModelBase
    {
        private readonly IPageDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;

        public DelegateCommand<IBrowserInvocation> InvokeNameCommand { get; private set; }

        public DisplayNameViewModel(IPageDialogService dialogService, IEventAggregator eventAggregator)
        {
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;

            InvokeNameCommand = new DelegateCommand<IBrowserInvocation>(OnInvokeName);
        }

        public async void OnInvokeName(IBrowserInvocation data)
        {
            _eventAggregator.GetEvent<BrowserInvokedEvent>().Publish(data);

            await _dialogService.DisplayAlertAsync($"Hello {data.Data}", $"If you are reading your name ({data.Data}), the javascript invokation was successful!", "OK");

            Debug.WriteLine($"Browser detected keyword: {data.Data} found at url: {data.BrowserUrl}");
        }
    }
}
