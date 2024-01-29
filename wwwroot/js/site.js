// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function () {
    "use strict";

    // Bind Submit targets href form
    $("form").on('submit', onSubmit);
    $(".submit-link").on('click', doSubmit);
    $("[contenteditable=\"true\"]").on('blur', onBlurContentEditable);
    $("[contenteditable=\"true\"]").on('focus', onFocusContentEditable);
    $("[contenteditable=\"true\"]").on('keydown', onSaveContentEditable);
    //$("a[href=\"#\"].nav-link").on('click', (e) => {
    //   e.preventDefault();
    //    return false;
    //});

    window.currentEditingElementId = null;

    window.showPreloader = function () {
        let preloader = document.getElementById('preloader');
        preloader.hidden = false;
    }

    window.hidePreloader = function () {
        let preloader = document.getElementById('preloader');
        preloader.hidden = true;
    }

    function doSubmit(e) {
        e.preventDefault();

        let target = $(e.target).attr("href");
        target = $(target);
        target.submit();

        return false;
    }

    function onSubmit(e) {
        let form = e.target;
        let $form = $(e.target);

        // check validity of the form 
        // with jquery validate unobtrusive
        if ($form.valid && !$form.valid()) {
            return;
        }

        // check validity of the form
        // with vanilla javascript
        if (!form.checkValidity()) {
            return;
        }

        // well - submit then
        window.showPreloader();
    }

    // I copy-pasted this from sumwhere
    // please work, I don wanna debug u
    function prettyPrintHTML(html) {

        var tab = '\t';
        var result = '';
        var indent = '';

        html.split(/>\s*</).forEach(function (element) {
            if (element.match(/^\/\w/)) {
                indent = indent.substring(tab.length);
            }

            result += indent + '<' + element + '>\r\n';

            if (element.match(/^<?\w[^>]*[^\/]$/) && !element.startsWith("input")) {
                indent += tab;
            }
        });

        return result.substring(1, result.length - 3);

    }

    function onBlurContentEditable(e) {
        e.target.innerHTML = e.target.innerText;
        window.currentEditingElementId = null;
    }

    function onFocusContentEditable(e) {

        // NOTE: enter this function only once on click per element
        if (window.currentEditingElementId !== e.target.id) {
            window.currentEditingElementId = e.target.id;
            e.target.innerText = prettyPrintHTML(e.target.innerHTML);            
        }
    }

    function onSaveContentEditable(e) {
        if (e.which === 13 && e.shiftKey === false) {

            e.preventDefault();
            e.stopPropagation();
            e.stopImmediatePropagation();

            Swal.fire({
                title: window.jsLocalizer["SetStringResource_Dialog_Title"],
                text: window.jsLocalizer["SetStringResource_Dialog_Description"],
                icon: "question",
                showCancelButton: true,
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
            }).then((result) => {
                if (!result.isConfirmed) {
                    return;
                }

                window.showPreloader();

                let request = {
                    key: e.target.id,
                    value: e.target.innerText,
                    editedById: "xxx"
                };

                $.ajax({

                    type: "POST",
                    contentType: "application/json",
                    url: "/home/set-string-resource",
                    data: JSON.stringify(request),

                    success: function (response) {
                        window.location.reload(true);
                    },

                    error: function (xhr, status, error) {
                        window.hidePreloader();

                        Swal.fire({
                            icon: "error",
                            title: window.jsLocalizer["AlertMessage_Error_Title"],
                            html: window.jsLocalizer["SetStringResource_Error"] + `<br /> ${status} - ${error}`,
                            footer: window.jsLocalizer["AlertMessage_Error_Footer"]
                        });
                    }

                });

            });
            
            return false;
        }
    }

})();