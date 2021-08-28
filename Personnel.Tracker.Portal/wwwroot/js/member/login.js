// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var Login = function () {
    return {       
        init: function () { 

            $("input", $("#form-auth")).off("keydown").keydown(function (e) {
                if (e.which == 13) {
                    var username = $("#email").val();
                    var password = $("#password").val();
                    if (username.trim() != "" && password.trim() != "") {
                        $("#do-login").click();
                    }
                }
            });

            $("#do-login").off("click").click(function () {

                var panel = $('#form-auth');

                var username = $("#email").val();
                var password = $("#password").val();
                if (username.trim() == "" && password.trim() == "") {
                    General.notifyFailure("Please enter all required fields!");
                    return false;
                }

                var login = {
                    Username: username,
                    Password: password
                };

                General.ajax({ 
                    url: "/api/sign-in",
                    data: login,
                    panel: panel,
                    sender: $("#do-login"),
                    success: function (data) {
                        window.location = login.ReturnUrl == null || login.ReturnUrl == '' ? "/home/index" : login.ReturnUrl;
                    },
                    failure: function (data) {
                        General.notifyFailure(data.ErrorMessage);
                    }
                });

            });
        }
    }
}();

$(document).ready(function () {
    Login.init();
});
