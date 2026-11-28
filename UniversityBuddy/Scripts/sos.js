document.addEventListener("DOMContentLoaded", function () {

    const mensaje = document.getElementById("mensaje");
    const contador = document.getElementById("contador");
    const iniciar = document.getElementById("btnIniciar");
    const reiniciar = document.getElementById("btnReiniciar");
    

    let timer = null;
    let segundos = 4;
    let ciclos = 0;
    let maxCiclos = 4;

    iniciar.addEventListener("click", function () {

        clearInterval(timer);

        segundos = 4;
        ciclos = 0;

        timer = setInterval(function () {

            contador.textContent = segundos;

            if (segundos > 0) {

                mensaje.textContent = "Inhala";

                segundos--;

            } else {

                mensaje.textContent = "Exhala";

                ciclos++;

                if (ciclos >= maxCiclos) {

                    clearInterval(timer);

                    mensaje.textContent =
                        "Ejercicio completado";

                    contador.textContent = "✔";

                    return;
                }

                segundos = 4;
            }

        }, 1000);

    });

    reiniciar.addEventListener("click", function () {

        clearInterval(timer);

        mensaje.textContent = "Preparado";
        contador.textContent = "0";

        ciclos = 0;
        segundos = 4;

    });

});;