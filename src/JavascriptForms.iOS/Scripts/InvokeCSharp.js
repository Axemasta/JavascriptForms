function invokeCSharpAction(data, elementCoords, screenSize, browserDetails, elementId) {

    var browserInvocation = {
        BrowserUrl: window.location.href,
        Data: data,
        ElementCoordinates: elementCoords,
        DisplayDimensions: screenSize,
        BrowserInfo: browserDetails,
        ElementName: elementId
    };

    var json = JSON.stringify(browserInvocation);

    window.webkit.messageHandlers.invokeAction.postMessage(json);
}