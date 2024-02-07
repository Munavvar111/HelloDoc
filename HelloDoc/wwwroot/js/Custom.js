// Light Dark Mode Switching js
document.getElementById("modeSwitch").addEventListener("click", () => {
    const theme = localStorage.getItem("theme") || "light";
    if (theme === "light") {
        applyDarkTheme();
        localStorage.setItem("theme", "dark");
    } else if (theme === "dark") {
        applyLightTheme();
        localStorage.setItem("theme", "light");
    }
});

const applyDarkTheme = () => {
    let list = document.querySelectorAll(".bg-light, .bg-white");
    document.documentElement.setAttribute("data-bs-theme", "dark");
    document.getElementById("modeSwitch").innerHTML = '<span class="material-symbols-outlined"> light_mode </span>'
    for (let i = 0; i < list.length; i++) {
        list[i].classList.add("bg-light-invert");
    }
}

const applyLightTheme = () => {
    let list = document.querySelectorAll(".bg-light, .bg-white");
    document.documentElement.setAttribute("data-bs-theme", "light");
    document.getElementById("modeSwitch").innerHTML = '<span class="material-symbols-outlined"> dark_mode </span>'
    for (let i = 0; i < list.length; i++) {
        list[i].classList.remove("bg-light-invert");
    }
}

window.addEventListener("DOMContentLoaded", () => {
    const theme = localStorage.getItem("theme") || "light";
    if (theme === "light") {
        applyLightTheme();
    } else if (theme === "dark") {
        applyDarkTheme();
    }
})