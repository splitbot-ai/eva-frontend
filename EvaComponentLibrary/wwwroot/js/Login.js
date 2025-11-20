document.getElementById("tab-1").addEventListener("change", function () {
    if (document.getElementById("tab-1").checked) {
        document.querySelector(".login-container").style.display = "flex";
        document.querySelector(".server-container").style.display = "none";
    }
   
});
document.getElementById("tab-2").addEventListener("change", function () {
    if (document.getElementById("tab-2").checked) {
        document.querySelector(".login-container").style.display = "none";
        document.querySelector(".server-container").style.display = "flex";

    }
   
});