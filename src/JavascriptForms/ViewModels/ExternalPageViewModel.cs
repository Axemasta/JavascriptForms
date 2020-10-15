using System;
using System.Diagnostics;
using JavascriptForms.Events;
using JavascriptForms.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Services;

namespace JavascriptForms.ViewModels
{
    public class ExternalPageViewModel : ViewModelBase
    {
        private readonly IPageDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;

        public DelegateCommand<IBrowserInvocation> InvokeKeywordCommand { get; private set; }

        public ExternalPageViewModel(IPageDialogService dialogService, IEventAggregator eventAggregator)
        {
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;

            Title = "External Page";

            InvokeKeywordCommand = new DelegateCommand<IBrowserInvocation>(OnInvokeKeyword);
        }

        public async void OnInvokeKeyword(IBrowserInvocation data)
        {
            _eventAggregator.GetEvent<BrowserInvokedEvent>().Publish(data);

            await _dialogService.DisplayAlertAsync($"Keyword Detected", $"You just typed the secret keyword!!! 😲", "OK");

            Debug.WriteLine($"Browser detected keyword: {data.Data} found at url: {data.BrowserUrl}");
        }
    }
}
