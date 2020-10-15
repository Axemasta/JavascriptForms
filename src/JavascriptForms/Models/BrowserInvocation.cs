namespace JavascriptForms.Models
{
    public class BrowserInvocation : IBrowserInvocation
    {
        public string BrowserUrl { get; set; }

        public string Data { get; set; }

        public Coordinates ElementCoordinates { get; set; }

        public Coordinates DisplayDimensions { get; set; }

        public BrowserInfo BrowserInfo { get; set; }

        public string ElementName { get; set; }
    }
}
