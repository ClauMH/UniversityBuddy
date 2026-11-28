(function () {
    "use strict";

    var selectedExercise = "cuadrada";
    var phaseIndex = 0;
    var secondsLeft = 0;
    var timer = null;
    var running = false;
    var cycles = 0;

    var exercises = {
        cuadrada: [
            ["Inhala", 4, "inhale"],
            ["Mantén", 4, "hold"],
            ["Exhala", 4, "exhale"],
            ["Pausa", 4, "pause"]
        ],
        "478": [
            ["Inhala", 4, "inhale"],
            ["Mantén", 7, "hold"],
            ["Exhala", 8, "exhale"]
        ],
        abdominal: [
            ["Inhala profundo", 5, "inhale"],
            ["Exhala lentamente", 5, "exhale"]
        ]
    };

    var circle = document.getElementById("breathingCircle");
    var instruction = document.getElementById("breathingInstruction");
    var counter = document.getElementById("breathingCounter");
    var cycleMessage = document.getElementById("cycleMessage");
    var cycleCount = document.getElementById("circleCount");
    var startButton = document.getElementById("startBreathing");
    var pauseButton = document.getElementById("pauseBreathing");
    var resetButton = document.getElementById("resetBreathing");

    if (!circle || !instruction || !counter || !cycleMessage ||
        !cycleCount || !startButton || !pauseButton || !resetButton) {
        return;
    }

    function clearClasses() {
        circle.classList.remove("inhale", "hold", "exhale", "pause");
    }

    function showPhase() {
        var phase = exercises[selectedExercise][phaseIndex];

        clearClasses();
        circle.classList.add(phase[2]);
        instruction.textContent = phase[0];
        counter.textContent = String(secondsLeft);

        if (phase[0].indexOf("Exhala") !== -1) {
            cycleMessage.textContent = "Suelta el aire sin forzar la respiración.";
        } else if (phase[0] === "Mantén") {
            cycleMessage.textContent = "Mantén suavemente, sin retener con tensión.";
        } else {
            cycleMessage.textContent = "Respira de manera tranquila y natural.";
        }
    }

    function resetExercise() {
        if (timer !== null) {
            clearInterval(timer);
            timer = null;
        }

        running = false;
        phaseIndex = 0;
        secondsLeft = 0;
        clearClasses();

        instruction.textContent = "Preparado";
        counter.textContent = "—";
        cycleMessage.textContent = "Siéntate cómodo y suelta los hombros.";

        startButton.textContent = "Iniciar";
        startButton.disabled = false;
        pauseButton.textContent = "Pausar";
        pauseButton.disabled = true;
    }

    function tick() {
        secondsLeft--;

        if (secondsLeft > 0) {
            counter.textContent = String(secondsLeft);
            return;
        }

        var phases = exercises[selectedExercise];
        phaseIndex++;

        if (phaseIndex >= phases.length) {
            phaseIndex = 0;
            cycles++;
            cycleCount.textContent = String(cycles);
        }

        secondsLeft = phases[phaseIndex][1];
        showPhase();
    }

    function startBreathing() {
        if (running) {
            return;
        }

        if (secondsLeft === 0) {
            phaseIndex = 0;
            secondsLeft = exercises[selectedExercise][0][1];
        }

        running = true;
        startButton.textContent = "Ejercicio activo";
        startButton.disabled = true;
        pauseButton.disabled = false;
        pauseButton.textContent = "Pausar";

        showPhase();
        timer = setInterval(tick, 1000);
    }

    startButton.addEventListener("click", startBreathing);

    pauseButton.addEventListener("click", function () {
        if (running) {
            clearInterval(timer);
            timer = null;
            running = false;
            startButton.disabled = false;
            startButton.textContent = "Reanudar";
            pauseButton.textContent = "Reanudar";
            cycleMessage.textContent = "Ejercicio en pausa.";
            return;
        }

        running = true;
        startButton.disabled = true;
        startButton.textContent = "Ejercicio activo";
        pauseButton.textContent = "Pausar";
        timer = setInterval(tick, 1000);
    });

    startButton.addEventListener("click", function () {
        if (!running && startButton.textContent === "Reanudar") {
            running = true;
            startButton.disabled = true;
            startButton.textContent = "Ejercicio activo";
            pauseButton.textContent = "Pausar";
            timer = setInterval(tick, 1000);
        }
    });

    resetButton.addEventListener("click", function () {
        cycles = 0;
        cycleCount.textContent = "0";
        resetExercise();
    });

    document.querySelectorAll(".exercise-card").forEach(function (card) {
        card.addEventListener("click", function () {
            var cards = document.querySelectorAll(".exercise-card");

            cards.forEach(function (item) {
                item.classList.remove("active");
            });

            card.classList.add("active");
            selectedExercise = card.getAttribute("data-exercise") || "cuadrada";
            cycles = 0;
            cycleCount.textContent = "0";
            resetExercise();
        });
    });
})();