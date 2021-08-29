// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var Attendance = function () {

    var setPersonnelAttendanceView = data => {

        var template = "#tmp-personnel-records";

        $('#attendances').html(General.renderTemplate(template, data));

        General.initInputs();
        Attendance.init();
    }

    var searchAttendances = filters => {

        var panel = $('body');
        General.ajax({
            url: "/api/attendances",
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

        //    searchAttendances({});

            $('.ui.search').search({
                apiSettings: {
                    url: '/api/personnel/search?query={query}'
                },
                fields: {
                    results: 'Response',
                    title: 'Name' 

                },
                minCharacters: 2,
                onSelect(result, response) {
                    $('#personnel').val(result.PersonnelId);
                } 
            });


            $("#personnel-autocomplete").change(function () { 
                $('#personnel').val('');
            });

            $("#personnel-autocomplete").blur(function () {
                if ($('#personnel').val() == '')
                    $("#personnel-autocomplete").val('');
            });
        },
        init: function () {

            $("#search-attendances").off("click").click(function () {

                var filters = {
                    CreationTime: $('#date-picker').find('input').val(),
                    PersonnelId: $('#personnel').val()
                }

                if (filters.CreationTime == '' && filters.PersonnelId == '') {
                    General.notifyFailure(`Please choose date or a personnel`);
                    return false;
                }

                console.log(filters);

                searchAttendances(filters);
            });
        }
    }
}();

$(document).ready(function () {
    Attendance.initOnce();
    Attendance.init();
});
