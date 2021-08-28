// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var Home = function () {
    return {       
        initOnce: function () { 

            var panel = $('body')

            General.ajax({
                url: "/api/last", 
                panel: panel, 
                success: function (data) {
                    //console.log(data);
                    var template = "";
                    if (data.Response == null || data.Response.PersonnelCheckType == 'Out') {
                        if (data.Response == null)
                            data.Response = {};
                        template = "#tmp-personnel-checked-out";
                    }
                    else {
                        template = "#tmp-personnel-checked-in";
                    }

                    $('#check-status-holder').replaceWith(General.renderTemplate(template, data.Response));

                    Home.init(); 
                } 
            });
        },
        init: function () { 

            $(".check-button").off("click").click(function () {

                var panel = $('#check-status-holder'); 
                var type = $(this).attr('data-status'); 

                General.ajax({
                    url: "/api/set-personnel-check",
                    data: { PersonnelCheckType: type},
                    panel: panel,
                    sender: $("#do-login"),
                    success: function (data) {
                         
                    } 
                });

            });
        }
    }
}();

$(document).ready(function () {
    Home.initOnce(); 

});
