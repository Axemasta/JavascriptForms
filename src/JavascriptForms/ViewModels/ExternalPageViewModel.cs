using System;
using Prism.Commands;
using Prism.Services;

namespace JavascriptForms.ViewModels
{
    public class ExternalPageViewModel : ViewModelBase
    {
        private readonly IPageDialogService _dialogService;

        public DelegateCommand<string> InvokeKeywordCommand { get; private set; }

        public ExternalPageViewModel(IPageDialogService dialogService)
        {
            _dialogService = dialogService;

            Title = "External Page";

            InvokeKeywordCommand = new DelegateCommand<string>(OnInvokeKeyword);
        }

        public async void OnInvokeKeyword(string name)
        {
            await _dialogService.DisplayAlertAsync($"Keyword Detected", $"You just typed the secret keyword!!! 😲", "OK");
        }
    }
}
