// Light Dark Mode Switching js
var linkdark = document.createElement('link');
linkdark.rel = "stylesheet";
linkdark.href = "//cdn.jsdelivr.net/npm/@sweetalert2/theme-dark@4/dark.css";
var linklight = document.createElement('link');
linklight.rel = "stylesheet";
linklight.href = "//cdn.jsdelivr.net/npm/@sweetalert2/theme-minimal/minimal.css";
document.getElementById("modeSwitch").addEventListener("click", () => {
    const theme = localStorage.getItem("theme") || "light";
    if (theme === "light") {
        applyDarkTheme();
        localStorage.setItem("theme", "dark");
        document.head.appendChild(linkdark);
        document.head.removeChild(linklight);

    } else if (theme === "dark") {
        applyLightTheme();
        localStorage.setItem("theme", "light");
        document.head.appendChild(linklight);
        document.head.removeChild(linkdark);

    }
});

const applyDarkTheme = () => {
    let list = document.querySelectorAll(".bg-light, .bg-white");
    document.documentElement.setAttribute("data-bs-theme", "dark");
    document.getElementById("modeSwitch").innerHTML = '<span class="material-symbols-outlined"> light_mode </span>'
    for (let i = 0; i < list.length; i++) {
        list[i].classList.add("bg-light-invert");
    }
    var elements = document.querySelectorAll('.leaflet-layer, .leaflet-control-zoom-in, .leaflet-control-zoom-out, .leaflet-control-attribution');
    elements.forEach(function (element) {
        element.style.filter = "invert(100%) hue-rotate(180deg) brightness(95%) contrast(90%)";
    });
}

const applyLightTheme = () => {
    let list = document.querySelectorAll(".bg-light, .bg-white");
    document.documentElement.setAttribute("data-bs-theme", "light");
    document.getElementById("modeSwitch").innerHTML = '<span class="material-symbols-outlined"> dark_mode </span>'
    for (let i = 0; i < list.length; i++) {
        list[i].classList.remove("bg-light-invert");
    }
    var elements = document.querySelectorAll('.leaflet-layer, .leaflet-control-zoom-in, .leaflet-control-zoom-out, .leaflet-control-attribution');
    elements.forEach(function (element) {
        element.style.filter = "";
    });
}

window.addEventListener("DOMContentLoaded", () => {
    const theme = localStorage.getItem("theme") || "light";
    if (theme === "light") {
        applyLightTheme();
        document.head.appendChild(linklight);
    } else if (theme === "dark") {
        applyDarkTheme();
        document.head.appendChild(linkdark);
    }
})

