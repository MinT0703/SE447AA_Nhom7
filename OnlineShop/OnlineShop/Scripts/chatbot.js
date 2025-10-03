// File: /Scripts/chatbot.js
(function () {
    // ===== Helpers truy cập DOM =====
    const W = () => document.getElementById('chatbot-widget');
    const M = () => document.getElementById('cb-messages');
    const I = () => document.getElementById('cb-input');

    // ====== Bubbles & chips ======
    function addBubble(text, who) {
        const box = M(); if (!box) return;
        const div = document.createElement('div');
        div.className = 'cb-bubble ' + (who === 'user' ? 'cb-user' : 'cb-bot');
        div.textContent = text;
        box.appendChild(div);
        box.scrollTop = box.scrollHeight;
    }

    function addSuggestions(list) {
        const box = M(); if (!box) return;
        const wrap = document.createElement('div');
        wrap.className = 'cb-suggest';
        list.forEach(t => {
            const b = document.createElement('button');
            b.type = 'button';
            b.className = 'cb-chip';
            b.textContent = t;
            b.onclick = () => { const input = I(); if (input) { input.value = t; send(); } };
            wrap.appendChild(b);
        });
        box.appendChild(wrap);
        box.scrollTop = box.scrollHeight;
    }

    // ====== Hiển/ẩn widget ======
    function show() {
        const w = W(); if (!w) return;
        w.classList.remove('cb-hidden');
        w.classList.remove('cb-animating');
        const input = I(); if (input) input.focus();
    }

    function hide() {
        const w = W(); if (!w) return;
        w.classList.add('cb-animating');
        setTimeout(() => { w.classList.add('cb-hidden'); }, 160);
    }

    function toggle() {
        const w = W(); if (!w) return;
        if (w.classList.contains('cb-hidden')) show(); else hide();
    }

    // ====== Gửi tin ======
    async function send() {
        const input = I(); if (!input) return;
        let msg = (input.value || '').trim();
        if (!msg) return;

        addBubble(msg, 'user');
        input.value = '';
        addBubble('Đang gõ...', 'bot');

        try {
            const url = (window.CHATBOT_ENDPOINT || 'Chatbot.ashx');
            const res = await fetch(url, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ message: msg })
            });

            const text = await res.text();
            let data;
            try { data = JSON.parse(text); }
            catch { throw new Error('Phản hồi không phải JSON: ' + text.slice(0, 200)); }

            // bỏ bubble "Đang gõ..."
            const bots = M().querySelectorAll('.cb-bot');
            if (bots.length) bots[bots.length - 1].remove();

            if (data && data.ok === true) {
                addBubble(data.reply || '(trống)', 'bot');
                if (Array.isArray(data.suggestions)) addSuggestions(data.suggestions);
            } else if (data && data.message && data.message.content) {
                // fallback JSON gốc từ Ollama
                addBubble(data.message.content || '(trống)', 'bot');
            } else {
                addBubble('Lỗi: ' + (data && (data.error || data.detail) || 'Không xác định'), 'bot');
            }
        } catch (err) {
            const bots = M().querySelectorAll('.cb-bot');
            if (bots.length) bots[bots.length - 1].remove();
            addBubble('Không thể kết nối máy chủ: ' + err.message, 'bot');
        }
    }

    function handleKey(e) {
        if (e.key === 'Enter' && !e.shiftKey) { e.preventDefault(); send(); }
    }

    // ====== Quick FAQ (3 nút gợi ý) ======
    function renderFAQQuickActions() {
        if (document.getElementById('cb-faq-quick')) return; // tránh trùng

        const box = M(); if (!box) return;

        const wrap = document.createElement('div');
        wrap.id = 'cb-faq-quick';
        wrap.className = 'cb-quick';
        wrap.innerHTML = [
            `<button type="button" class="cb-quick-btn" data-q="chính sách bảo hành">Bảo hành</button>`,
            `<button type="button" class="cb-quick-btn" data-q="chính sách đổi trả">Đổi trả</button>`,
            `<button type="button" class="cb-quick-btn" data-q="hỗ trợ liên hệ">Hỗ trợ</button>`
        ].join('');

        box.appendChild(wrap);
        box.scrollTop = box.scrollHeight;

        wrap.addEventListener('click', function (e) {
            const btn = e.target.closest('.cb-quick-btn');
            if (!btn) return;
            const q = btn.getAttribute('data-q') || btn.textContent.trim();
            const input = I();
            if (input) input.value = q;
            send();
            wrap.remove();
        });
    }

    // ====== Auto setup khi load trang ======
    document.addEventListener('DOMContentLoaded', () => {
        const w = W();
        if (w && !w.classList.contains('cb-hidden')) w.classList.add('cb-hidden');
    });

    // Lời chào tự động 1 lần / session + hiện FAQ
    document.addEventListener('DOMContentLoaded', async () => {
        try {
            if (!sessionStorage.getItem('rb_greeted')) {
                let txt = 'Xin chào! Mình là Radian Bot. Mình có thể giúp gì cho bạn?';
                // (tuỳ chọn) gọi server để lấy câu chào cá nhân hoá
                try {
                    const res = await fetch('Chatbot.ashx', {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({ action: 'greet' })
                    });
                    if (res.ok) {
                        const data = await res.json();
                        if (data && data.ok && data.reply) txt = data.reply;
                    }
                } catch (e) { /* im lặng nếu lỗi */ }

                show();
                // đợi widget sẵn sàng 1 nhịp
                setTimeout(() => {
                    addBubble(txt, 'bot');
                    renderFAQQuickActions(); // hiện 3 nút ngay sau lời chào
                }, 50);

                sessionStorage.setItem('rb_greeted', '1');
            }
        } catch { /* noop */ }
    });

    // ====== Xuất API toàn cục ======
    window.RadianBot = {
        toggle, show, hide, send, handleKey,
        addBubble, addSuggestions,
        renderFAQ: renderFAQQuickActions
    };
})();
