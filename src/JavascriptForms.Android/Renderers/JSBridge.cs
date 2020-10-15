using System;
using System.Diagnostics;
using Android.Webkit;
using Java.Interop;
using JavascriptForms.Controls;
using JavascriptForms.Models;
using Newtonsoft.Json;

namespace JavascriptForms.Droid.Renderers
{
    public class JSBridge : Java.Lang.Object
    {
        readonly WeakReference<HybridWebViewRenderer> hybridWebViewRenderer;

        public JSBridge(HybridWebViewRenderer hybridRenderer)
        {
            hybridWebViewRenderer = new WeakReference<HybridWebViewRenderer>(hybridRenderer);
        }

        [JavascriptInterface]
        [Export("invokeAction")]
        public void InvokeAction(string data)
        {
            HybridWebViewRenderer hybridRenderer;

            if (hybridWebViewRenderer != null && hybridWebViewRenderer.TryGetTarget(out hybridRenderer))
            {
                //((HybridWebView)hybridRenderer.Element).InvokeAction(data);

                

                try
                {
                    IBrowserInvocation args = JsonConvert.DeserializeObject<BrowserInvocation>(data);

                    ((HybridWebView)hybridRenderer.Element).InvokeAction(args);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Unable to parse browser invocation: {data}");
                    Debug.WriteLine(ex);
                }
            }
        }
    }
}
