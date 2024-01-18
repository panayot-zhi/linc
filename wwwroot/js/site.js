// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function () {
    "use strict";

    // Bind Submit targets href form
    $("form").on('submit', onSubmit);
    $(".submit-link").on('click', doSubmit);    
    $("[contenteditable=\"true\"]").on('keydown', onSaveContentEditable);

    function doSubmit(e) {
        e.preventDefault();

        let target = $(e.target).attr("href");
        target = $(target);
        target.submit();

        return false;
    }

    function onSubmit(e) {
        let form = $(e.target);

        if (form.valid()) {
            let preloader = document.getElementById('preloader');
            preloader.hidden = false;
        }
    }

    function onSaveContentEditable(e) {
        if (e.which === 13 && e.shiftKey === false) {
            e.preventDefault();
            e.stopPropagation();
            e.stopImmediatePropagation();

            console.log("Save");

            return false;
        }
    }

})();