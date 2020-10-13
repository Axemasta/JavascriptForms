using Prism.Commands;
using Prism.Services;

namespace JavascriptForms.ViewModels
{
    public class DisplayNameViewModel : ViewModelBase
    {
        private readonly IPageDialogService _dialogService;

        public DelegateCommand<string> InvokeNameCommand { get; private set; }

        public DisplayNameViewModel(IPageDialogService dialogService)
        {
            _dialogService = dialogService;

            InvokeNameCommand = new DelegateCommand<string>(OnInvokeName);
        }

        public async void OnInvokeName(string name)
        {
            await _dialogService.DisplayAlertAsync($"Hello {name}", $"If you are reading your name ({name}), the javascript invokation was successful!", "OK");
        }
    }
}
