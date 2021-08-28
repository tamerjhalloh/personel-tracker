// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var General = function () {

    // var baseUrl = "https://localhost:44346"

    var requestType = {
        Get: "Get",
        Post: "POST",
        Put: "PUT",
        Delete: "DELETE"
    }

    var requestDataType = {
        Json: "json",
        Html: "html",
        Xml: "xml"
    }

    var state = {
        success: "success",
        danger: "danger",
        warning: "warning",
        info: "info",
        primary: "primary",
        brand: "brand"
    }

    return {
        ajax: function (options) {
            options = $.extend(true,
                {
                    dataType: requestDataType.Json,
                    type: requestType.Post,
                    // contentType: "application/json",
                    timeout: 3 * 60 * 1000,
                    async: true,
                    activateSender: true
                },
                options);

            if (options.sender) {
                General.playButton(options.sender);
            }

            if (options.panel) {
                General.block(options.panel);
            }


            $.ajax({
                // url: baseUrl + options.url,
                url: options.url,
                data: General.jsFriendlyJSONStringify(options.data),
                type: options.type,
                dataType: options.dataType,
                processData: options.processData,
                contentType: options.contentType,
                timeout: options.timeout,
                async: options.async,
                success: function (data) {
                    if (data.Result) {

                        if (data.Message == undefined ||
                            data.Message == null ||
                            data.Message == 'null' ||
                            data.Message == '')
                            data.Message = "Operation succeeded";

                        if (!options.success) {
                            General.notify(data.Message, state.success, 3);
                        } else {

                            if (options.showOpertionResultMessage) {
                                General.notify(data.Message, state.Success, 3);
                            }
                            options.success(data);
                        }


                        if (options.done) {
                            options.done();
                        }

                        if (options.sender && options.activateSender) {
                            General.stopButton(options.sender);
                        }
                    } else {

                        if (data.ErrorMessage == undefined ||
                            data.ErrorMessage == null ||
                            data.ErrorMessage == 'null' ||
                            data.ErrorMessage == '')
                            data.ErrorMessage = "operation failed";

                        if (options.sender) {
                            General.stopButton(options.sender);
                        }

                        if (data.ErrorType == 'General.TimedOut') {
                            //TODO:
                        } else {

                            if (!options.failure) {
                                General.notify(data.ErrorMessage, state.danger, 5);
                            } else {

                                if (options.showOpertionResultMessage) {
                                    General.notify(data.ErrorMessage, state.danger, 5);
                                }

                                options.failure(data);

                            }


                        }
                    }

                    if (options.panel) {
                        General.unblock(options.panel);
                    }
                },
                error: function (data) {

                    if (options.panel) {
                        General.unblock(options.panel);
                    }

                    if (options.sender) {
                        General.stopButton(options.sender);
                    }

                    if (options.error) {

                        if (data == undefined)
                            data = {};

                        if (data.ErrorMessage == undefined)
                            data.ErrorMessage = General.getSystemMessage(data);

                        options.error(data);

                    } else {
                        if (data.status == 403) {
                            General.notify("You are not authorized to do this action", state.warning, 5);
                        }
                        else if (data.status == 401) {
                            General.notify("You are not authorized to do this action, you will be redirected to main page", statestate.warning, 5);
                            setTimeout(function () { location.href = "/member/login"; }, 3000);
                        }
                        else {
                            General.notify(General.getSystemMessage(data), state.danger, 5);
                        }

                    }
                }
            });
        },
        jsFriendlyJSONStringify: function (s) {
            if (s)
                return JSON.parse(JSON.stringify(s).
                    replace(/\u2028/g, '\\u2028').
                    replace(/\u2029/g, '\\u2029'));
            return s
        },
        getSystemMessage: function (data) {
            console.log(data);
            return "One or more error happened while processing your request. We are working on it...";
        },
        notify: function (message, type, timer, title) {

            if (type == undefined || type == "") {
                type = state.Primary;
            } 

            if (type == 'danger')
                type = 'error';

            $('body')
                .toast({
                   // class: type,
                    message: message,
                    position: 'top center',
                    class: 'center aligned ' + type
                });
        },
        block: function (item, options) {
            $(item).waitMe({});
        },
        unblock: function (item) {
            $(item).waitMe('hide');
        },
        playButton: function (el) {
            $(el).addClass('loading');
        },
        stopButton: function (el) {
            $(el).removeClass('loading');
        },
        notifySuccess: function (message) {
            General.notify(message == undefined || message.trim() == '' ? "Operation succeeded" : message, state.success, 3);
        },
        notifyFailure: function (message) {
            General.notify(message == undefined || message.trim() == '' ? "Operation failed" : message, state.danger, 3);
        },
        renderTemplate: function (template, data, param = {}) {
            var html = "";
            if ($(template).length) {
                var tmp = $(template)
                if (data)

                    html = tmp.render(data, param);
                else
                    html = tmp.render({}, param);
            }
            return html;

        },
        formatDateTime: function (orginaldate, dateOnly) { 

            if (orginaldate == undefined || orginaldate == null || orginaldate == '') {
                return "";
            }

            var date = new Date(orginaldate);
            var day = date.getDate();
            var month = date.getMonth() + 1;
            var year = date.getFullYear();

            if (year < 1990)
                return "";

            var h = date.getHours();
            var m = date.getMinutes();
            if (day < 10) {
                day = "0" + day;
            }
            if (month < 10) {
                month = "0" + month;
            }

            if (h < 10) {
                h = "0" + h;
            }

            if (m < 10) {
                m = "0" + m;
            }

            var date = dateOnly && dateOnly == true ? day + "/" + month + "/" + year :  day + "/" + month + "/" + year + " " + h + ":" + m;
            return date;
        },
        initInputs: function () {
            $.each($(".date-text"), function (i, input) {  
                $(input).text(General.formatDateTime($(input).attr('data-value'), $(input).attr('data-format') == "date-only"));
            });
        }
    }
}();

$(document).ready(function () {
    General.initInputs();
});
