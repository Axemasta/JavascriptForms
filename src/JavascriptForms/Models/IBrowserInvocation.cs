using System;
namespace JavascriptForms.Models
{
    public interface IBrowserInvocation
    {
        string BrowserUrl { get; }

        string Data { get; }

        Coordinates ElementCoordinates { get; }

        Coordinates DisplayDimensions { get; }

        BrowserInfo BrowserInfo { get; }

        string ElementName { get; }
    }
}
