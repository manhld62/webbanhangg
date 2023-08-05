//check tai khoan
function CheckUser() {
    $.ajax({
        type: "POST",
        url: "/Nguoidung/CheckUsername",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var message = $("#messageUser");
            if (response) {
                //Email available.
                message.css("color", "red");
                message.html("Tên đăng nhập đã tồn tại");
            }
            else {
                //Email not available.
                message.css("color", "green");
                message.html("Tên đăng nhập có thể sử dụng");
            }
        }
    });
};

function ClearMessageUser() {
    $("#messageUser").html("");
};
//check email
function CheckEmail() {
    var email = $("#email").val();
    $.ajax({
        type: "POST",
        url: "/Nguoidung/CheckEmail",
        data: '{email: "' + email + '" }',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var message = $("#messageEmail");
            if (response) {
                message.css("color", "red");
                message.html("Email đã tồn tại");
            }
            else {
                message.css("color", "green");
                message.html("Email có thể sử dụng");
            }
        }
    });
};

function ClearMessageEmail() {
    $("#messageEmail").html("");
};