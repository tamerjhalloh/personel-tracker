// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var MyAttendance = function () {

    var setPersonnelAttendanceView = data => {

        var template = "#tmp-personnel-records"; 

        $('#attendances').replaceWith(General.renderTemplate(template, data));

        General.initInputs();
        MyAttendance.init(); 
    }

    return {       
        initOnce: function () { 

            var panel = $('body')

            General.ajax({
                url: "/api/my-attendances", 
                panel: panel, 
                success: function (data) {
                    console.log(data.Response);
                    setPersonnelAttendanceView(data); 
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
    MyAttendance.initOnce();  
});
