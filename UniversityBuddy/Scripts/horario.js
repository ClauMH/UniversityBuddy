document.addEventListener("DOMContentLoaded", function () {

    const reloj = document.getElementById("current-time");
    const btnStart = document.getElementById("btnStart");
    const btnStop = document.getElementById("btnStop");

    const minutes = document.getElementById("minutes");
    const countdown = document.getElementById("countdown");

    const alarmTime = document.getElementById("alarmTime");
    const alarmName = document.getElementById("alarmName");
    const btnAddAlarm = document.getElementById("btnAddAlarm");
    const alarmList = document.getElementById("alarmList");

    let timer = null;
    let timeLeft = 0;
    let alarms = [];

    setInterval(function () {

        const now = new Date();

        reloj.textContent = now.toLocaleTimeString();

        const currentTime =
            String(now.getHours()).padStart(2, "0") +
            ":" +
            String(now.getMinutes()).padStart(2, "0");

        alarms.forEach(function (alarm) {

            if (!alarm.triggered &&
                alarm.time === currentTime) {

                alarm.triggered = true;

                alert("⏰ " + alarm.name);
            }

        });

    }, 1000);

    btnStart.addEventListener("click", function () {

        timeLeft = parseInt(minutes.value || 0) * 60;

        if (timeLeft <= 0) {
            alert("Ingresa un número de minutos");
            return;
        }

        clearInterval(timer);

        timer = setInterval(function () {

            timeLeft--;

            let min = Math.floor(timeLeft / 60);
            let sec = timeLeft % 60;

            countdown.textContent =
                String(min).padStart(2, "0") +
                ":" +
                String(sec).padStart(2, "0");

            if (timeLeft <= 0) {

                clearInterval(timer);

                alert("⏰ Temporizador finalizado");
            }

        }, 1000);

    });

    btnStop.addEventListener("click", function () {

        clearInterval(timer);

    });

    btnAddAlarm.addEventListener("click", function () {

        if (alarmTime.value === "") {

            alert("Selecciona una hora");
            return;
        }

        const alarm = {
            time: alarmTime.value,
            name: alarmName.value || "Alarma",
            triggered: false
        };

        alarms.push(alarm);

        const li = document.createElement("li");

        li.textContent =
            alarm.time +
            " - " +
            alarm.name;

        alarmList.appendChild(li);

        alarmName.value = "";
    });

});