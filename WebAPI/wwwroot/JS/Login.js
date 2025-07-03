let loginurl = "http://localhost:8080/api/User/Login";



$(document).ready(function () {
    if (localStorage.getItem("JWT") != undefined)
    {
        $("#login").hide();
        $("#register").hide();
        $("#logout").show();
    }
    else if (window.location.href == "Logs.html")
    {
        console.log(localStorage.getItem("JWT"));
        console.log("mum im undefined mum");
        window.location.href = "Login.html";
    }
    else
    {
        $("#login").show();
        $("#register").show();
        $("#logout").hide();
    }
    $("#logout").click(function ()
    {
        localStorage.removeItem("JWT");
        $("#login").show();
        $("#register").show();
        $("#logout").hide();
        window.location.href = "Login.html";
    });
});


function login() {

    $("#login-button").prop("disabled", true);

    let loginData = {
        "username": $("#username").val(),
        "password": $("#password").val()
    }
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
}