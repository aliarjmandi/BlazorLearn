window.modalHelper = {
    show: (id) => {
        console.log("modalHelper.show", id);
        const el = document.getElementById(id);
        if (!el) return;
        const m = bootstrap.Modal.getOrCreateInstance(el);
        m.show();
    },
    hide: (id) => {
        console.log("modalHelper.hide", id);
        const el = document.getElementById(id);
        if (!el) return;
        const m = bootstrap.Modal.getInstance(el);
        if (m) m.hide();
    }
};
console.log("modalHelper.js loaded!");
