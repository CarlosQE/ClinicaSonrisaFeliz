// Funciones JavaScript mínimas que usan los componentes Blazor (vía IJSRuntime).
window.sonrisa = {
    // Oculta la pantalla de carga inicial (la llama el layout tras su primer render).
    ocultarSplash: function () {
        const splash = document.getElementById('splash');
        if (!splash || splash.classList.contains('oculto')) return;
        setTimeout(() => {
            splash.classList.add('oculto');
            setTimeout(() => splash.remove(), 400);
        }, 500);
    },
    imprimir: function () { window.print(); },
    enfocar: function (id) { document.getElementById(id)?.focus(); }
};
