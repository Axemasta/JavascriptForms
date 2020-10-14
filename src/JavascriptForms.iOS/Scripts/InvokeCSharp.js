function invokeCSharpAction(data, elementCoords, screenSize, browserDetails) {

    var browserInvocation = {
        BrowserUrl: window.location.href,
        Data: data,
        ElementCoordinates: elementCoords,
        DisplayDimensions: screenSize,
        BrowserInfo: browserDetails
    };

    var json = JSON.stringify(browserInvocation);

    window.webkit.messageHandlers.invokeAction.postMessage(json);
}