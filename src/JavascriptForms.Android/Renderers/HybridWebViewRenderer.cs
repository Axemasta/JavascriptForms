using System;
using Android.Content;
using JavascriptForms.Controls;
using JavascriptForms.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace JavascriptForms.Droid.Renderers
{
    public class HybridWebViewRenderer : WebViewRenderer
    {
        const string JavascriptFunction = "function invokeCSharpAction(data){jsBridge.invokeAction(data);}";
        Context _context;

        public HybridWebViewRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                Control.RemoveJavascriptInterface("jsBridge");
                ((HybridWebView)Element).Cleanup();
            }

            if (e.NewElement != null)
            {
                HybridWebView hybridWebView = Element as HybridWebView;

                if (hybridWebView == null)
                    throw new NullReferenceException();

                Control.SetWebViewClient(new JavascriptWebViewClient(this, $"javascript: {JavascriptFunction}"));
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
                ((HybridWebView)Element).Cleanup();
            }
            base.Dispose(disposing);
        }
    }
}
