let rurl = "http://localhost:8080/api/User/Register";
function register() {
    $("#register-button").prop("disabled", true);

    let rData = {
        "firstName": $("#firstName").val(),
        "lastName": $("#lastName").val(),
        "email": $("#email").val(),
        "username": $("#username").val(),
        "password": $("#password").val()
    }
    $.ajax({
        method: "POST",
        url: rurl,
        data: JSON.stringify(rData),
        contentType: 'application/json'
    }).done(function (token) {
        login();
    }).fail(function (err) {
        alert(err.responseText);

        localStorage.removeItem("JWT");
        $("#register-button").prop("disabled", false);
    });




}