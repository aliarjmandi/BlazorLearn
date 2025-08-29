// یکبار و یکجا تعریف کن
window.modalHelper = {
    show: (id) => {
        const el = document.getElementById(id);
        if (!el) return;
        const m = bootstrap.Modal.getOrCreateInstance(el);
        m.show();
    },
    hide: (id) => {
        const el = document.getElementById(id);
        if (!el) return;
        const m = bootstrap.Modal.getInstance(el);
        if (m) m.hide();
    },
    scrollIntoView: (elementId) => {
        try {
            const el = document.getElementById(elementId);
            if (el) el.scrollIntoView({ behavior: "smooth", block: "center" });
        } catch { /* ignore */ }
    }
};

console.log("modalHelper.js loaded!");
