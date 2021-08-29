// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var MyAttendance = function () {

    var setPersonnelAttendanceView = data => {

        var template = "#tmp-personnel-records"; 

        $('#attendances').html(General.renderTemplate(template, data));

        General.initInputs();
        MyAttendance.init(); 
    }

    var searchAttendances = filters => {

        var panel = $('body'); 
        General.ajax({
            url: "/api/my-attendances",
            data: filters,
            panel: panel,
            success: function (data) {
                console.log(data.Response);
                setPersonnelAttendanceView(data);
            }
        });
    }

    return {       
        initOnce: function () {  
           
            $('#date-picker').calendar({
                type: 'date' 
            });

            searchAttendances({});
          
        },
        init: function () { 

            $("#search-attendances").off("click").click(function () {

                var filters = {
                    CreationTime: $('#date-picker').find('input').val()
                }

                searchAttendances(filters);
            });
        }
    }
}();

$(document).ready(function () {
    MyAttendance.initOnce();
    MyAttendance.init();
});
