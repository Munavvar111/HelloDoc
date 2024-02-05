// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Light Dark Mode Switching js
var input = document.querySelector("#phone");
var iti = window.intlTelInput(input, {
    separateDialCode: true,
    utilsScript: "https://cdn.jsdelivr.net/npm/intl-tel-input@16.0.3/build/js/utils.js",
});

// store the instance variable so we can access it in the console e.g. window.iti.getNumber()
window.iti = iti;
window.onload = () => {
    const myModal = new bootstrap.Modal('#onload');
    myModal.show();
}