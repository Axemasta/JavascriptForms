using System;

using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace JavascriptForms.Pages
{
    public class TabbedPage : Xamarin.Forms.TabbedPage
    {
        public TabbedPage()
        {
            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
        }
    }
}
