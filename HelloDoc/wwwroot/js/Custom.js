document.body.style = "background-color: var(--bs-dark);transition: 0.5s;"

var theme = "light";
const root = document.querySelector(":root");
const textColor = document.querySelectorAll(".text-change-dark");
var container = document.getElementsByClassName("theme-container")[0];

const themeIcon = document.getElementById("theme-icon");
const themeIcon2 = document.getElementById("theme-icon2");
const navbar = document.querySelector("nav");

console.log(document.getElementsByClassName("theme-container")[0])
if (container) {

    container.addEventListener("click", setTheme);
    console.log("ndf")
    function setTheme() {
        switch (theme) {
            case "light":
                setDark();
                theme = "dark";
                break;
            case "dark":
                setLight();
                theme = "light";
                break;
        }
    }
}

function setDark() {
    root.style.setProperty("--bs-dark", "#212529");
    container.style.backgroundColor = "#37b5cc";
    themeIcon.classList.add("d-none");
    themeIcon2.classList.remove("d-none");
    setTimeout(() => {

        themeIcon2.classList.remove("change");
    }, 300);

    for (let i = 0; i < textColor.length; i++) {
        textColor[i].classList.remove("text-change-dark");
        textColor[i].classList.add("text-change");
    }
    themeIcon2.classList.add("change");
    // themeIcon.src = moon2;

}

function setLight() {
    root.style.setProperty(
        "--bs-dark",
        "linear-gradient(318.32deg, #c3d1e4 0%, #dde7f3 55%, #d4e0ed 100%)"
    );

    container.style.backgroundColor = "#fff";
    themeIcon.classList.remove("d-none");
    themeIcon2.classList.add("d-none");

    setTimeout(() => {
        themeIcon.classList.remove("change");
    }, 300);

    for (let i = 0; i < textColor.length; i++) {
        textColor[i].classList.remove("text-change");
        textColor[i].classList.add("text-change-dark");
    }
    themeIcon.classList.add("change");
    // themeIcon.src = sun;

}
