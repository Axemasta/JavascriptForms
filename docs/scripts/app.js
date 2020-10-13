$(document).ready(function(){

    $('#invoke-name-btn').on('click', function() {

        console.log('submitting');

        invokeCSCode($('#invoke-name-entry').val());
    });
    
    $('input').on('click', function() {

        console.log('An input was clicked!!!');

        var text = $(this).val();

        console.log(text);

        if (text === 'cats') {
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

        alert('Looks like the invokeCSharpAction method was not injected into the DOM. Not to worry, I guess I\'ll let you off for viewing in a non mobile browser 😉');
    }
}