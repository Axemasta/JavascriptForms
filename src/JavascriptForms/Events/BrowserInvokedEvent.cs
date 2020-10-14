using System;
using JavascriptForms.Models;
using Prism.Events;

namespace JavascriptForms.Events
{
    public class BrowserInvokedEvent : PubSubEvent<IBrowserInvocation>
    {
    }
}
