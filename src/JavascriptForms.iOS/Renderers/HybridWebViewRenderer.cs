using System;
using System.IO;
using JavascriptForms;
using JavascriptForms.iOS.Renderers;
using Foundation;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using JavascriptForms.Controls;
using System.Reflection;

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
            var invokationScript = new WKUserScript(new NSString(JavaScriptFunction), WKUserScriptInjectionTime.AtDocumentEnd, false);
            var jqueryScript = new WKUserScript(new NSString(LoadJquery()), WKUserScriptInjectionTime.AtDocumentEnd, false);
            var inputSpyScript = new WKUserScript(new NSString(LoadAppJs()), WKUserScriptInjectionTime.AtDocumentEnd, false);

            userController.AddUserScript(invokationScript);
            userController.AddUserScript(jqueryScript);
            userController.AddUserScript(inputSpyScript);
            userController.AddScriptMessageHandler(this, "invokeAction");
            //userController.AddScriptMessageHandler(this, "logging");

            //string js = @"var console = { log: function(msg){window.webkit.messageHandlers.logging.postMessage(msg) } };";
            //this.EvaluateJavaScript(new NSString(js), (result, error) =>
            //{
            //    if (error != null)
            //        Console.WriteLine($"installation of console.log() failed: {0}", error);
            //});
        }

        private string LoadJquery()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "JavascriptForms.iOS.Scripts.jquery-3.5.1.min.js";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();

                return result;
            }
        }

        private string LoadAppJs()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "JavascriptForms.iOS.Scripts.app.js";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();

                return result;
            }
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
            //Console.WriteLine(message.Body);

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
