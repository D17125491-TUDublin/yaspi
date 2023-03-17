// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function updateMessageIndicator() {
    $.ajax({
        url: "/Home/GetUnreadMessagesCount",
        type: "GET",
        success: function (data) {
            if (data === 0) {
                $('#messages-tab').text("Messages");
            } else {
                //alert("You have " + data + " unread messages")
                $('#messages-tab').text("Messages (" + data + ")");
            }
        },
        error: function (xhr, status, error) {
            
        }
    });
}

$(document).ready(updateMessageIndicator())


// #character-indicator -->

function updateCharacterIndicator() {
    var maxLength = 280;
    $('#message-input').keyup(function () {
        var length = $(this).val().length;
        var length = maxLength - length;
        $('#character-indicator').text(length + ' characters remaining');
        if (length < 0) {
            $('#character-indicator').css('color', 'red');
        } else {
            $('#character-indicator').css('color', 'black');
        }
        if (length < 0) {
            $('#Submit').prop('disabled', true);
        } else {
            $('#Submit').prop('disabled', false);
        }
    });
}

$(document).ready(updateCharacterIndicator());


function markAsRead(id) {
    $('#card-' + id).slideUp("normal", function () { $(this).remove(); });
    var d = JSON.stringify({ id: id });
    $.ajax({
        type: "POST",
        url: "/Home/MessageRead",
        data: d,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: updateMessageIndicator,
        // error: errorFunc
    });
}
