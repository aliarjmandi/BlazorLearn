window.auth = {
    postJson: async function (url, data) {
        const res = await fetch(url, {
            method: "POST",
            credentials: "include",           // ← ست/ارسال کوکی‌ها
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(data)
        });
        let json = null;
        try { json = await res.json(); } catch { json = null; }
        return { ok: res.ok, data: json };
    }
};

