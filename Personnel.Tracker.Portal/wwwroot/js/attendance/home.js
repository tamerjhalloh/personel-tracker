// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var Home = function () {

    var setPersonnelCheckView = lastCheck => {
        var template = "";
        if (lastCheck == null || lastCheck.PersonnelCheckType == 'Out') {
            if (lastCheck == null)
                lastCheck = {};
            template = "#tmp-personnel-checked-out";
        }
        else {
            template = "#tmp-personnel-checked-in";
        }

        $('#check-status-holder').replaceWith(General.renderTemplate(template, lastCheck));

        General.initInputs();
        Home.init(); 
    }

    return {       
        initOnce: function () { 

            var panel = $('body')

            General.ajax({
                url: "/api/last", 
                panel: panel, 
                success: function (data) { 
                    setPersonnelCheckView(data.Response); 
                } 
            });
        },
        init: function () { 

            $(".check-button").off("click").click(function () {

                var cntrl = $(this);
                var panel = $('#check-status-holder'); 
                var type = $(this).attr('data-status'); 

                General.ajax({
                    url: "/api/set-personnel-check",
                    data: { PersonnelCheckType: type},
                    panel: panel,
                    sender: cntrl,
                    success: function (data) { 
                        setPersonnelCheckView(data.Response); 
                    } 
                });

            });
        }
    }
}();

$(document).ready(function () {
    Home.initOnce(); 

});
