document.getElementById("fileInput").onchange = function () {
  let path = this.value.substr(this.value.lastIndexOf(`\\`) + 1);
  document.getElementById("fileName").innerText = path;
};
