using System;
using Prism.Mvvm;

namespace JavascriptForms.ViewModels
{
    public class ViewModelBase : BindableBase
    {
        public string Title { get; set; }

        public ViewModelBase()
        {
        }
    }
}
