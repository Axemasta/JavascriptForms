using System;
using System.Windows.Input;
using JavascriptForms.Enums;
using JavascriptForms.Models;
using Xamarin.Forms;

namespace JavascriptForms.Controls
{
    public class HybridWebView : WebView
    {
        public static readonly BindableProperty UriProperty = BindableProperty.Create(nameof(Uri), typeof(string), typeof(HybridWebView), default(string));

        public static readonly BindableProperty SiteSourceProperty = BindableProperty.Create(nameof(SiteSource), typeof(SiteSource), typeof(HybridWebView), default(SiteSource));

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(HybridWebView));

        public string Uri
        {
            get => (string)GetValue(UriProperty);
            set => SetValue(UriProperty, value);
        }

        public SiteSource SiteSource
        {
            get => (SiteSource)GetValue(SiteSourceProperty);
            set => SetValue(SiteSourceProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public void InvokeAction(IBrowserInvocation data)
        {
            if (Command == null)
                return;

            if (!Command.CanExecute(data))
                return;

            Command.Execute(data);
        }
    }
}
