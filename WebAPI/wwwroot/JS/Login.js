let loginurl = "http://localhost:8080/api/User/Login";


function login() {

    $("#login-button").prop("disabled", true);

    let loginData = {
        "username": $("#username").val(),
        "password": $("#password").val()
    }

    console.log(loginData);


    $.ajax({
        method: "POST",
        url: loginurl,
        data: JSON.stringify(loginData),
        contentType: 'application/json'
    }).done(function (tokenData)
    {
        localStorage.setItem("JWT", tokenData);
        $("#login-button").prop("disabled", false);

        window.location.href = "Logs.html";
    }).fail(function (err)
    {
        alert(err.responseText);

        localStorage.removeItem("JWT");
        $("#login-button").prop("disabled", false);
    });


    console.log(localStorage);
}