// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var Personnel = function () {

    var pageIndex = 0;
    var PageSize = 20;


    var setPersonnelView = data => {

        data.PageCount = Math.round(data.TotalCount / PageSize) + (data.TotalCount % PageSize > 0 ? 1 : 0);


        var template = "#tmp-personnel-records";

        $('#personnels').html(General.renderTemplate(template, data));

        General.initInputs();
        Personnel.init();
    }

    var searchPersonnel = () => {

        var filters = {
            Search: $('#search').val()
        }
        var query = {
            Parameter: filters,
            PageIndex: pageIndex,
            PageSize: PageSize,
            OrderBy: 'Name'
        };

        var panel = $('body');
        General.ajax({
            url: "/api/personnels",
            data: query,
            panel: panel,
            success: function (data) {

                setPersonnelView(data);
            }
        });
    }

    return {
        initOnce: function () {

            //$('#date-picker').calendar({
            //    type: 'date'
            //});

            //    searchAttendances({});


        },
        init: function () {

            $("#search-personnel").off("click").click(function () {
                pageIndex = 0;
                searchPersonnel();
            });

            $(".page-item").off("click").click(function () {
                pageIndex = $(this).attr('data-index');
                searchPersonnel();
            });

            $(".page-item-prev").off("click").click(function () {
                pageIndex = parseInt(pageIndex) - 1;
                searchPersonnel();
            });

            $(".page-item-next").off("click").click(function () {
                pageIndex = parseInt(pageIndex) + 1;
                searchPersonnel();
            });

            $("#add-personnel").off("click").click(function () {
                var personnelForm = General.renderTemplate('#tmp-personnel-form', {});
                $(personnelForm).modal('show');
                Personnel.initFormEvents();
            });

            $(".edit-personnel").off("click").click(function () {

                var cntrl = $(this);

                var personnelId = $(cntrl).attr('data-id');

                General.ajax({
                    url: "/api/personnels/get?id=" + personnelId,
                    type: General.requestType().Get, 
                    panel : $('body'),
                    sender: cntrl,
                    success: function (data) {
                        var personnelForm = General.renderTemplate('#tmp-personnel-form', data.Response);
                        $(personnelForm).modal('show');
                        Personnel.initFormEvents();
                    }
                });   
            });
        },
        initFormEvents: function () {

            var form = $('.ui.form')
                .form({
                    fields: {
                        Name: 'empty',
                        Surname: 'empty',
                        Email: 'empty',
                        Password: 'empty'
                    }
                });



            $(form).on('submit', function (e) {
                e.preventDefault();
                var model = form.form('get values');
                model.PasswordHash = model.Password;
                console.log(model);
                //console.log(form.form('is valid')); 
                if (form.form('is valid')) { 
                    General.ajax({
                        url: "/api/personnels/set",
                        data: model,
                        panel: form,
                        sender: $(form).find('button'),
                        success: function (data) {
                            $('.ui.modal').modal('hide');
                            $('.ui.modal').remove();
                            pageIndex = 0;
                            searchPersonnel();
                        }
                    }); 
                }
            }); 

        }
    }
}();

$(document).ready(function () {
    Personnel.initOnce();
    Personnel.init();
});
