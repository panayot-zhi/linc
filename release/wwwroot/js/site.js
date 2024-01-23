// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function () {
    "use strict";

    // Bind Submit targets href form
    $(".submit-link").on('click', doSubmit);
    $("form").on('submit', onSubmit);

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

})();