let passwordInput=document.getElementById("password");
let passwordBtn=document.getElementById("passwordBtn")
function togglePasswordVisibility(){
    if (passwordInput.type === "password") {
        passwordInput.type = "text";
      } else {
        passwordInput.type = "password";
      }
}
passwordBtn.addEventListener("click",()=>togglePasswordVisibility())
