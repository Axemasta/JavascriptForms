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