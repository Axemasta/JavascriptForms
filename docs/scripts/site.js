/* Scripts for the docs site. Nothing here goes into the mobile app */

$(document).ready(function () {

    // RenderMarkdown();

    function RenderMarkdown() {

        console.log('rendering markdown');

        try {
            var text = $('#raw-setup-md').text();
            var target = $('#rendered-setup-md');
            var converter = new showdown.Converter();
            var html = converter.makeHtml(text);
    
            target.html(html);

            // $('pre code').each(function(i, e) {hljs.highlightBlock(e)});
        }
        catch (err) {
            console.log(err);
        }
    }
});

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

    console.log('this is the dummy invoke method: ' + json);
}