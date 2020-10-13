using System;
using JavascriptForms.Enums;
using Xamarin.Forms;

namespace JavascriptForms.Controls
{
    public class HybridWebView : WebView
    {
        Action<string> action;

        public static readonly BindableProperty UriProperty = BindableProperty.Create(
            propertyName: nameof(Uri),
            returnType: typeof(string),
            declaringType: typeof(HybridWebView),
            defaultValue: default(string));

        public static readonly BindableProperty SiteSourceProperty = BindableProperty.Create(
            propertyName: nameof(SiteSource),
            returnType: typeof(SiteSource),
            declaringType: typeof(HybridWebView),
            defaultValue: default(SiteSource));

        public string Uri
        {
            get { return (string)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }

        public SiteSource SiteSource
        {
            get { return (SiteSource)GetValue(SiteSourceProperty); }
            set { SetValue(SiteSourceProperty, value); }
        }

        public void RegisterAction(Action<string> callback)
        {
            action = callback;
        }

        public void Cleanup()
        {
            action = null;
        }

        public void InvokeAction(string data)
        {
            if (action == null || data == null)
            {
                return;
            }
            action.Invoke(data);
        }
    }
}
