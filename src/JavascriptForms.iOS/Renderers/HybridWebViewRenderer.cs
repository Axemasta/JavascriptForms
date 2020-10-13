using System;
using System.IO;
using JavascriptForms;
using JavascriptForms.iOS.Renderers;
using Foundation;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using JavascriptForms.Controls;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace JavascriptForms.iOS.Renderers
{
    public class HybridWebViewRenderer : WkWebViewRenderer, IWKScriptMessageHandler
    {
        const string JavaScriptFunction = "function invokeCSharpAction(data){window.webkit.messageHandlers.invokeAction.postMessage(data);}";
        WKUserContentController userController;

        public HybridWebViewRenderer() : this(new WKWebViewConfiguration())
        {
        }

        public HybridWebViewRenderer(WKWebViewConfiguration config) : base(config)
        {
            userController = config.UserContentController;
            var script = new WKUserScript(new NSString(JavaScriptFunction), WKUserScriptInjectionTime.AtDocumentEnd, false);
            userController.AddUserScript(script);
            userController.AddScriptMessageHandler(this, "invokeAction");
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                userController.RemoveAllUserScripts();
                userController.RemoveScriptMessageHandler("invokeAction");
                HybridWebView hybridWebView = e.OldElement as HybridWebView;
                hybridWebView.Cleanup();
            }

            if (e.NewElement != null)
            {
                HybridWebView hybridWebView = Element as HybridWebView;

                if (hybridWebView == null)
                    throw new NullReferenceException();

                switch (hybridWebView.SiteSource)
                {
                    case Enums.SiteSource.Local:
                        {
                            string filename = Path.Combine(NSBundle.MainBundle.BundlePath, $"Content/{hybridWebView.Uri}");
                            LoadRequest(new NSUrlRequest(new NSUrl(filename, false)));
                            break;
                        }

                    case Enums.SiteSource.Browser:
                        {
                            LoadRequest(new NSUrlRequest(new NSUrl(hybridWebView.Uri)));
                            break;
                        }

                    default:
                        throw new NotImplementedException($"SiteSource:{hybridWebView.SiteSource} has not been implemented");
                }

                
            }
        }

        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            ((HybridWebView)Element).InvokeAction(message.Body.ToString());
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
