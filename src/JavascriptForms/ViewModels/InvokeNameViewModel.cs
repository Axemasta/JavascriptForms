using System;
using Prism.Commands;
using Prism.Services;

namespace JavascriptForms.ViewModels
{
    public class InvokeNameViewModel : ViewModelBase
    {
        private readonly IPageDialogService _dialogService;

        public DelegateCommand<string> InvokeNameCommand { get; private set; }

        public InvokeNameViewModel(IPageDialogService dialogService)
        {
            _dialogService = dialogService;

            Title = "Invoke Name";

            InvokeNameCommand = new DelegateCommand<string>(OnInvokeName);
        }

        public async void OnInvokeName(string name)
        {
            await _dialogService.DisplayAlertAsync($"Hello {name}", "If you are reading your name, the javascript invokation was successful!", "OK");
        }
    }
}
