using System;
using System.Reflection;
using Android.Content;
using JavascriptForms.Controls;
using JavascriptForms.Droid.Renderers;
using JavascriptForms.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace JavascriptForms.Droid.Renderers
{
    public class test : WebViewRenderer
    {
        public test(Context context) : base(context)
        {

        }
    }

    public class HybridWebViewRenderer : WebViewRenderer
    {
        const string _nativeInvoker = "JavascriptForms.Droid.Scripts.AndroidInvoker.js";

        public HybridWebViewRenderer(Context context) : base(context)
        {
        }

        private string LoadScript(string resourceName)
        {
            bool isNativeResource = resourceName == _nativeInvoker;

            var assembly = isNativeResource ? Assembly.GetExecutingAssembly() : typeof(JavascriptForms.App).Assembly;

            if (!isNativeResource)
            {
                resourceName = string.Format("JavascriptForms.Scripts.{0}", resourceName);
            }
            //string resourceName = string.Format("JavascriptForms.Scripts.{0}", scriptName);

            try
            {
                return ScriptHelper.LoadJavascript(assembly, resourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"An exception occured loading script: {resourceName}");
                System.Diagnostics.Debug.WriteLine(ex);
                return default;
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                Control.RemoveJavascriptInterface("jsBridge");
                //((HybridWebView)Element).Cleanup();
            }

            if (e.NewElement != null)
            {
                HybridWebView hybridWebView = Element as HybridWebView;

                if (hybridWebView == null)
                    throw new NullReferenceException();

                Control.SetWebViewClient(
                    new JavascriptWebViewClient(this,
                        LoadScript(_nativeInvoker),
                        LoadScript(Constants.Scripts.Invoker),
                        LoadScript(Constants.Scripts.JQuery),
                        LoadScript(Constants.Scripts.Spy)
                ));
                Control.AddJavascriptInterface(new JSBridge(this), "jsBridge");

                switch (hybridWebView.SiteSource)
                {
                    case Enums.SiteSource.Local:
                        {
                            Control.LoadUrl($"file:///android_asset/Content/{hybridWebView.Uri}");
                            break;
                        }

                    case Enums.SiteSource.Browser:
                        {
                            Control.LoadUrl(hybridWebView.Uri);
                            break;
                        }

                    default:
                        throw new NotImplementedException($"SiteSource:{hybridWebView.SiteSource} has not been implemented");
                }


                
                
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //((HybridWebView)Element).Cleanup();
            }
            base.Dispose(disposing);
        }
    }
}
