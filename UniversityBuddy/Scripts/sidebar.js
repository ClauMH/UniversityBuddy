(function () {
    'use strict';

    function iniciarSidebar() {
        const body = document.body;
        const html = document.documentElement;
        const sidebar = document.getElementById('appSidebar');
        const btnMenu = document.getElementById('sidebarToggle');
        const btnCollapse = document.getElementById('collapseToggle');
        const btnTheme = document.getElementById('themeToggle');
        const themeIcon = document.getElementById('themeIcon');
        const overlay = document.getElementById('appOverlay');

        if (!sidebar) return;

        // ===== Lógica de Tema Claro / Oscuro =====
        function actualizarIcono(tema) {
            if (themeIcon) {
                themeIcon.textContent = tema === 'light' ? '☀️' : '🌙';
            }
        }

        // Leer tema inicial
        var temaActual = html.getAttribute('data-theme') || 'dark';
        actualizarIcono(temaActual);

        if (btnTheme) {
            btnTheme.addEventListener('click', function () {
                var estadoActual = html.getAttribute('data-theme');
                var nuevoTema = estadoActual === 'light' ? 'dark' : 'light';

                html.setAttribute('data-theme', nuevoTema);
                localStorage.setItem('ub_theme', nuevoTema);
                actualizarIcono(nuevoTema);
            });
        }

        // ===== Lógica de Colapso / Despliegue Sidebar =====
        const desktop = window.matchMedia('(min-width: 993px)');

        function cerrarMenuMovil() {
            body.classList.remove('sidebar-open');
        }

        if (btnMenu) {
            btnMenu.addEventListener('click', function () {
                if (desktop.matches) {
                    body.classList.toggle('sidebar-collapsed');
                } else {
                    body.classList.toggle('sidebar-open');
                }
            });
        }

        if (btnCollapse) {
            btnCollapse.addEventListener('click', function () {
                if (desktop.matches) {
                    body.classList.toggle('sidebar-collapsed');
                }
            });
        }

        if (overlay) {
            overlay.addEventListener('click', cerrarMenuMovil);
        }
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', iniciarSidebar);
    } else {
        iniciarSidebar();
    }
})();