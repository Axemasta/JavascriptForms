# Setup

This section explains how you can setup your Xamarin Forms application to be able to inject, intercept & invoke events within your app.

We will be using `WebView` as our primary browser, this will renderer down to native implemenations:

- `WkWebView` on iOS
- `GoogleChrome` on Android

![The official xamarin](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/custom-renderer/hybridwebview-images/webview-classes.png)

## Shared

A custom renderer will be used so to make things less likely to interfere with other components, we will subclass `WebView` and create `HybridWebView`. In this example repository I have modified the `HybridWebView` to accept either a local html file or a url, you will only need to do this if you are displaying both in your app. To see this alternative implementation see [here](https://github.com/Axemasta/JavascriptForms/blob/main/src/JavascriptForms/Controls/HybridWebView.cs).

```csharp
public class HybridWebView : WebView
{
    public static readonly BindableProperty UriProperty = BindableProperty.Create(nameof(Uri), typeof(string), typeof(HybridWebView), default(string));

    public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(HybridWebView));

    public string Uri
    {
        get => (string)GetValue(UriProperty);
        set => SetValue(UriProperty, value);
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
```

You can reference this webview in xaml:

```xml
<ContentPage ...
xmlns:controls="clr-namespace:YourNameSpace;assembly=CustomRenderer"
x:Class="YourNameSpace.HybridWebViewPage"
Padding="0,40,0,0">
<controls:HybridWebView Command="{Binding InvokeNameCommand}" Uri="https://axemasta.github.io/JavascriptForms/"/>
</ContentPage>
```



In my source code example I am using the following interface `IBrowserInvocation`:

```csharp
public interface IBrowserInvocation
{
    string BrowserUrl { get; }

    string Data { get; }

    Coordinates ElementCoordinates { get; }

    Coordinates DisplayDimensions { get; }

    BrowserInfo BrowserInfo { get; }

    string ElementName { get; }
}
```



You do not have to use this object, I simply wanted to pass some extra information to my App when a javascript event is invoked. By default a `string` is passed to the app from the javascript, so if you use a more complex object type (like my example) you should serialise it as JSON first (`var json = JSON.stringify('{ "foo" : "bar" }');`).

Next we will setup our custom renderers on each platform.

## iOS

For the entire class see the source code example [here](https://github.com/Axemasta/JavascriptForms/blob/main/src/JavascriptForms.iOS/Renderers/HybridWebViewRenderer.cs).

Your renderer will need to inherit `WkWebViewRenderer` and implement `IWKScriptMessageHandler`. `IWKScriptMessageHandler` will add the following method to your class:

```csharp
public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
{
}
```

This is the method that will be called if our javascript invocation has been successful. The data passed from the javascript can be found in the `message.Body` property.



You will need to add a new paramaterless constructor to your renderer that initializes a `WKWebViewConfiguration` object, we will use this setup inject the javascript.

```csharp
WKUserContentController _userController;

public HybridWebViewRenderer() : this(new WKWebViewConfiguration())
{
}

public HybridWebViewRenderer(WKWebViewConfiguration config) : base(config)
{
    string js = "alert('I have been loaded!')";
  	string nativeCaller = "function invokeNative(payload) { window.webkit.messageHandlers.invokeAction.postMessage(payload); }";

    _userController = config.UserContentController;
    var myScript = new WKUserScript(new NSString(js), WKUserScriptInjectionTime.AtDocumentEnd, false);
		var nativeInvoker = new WKUserScript(new NSString(nativeCaller), WKUserScriptInjectionTime.AtDocumentEnd, false);
  
    _userController.AddUserScript(myScript);
  	_userController.AddUserScript(nativeInvoker);
    _userController.AddScriptMessageHandler(this, "invokeAction");
}
```

We create a new `WKUserScript` containing our javascript and add it to the web view. We then add a script message handler, this handler is a web kit handler and will be invoked by our native caller script. The message handler name is a string and any number of handlers can be added to the webview.



Finally we override the `OnElementChanged` method of the renderer and handle setup / teardown of the `WebView`.

```csharp
protected override void OnElementChanged(VisualElementChangedEventArgs e)
{
    base.OnElementChanged(e);

    if (e.OldElement != null)
    {
        userController.RemoveAllUserScripts();
        userController.RemoveScriptMessageHandler("invokeAction");
        HybridWebView hybridWebView = e.OldElement as HybridWebView;
    }

    if (e.NewElement != null)
    {
        HybridWebView hybridWebView = Element as HybridWebView;

        if (hybridWebView == null)
            throw new InvalidCastException("Could not cast Element as HybridWebView");

        LoadRequest(new NSUrlRequest(new NSUrl(hybridWebView.Uri)));
    }
}
```

Finally don't forget to export your renderer 😇

```csharp
[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
```



## Android

For the entire class and external classes see the source code example [here](https://github.com/Axemasta/JavascriptForms/tree/main/src/JavascriptForms.Android/Renderers).

Your renderer will need to inherit `Xamarin.Forms.Platform.Android.WebViewRenderer`. Add a new constructor to your renderer and pass in the `Context` class, otherwise the compiler will shout at you (one of my fave quirks on android). This time we will be doing the majority of the config in the `OnElementChanged` method.

```csharp
const string nativeInvokeScript = "function invokeNative(payload) { jsBridge.invokeAction(payload); }";
const string testScript = "alert('I have been loaded!')";

protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
{
    base.OnElementChanged(e);

    if (e.OldElement != null)
    {
        Control.RemoveJavascriptInterface("jsBridge");
    }

    if (e.NewElement != null)
    {
        HybridWebView hybridWebView = Element as HybridWebView;

        if (hybridWebView == null)
            throw new NullReferenceException();

        Control.SetWebViewClient(
            new JavascriptWebViewClient(this,
                LoadScript(nativeInvokeScript),
                LoadScript(testScript)
        ));
        Control.AddJavascriptInterface(new JSBridge(this), "jsBridge");

        Control.LoadUrl(hybridWebView.Uri);
    }
}
```



We now need to create a webview client and a javascript bridge.

#### Web View Client

Create a class that inherits `FormsWebViewClient`:

```csharp
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
```

I have jazzed up this class a little from the official Xamarin [sample](https://github.com/xamarin/xamarin-forms-samples/blob/master/CustomRenderers/HybridWebView/Droid/JavascriptWebViewClient.cs). In this example only 1 script is passed to the webview and it is prefixed with: `"javascript: {0}"`. In my example we use a few scripts so it makes sense to make the scripts and array.

#### Javascript Bridge

We will now need a callback for our injected scripts, add the following class:

```csharp
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
```

This class maintains a weak reference to the webview renderer in order to avoid a circular dependency. Similar to iOS this class exports the action `"invokeAction"` which acts as the link between the browser and app. When the action is called you can handle the payload invoked from the browser and then call the `InvokeAction` method on the `HybridWebView` which will update the view models.



In order to use the `[Export]` attribute, you must add a project reference in the android project to: `Mono.Android.Export`.



## The Premise

Our webview's each inject shared and native code. Each renderer specifies a slightly different "hook" to invoke the c# code. In my examples I have made the common invokation method generic and then injected the native handler on each platform, leaving us with the following call stack:

```
=> Javascript Event
	=> Javascript Event Listener         {external code}
-------------------------------------------------------
		=> InvokeCSharpAction();           {our code}
			=> InvokeNative();
				=> Method invoked in renderer
```

We then invoke an `ICommand` with the given payload (`string`, `IBrowserInvocation` etc), which can be bound too and handled further from any viewmodel. This has given us a really easy MVVM friendly native level implementation 😁



## Expanding Upon Design

We now have a custom renderer for each platform that can inject and invoke javascript into the natively rendered `WebView`. In my source code example I added a more detailed invocation object, this is a great place to start. Further improvements could be:

- Intercepting `console.log()` events and logging them in the xamarin app
- Adding more error handling
- Making things more typesafe (Typescript).