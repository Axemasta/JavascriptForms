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
using JavascriptForms.Models;
using Newtonsoft.Json;
using JavascriptForms.Helpers;
using System.Diagnostics;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace JavascriptForms.iOS.Renderers
{
    public class HybridWebViewRenderer : WkWebViewRenderer, IWKScriptMessageHandler
    {
        const string _nativeInvoker = "JavascriptForms.iOS.Scripts.iOSInvoker.js";

        WKUserContentController userController;

        public HybridWebViewRenderer() : this(new WKWebViewConfiguration())
        {
        }

        public HybridWebViewRenderer(WKWebViewConfiguration config) : base(config)
        {
            userController = config.UserContentController;
            var invokationScript = new WKUserScript(new NSString(LoadScript(Constants.Scripts.Invoker)), WKUserScriptInjectionTime.AtDocumentEnd, false);
            var nativeInvoker = new WKUserScript(new NSString(LoadScript(_nativeInvoker)), WKUserScriptInjectionTime.AtDocumentEnd, false);
            var jqueryScript = new WKUserScript(new NSString(LoadScript(Constants.Scripts.JQuery)), WKUserScriptInjectionTime.AtDocumentEnd, false);
            var inputSpyScript = new WKUserScript(new NSString(LoadScript(Constants.Scripts.Spy)), WKUserScriptInjectionTime.AtDocumentEnd, false);

            userController.AddUserScript(invokationScript);
            userController.AddUserScript(nativeInvoker);
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

        private string LoadScript(string resourceName)
        {
            bool isNativeResource = resourceName == _nativeInvoker;

            var assembly = isNativeResource ? Assembly.GetExecutingAssembly() : typeof(JavascriptForms.App).Assembly;

            if (!isNativeResource)
            {
                resourceName = string.Format("JavascriptForms.Scripts.{0}", resourceName);
            }

            try
            {
                return ScriptHelper.LoadJavascript(assembly, resourceName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An exception occured loading script: {resourceName}");
                Debug.WriteLine(ex);
                return default;
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
                //hybridWebView.Cleanup();
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

            try
            {
                IBrowserInvocation args = JsonConvert.DeserializeObject<BrowserInvocation>(message.Body.ToString());

                ((HybridWebView)Element).InvokeAction(args);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to parse browser invocation: {message.Body}");
                Debug.WriteLine(ex);
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
