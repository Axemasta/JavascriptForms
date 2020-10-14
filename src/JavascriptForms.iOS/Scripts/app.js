$(document).ready(function () {

    var secretWords = ['cat', 'dog', 'Hello'];

    $('#invoke-name-btn').on('click', function () {

        console.log('submitting');

        invokeCSCode($('#invoke-name-entry').val());
    });

    $('input').on('click', function () {

        console.log('An input was clicked!!!');

        var text = $(this).val();

        console.log(text);

        if (secretWords.includes(text.toLowerCase())) {
            invokeCSCode(text);
        }
    });
});

function invokeCSCode(data) {
    try {
        console.log("Sending Data:" + data);
        invokeCSharpAction(data);
    }
    catch (err) {
        console.log(err);

        alert('Looks like the invokeCSharpAction method was not injected into the DOM. Not to worry, I guess I\'ll let you off for viewing outside of the sample app 😉');
    }
}