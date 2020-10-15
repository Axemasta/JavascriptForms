using System.Linq;
using Android.Webkit;
using Xamarin.Forms.Platform.Android;

namespace JavascriptForms.Droid.Renderers
{
    public class JavascriptWebViewClient : FormsWebViewClient
    {
        string[] _scripts;

        public JavascriptWebViewClient(HybridWebViewRenderer renderer, params string[] scripts) : base(renderer)
        {
            _scripts = scripts;
        }

        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);

            if (_scripts != null && _scripts.Count() > 0)
            {
                foreach (string script in _scripts)
                {
                    view.EvaluateJavascript(script, null);
                }
            }
        }
    }
}
