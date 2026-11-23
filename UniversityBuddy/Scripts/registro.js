document.addEventListener("DOMContentLoaded", function () {
    var txtNombre = document.getElementById("txtNombre");
    var fotoFile = document.getElementById("fotoFile");
    var imgPreview = document.getElementById("imgPreview");

    // 1. Actualizar inicial dinámicamente si no hay imagen elegida
    if (txtNombre) {
        txtNombre.addEventListener("input", function () {
            var tieneArchivo = fotoFile && fotoFile.files && fotoFile.files.length > 0;

            if (!tieneArchivo) {
                var valor = txtNombre.value ? txtNombre.value.trim() : "";
                var inicial = valor.length > 0 ? valor.charAt(0).toUpperCase() : "U";
                var svg = "data:image/svg+xml;utf8,<svg xmlns='http://www.w3.org/2000/svg' width='128' height='128'><rect width='100%' height='100%' fill='%236366f1'/><text x='50%' y='55%' dominant-baseline='middle' text-anchor='middle' font-size='64' font-family='Arial' font-weight='bold' fill='white'>" + inicial + "</text></svg>";

                if (imgPreview) {
                    imgPreview.src = svg;
                }
            }
        });
    }

    // 2. Vista previa si selecciona un archivo de imagen
    if (fotoFile) {
        fotoFile.addEventListener("change", function () {
            if (fotoFile.files && fotoFile.files.length > 0) {
                var reader = new FileReader();

                reader.onload = function (e) {
                    if (imgPreview && e.target) {
                        imgPreview.src = e.target.result;
                    }
                };

                reader.readAsDataURL(fotoFile.files[0]);
            }
        });
    }
});