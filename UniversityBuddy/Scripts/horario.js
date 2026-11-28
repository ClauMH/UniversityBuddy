document.addEventListener("DOMContentLoaded", function () {
    const reloj = document.querySelector("#current-time");

    if (!reloj) {
        return;
    }

    setInterval(function () {
        reloj.textContent = new Date().toLocaleTimeString("es-BO");
    }, 1000);
});