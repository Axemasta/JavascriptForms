$(document).ready(function(){

    $('#invoke-name-btn').on('click', function() {

        console.log('submitting');

        invokeCSCode($('#invoke-name-entry').val());
    });
    
});

function invokeCSCode(data) {
    try {
        console.log("Sending Data:" + data);
        invokeCSharpAction(data);
    }
    catch (err) {
        log(err);
    }
}