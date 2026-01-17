// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function () {
    "use strict";

    $.ajaxSetup({
        statusCode: {

            400: function () {
                Swal.fire({
                    icon: "warning",
                    title: window.jsLocalizer["AlertMessage_Warning_Title"],
                    html: window.jsLocalizer["AlertMessage_400Warning"],
                    footer: window.jsLocalizer["AlertMessage_Warning_Footer"]
                });
            },

            401: function (xhr) {

                // establish login location if server responded with it
                let location = xhr.getResponseHeader("Location");

                // challenge user to login at that location or the default one
                window.location.href = location ?? "/identity/account/login"
            },

            403: function () {
                Swal.fire({
                    icon: "warning",
                    title: window.jsLocalizer["AlertMessage_Warning_Title"],
                    html: window.jsLocalizer["AlertMessage_403Warning"],
                    footer: window.jsLocalizer["AlertMessage_Warning_Footer"]
                });
            },

            404: function () {
                Swal.fire({
                    icon: "warning",
                    title: window.jsLocalizer["AlertMessage_Warning_Title"],
                    html: window.jsLocalizer["AlertMessage_404Warning"],
                    footer: window.jsLocalizer["AlertMessage_Warning_Footer"]
                });
            }
        }
    });

    // Bind Submit targets href form
    $(".submit-link").on('click', doSubmit);
    $("form:not(.no-preloader)").on('submit', onSubmit);
    $('input.trim').on('blur', (e) => {
        e.target.value = e.target.value.trim();
    });
    //$("a[href=\"#\"].nav-link").on('click', (e) => {
    //   e.preventDefault();
    //    return false;
    //});

    // Trigger popovers
    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'))

    var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl)
    })

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

    // Team: toggle literature/culture history section (fade-up / fade-down)
    (function initTeamHistoryToggle() {
        const toggleId = "display_literature_and_culture_history_2024-2026";
        const targetId = "literature_and_culture_history_2024-2026";

        const toggle = document.getElementById(toggleId);
        const target = document.getElementById(targetId);

        if (!toggle || !target) {
            return;
        }

        let isAnimating = false;

        function setExpanded(expanded) {
            toggle.setAttribute("aria-expanded", expanded ? "true" : "false");
            target.setAttribute("aria-hidden", expanded ? "false" : "true");
        }

        function show() {
            if (isAnimating || !target.classList.contains("d-none")) {
                return;
            }

            isAnimating = true;
            target.classList.remove("d-none");

            // restart animation reliably
            target.classList.remove("lc-history--anim-out");
            void target.offsetWidth;
            target.classList.add("lc-history--anim-in");
            setExpanded(true);

            const done = () => {
                target.classList.remove("lc-history--anim-in");
                target.removeEventListener("animationend", done);
                isAnimating = false;
            };

            target.addEventListener("animationend", done);
        }

        function hide() {
            if (isAnimating || target.classList.contains("d-none")) {
                return;
            }

            isAnimating = true;
            target.classList.remove("lc-history--anim-in");
            void target.offsetWidth;
            target.classList.add("lc-history--anim-out");

            const done = () => {
                target.classList.add("d-none");
                target.classList.remove("lc-history--anim-out");
                setExpanded(false);
                target.removeEventListener("animationend", done);
                isAnimating = false;
            };

            target.addEventListener("animationend", done);
        }

        // ensure consistent initial state
        setExpanded(!target.classList.contains("d-none"));

        toggle.addEventListener("click", (e) => {
            e.preventDefault();

            if (target.classList.contains("d-none")) {
                show();
            } else {
                hide();
            }
        });
    })();

})();