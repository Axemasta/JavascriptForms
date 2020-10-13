$(document).ready(function () {
    $('input').on('click', function () {

        console.log('An input was clicked!!!');

        var text = $(this).val();

        console.log(text);

        if (text === 'Hello') {
            invokeCSCode(text);
        }
    });
});

function invokeCSCode(data) {
    try {
        console.log("Sending Data:" + data);
        invokeCSharpAction(data);
    } catch (err) {
        log(err);
    }
}