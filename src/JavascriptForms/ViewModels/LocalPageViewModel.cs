using Prism.Commands;
using Prism.Services;

namespace JavascriptForms.ViewModels
{
    public class LocalPageViewModel : ViewModelBase
    {
        private readonly IPageDialogService _dialogService;

        public DelegateCommand<string> InvokeNameCommand { get; private set; }

        public LocalPageViewModel(IPageDialogService dialogService)
        {
            _dialogService = dialogService;

            Title = "Local Page";

            InvokeNameCommand = new DelegateCommand<string>(OnInvokeName);
        }

        public async void OnInvokeName(string name)
        {
            await _dialogService.DisplayAlertAsync($"Hello {name}", $"If you are reading your name ({name}), the javascript invokation from local file was successful!", "OK");
        }
    }
}
