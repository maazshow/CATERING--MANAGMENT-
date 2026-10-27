(function () {
    const $ = id => document.getElementById(id);
    const form = $('orderForm');
    const d = form.dataset;
    const container = $('itemsContainer');

    const P = {
        bRice: +d.bRice, bSabz: +d.bSabz, bGaram: +d.bGaram, bMeat: +d.bMeat,
        zRice: +d.zRice, zSweets: +d.zSweets,
        kMeat: +d.kMeat, bkMeat: +d.bkMeat,
        hKg: +d.hKg, cKg: +d.cKg, cSabz: +d.cSabz, cGaram: +d.cGaram, wKg: +d.wKg, sKg: +d.sKg, gRice: +d.gRice, gGur: +d.gGur
    };

    // Price Settings se har item ki cooking charges (order mein badli ja sakti hain).
    // Korma aur Beef Korma mein ye rate per kg meat hai, baqi items mein per item.
    const COOK = { Biryani: +d.bCook, Zarda: +d.zCook, Korma: +d.kCook, BeefKorma: +d.bkCook, WhiteRice: +d.wCook, SimpleRice: +d.sCook, GurChawal: +d.gCook, Haleem: +d.hCook, ChanaDaig: +d.cCook };

    const RICE_OPTIONS = [8, 10, 12, 14];
    const fmt = n => 'Rs ' + Number(n || 0).toLocaleString('en-US', { maximumFractionDigits: 2 });
    const num = el => (el ? parseFloat(el.value) || 0 : 0);

    const TITLES = {
        Biryani: '🍚 Biryani Daig', Zarda: '🍮 Zarda', Korma: '🍲 Korma',
        BeefKorma: '🥩 Beef Korma', WhiteRice: '🍚 White Rice', SimpleRice: '🍚 Simple Rice', GurChawal: '🍯 Gor Wala Chawal', Haleem: '🥣 Haleem', ChanaDaig: '🫘 Chana Wala Chawal'
    };

    function riceSelect(selected) {
        return `<select class="form-select" data-f="RiceKg">` +
            RICE_OPTIONS.map(r => `<option value="${r}" ${+selected === r ? 'selected' : ''}>${r} kg</option>`).join('') +
            `</select>`;
    }

    function cookingField(col, value, label) {
        return `<div class="${col}"><label class="form-label">${label || 'Cooking charges (Rs)'}</label>
            <input type="number" class="form-control" data-f="CookingCharge" min="0" step="any" value="${value}" placeholder="0" /></div>`;
    }

    function fieldsFor(type, v, cookValue) {
        const val = x => (x === null || x === undefined ? '' : x);
        if (type === 'Biryani') {
            return `
                <div class="col-md-4"><label class="form-label">Rice (kg)</label>${riceSelect(v.rice || 8)}</div>
                <div class="col-md-4"><label class="form-label">Meat (kg)</label>
                    <input type="number" class="form-control" data-f="MeatKg" min="0.5" step="0.5" value="${val(v.meat)}" required /></div>
                ${cookingField('col-md-4', cookValue)}`;
        }
        if (type === 'Zarda' || type === 'WhiteRice' || type === 'SimpleRice' || type === 'GurChawal') {
            return `
                <div class="col-md-6"><label class="form-label">Rice (kg)</label>
                    <input type="number" class="form-control" data-f="RiceKg" min="0.5" step="0.5" value="${val(v.rice)}" required /></div>
                ${cookingField('col-md-6', cookValue)}`;
        }
        if (type === 'Korma' || type === 'BeefKorma') {
            return `
                <div class="col-md-6"><label class="form-label">Meat (kg)</label>
                    <input type="number" class="form-control" data-f="MeatKg" min="0.5" step="0.5" value="${val(v.meat)}" required /></div>
                ${cookingField('col-md-6', cookValue, 'Cooking (Rs per kg meat)')}`;
        }
        if (type === 'ChanaDaig') {
            return `
                <div class="col-md-6"><label class="form-label">Rice (kg)</label>
                    <input type="number" class="form-control" data-f="RiceKg" min="0.5" step="0.5" value="${val(v.rice)}" required /></div>
                ${cookingField('col-md-6', cookValue)}`;
        }
        // Haleem: kg
        return `
            <div class="col-md-6"><label class="form-label">${type === 'Haleem' ? 'Haleem' : 'Chana daig'} (kg)</label>
                <input type="number" class="form-control" data-f="QuantityKg" min="0.5" step="0.5" value="${val(v.qty)}" required /></div>
            ${cookingField('col-md-6', cookValue)}`;
    }

    function addItem(type, v, restoring) {
        v = v || {};
        // Naye item par Price Settings wali cooking bhar dein; wapas aaye hue (validation fail) item mein wahi jo likha tha.
        const cookValue = restoring ? (v.cooking === null || v.cooking === undefined ? '' : v.cooking) : COOK[type];

        const card = document.createElement('div');
        card.className = 'item-card';
        card.dataset.type = type;
        card.innerHTML = `
            <div class="d-flex justify-content-between align-items-center mb-2">
                <strong class="item-title"></strong>
                <button type="button" class="btn btn-sm btn-outline-danger" data-remove title="Item hata dein"><i class="bi bi-x-lg"></i></button>
            </div>
            <input type="hidden" data-f="ItemType" value="${type}" />
            <div class="row g-2">${fieldsFor(type, v, cookValue)}</div>
            <div class="item-sum d-flex justify-content-between mt-2">
                <span class="text-muted small item-detail"></span>
                <strong class="item-amount">Rs 0</strong>
            </div>`;
        container.appendChild(card);

        // Biryani: meat default rice ke barabar (jab tak khud na badlein).
        if (type === 'Biryani') {
            const rice = card.querySelector('[data-f="RiceKg"]');
            const meat = card.querySelector('[data-f="MeatKg"]');
            if (!meat.value) meat.value = rice.value;
            let touched = !!v.meat;
            meat.addEventListener('input', () => { touched = true; });
            rice.addEventListener('change', () => { if (!touched) meat.value = rice.value; });
        }
        renumber();
    }

    function amountOf(card) {
        const type = card.dataset.type;
        const rice = num(card.querySelector('[data-f="RiceKg"]'));
        const meat = num(card.querySelector('[data-f="MeatKg"]'));
        const qty = num(card.querySelector('[data-f="QuantityKg"]'));
        const cook = num(card.querySelector('[data-f="CookingCharge"]'));

        if (type === 'Biryani') {
            return {
                amount: rice * (P.bRice + P.bSabz + P.bGaram) + meat * P.bMeat + cook,
                detail: `Rice ${rice}×${P.bRice + P.bSabz + P.bGaram} + Meat ${meat}×${P.bMeat} + Cooking ${cook}`
            };
        }
        if (type === 'WhiteRice') {
            return { amount: rice * P.wKg + cook, detail: `Rice ${rice}×${P.wKg} + Cooking ${cook}` };
        }
        if (type === 'SimpleRice') {
            return { amount: rice * P.sKg + cook, detail: `Rice ${rice}×${P.sKg} + Cooking ${cook}` };
        }
        if (type === 'GurChawal') {
            return { amount: rice * (P.gRice + P.gGur) + cook, detail: `Rice ${rice}×${P.gRice + P.gGur} (rice+gor) + Cooking ${cook}` };
        }
        if (type === 'Zarda') {
            return { amount: rice * (P.zRice + P.zSweets) + cook, detail: `Rice ${rice}×${P.zRice + P.zSweets} + Cooking ${cook}` };
        }
        if (type === 'Korma') {
            return { amount: meat * (P.kMeat + cook), detail: `Meat ${meat}×${P.kMeat} + Cooking ${meat}×${cook}` };
        }
        if (type === 'BeefKorma') {
            return { amount: meat * (P.bkMeat + cook), detail: `Meat ${meat}×${P.bkMeat} + Cooking ${meat}×${cook}` };
        }
        if (type === 'Haleem') {
            return { amount: qty * P.hKg + cook, detail: `Haleem ${qty}×${P.hKg} + Cooking ${cook}` };
        }
        const chanaRate = P.cKg + P.cSabz + P.cGaram;
        return { amount: rice * chanaRate + cook, detail: `Rice ${rice}×${chanaRate} (rice+masalay) + Cooking ${cook}` };
    }

    function renumber() {
        const cards = container.querySelectorAll('.item-card');
        cards.forEach((card, i) => {
            card.querySelector('.item-title').textContent = `${i + 1}. ${TITLES[card.dataset.type]}`;
            card.querySelectorAll('[data-f]').forEach(el => { el.name = `Items[${i}].${el.dataset.f}`; });
        });
        update();
    }

    function update() {
        let total = 0;
        container.querySelectorAll('.item-card').forEach(card => {
            const r = amountOf(card);
            card.querySelector('.item-amount').textContent = fmt(r.amount);
            card.querySelector('.item-detail').textContent = r.detail;
            total += r.amount;
        });
        $('totalAmount').textContent = fmt(total);

        const advance = parseFloat($('advancePaid').value) || 0;
        const remaining = total - advance;
        const remEl = $('remainingAmount');
        remEl.textContent = fmt(Math.max(0, remaining));
        remEl.className = 'fs-4 fw-bold ' + (advance > total ? 'text-danger' : remaining <= 0 ? 'text-success' : advance > 0 ? 'text-warning' : 'text-danger');
        $('advancePaid').classList.toggle('is-invalid', advance > total);
    }

    document.querySelectorAll('[data-add]').forEach(b =>
        b.addEventListener('click', () => addItem(b.dataset.add)));

    container.addEventListener('input', update);
    container.addEventListener('change', update);
    $('advancePaid').addEventListener('input', update);
    container.addEventListener('click', e => {
        const rm = e.target.closest('[data-remove]');
        if (!rm) return;
        rm.closest('.item-card').remove();
        renumber();
    });

    form.addEventListener('submit', e => {
        const err = $('clientError');
        err.classList.add('d-none');

        if (!container.querySelector('.item-card')) {
            e.preventDefault();
            err.textContent = 'Kam az kam ek item add karein.';
            err.classList.remove('d-none');
            return;
        }
        if (!form.checkValidity()) {
            e.preventDefault();
            form.classList.add('was-validated');
            const bad = form.querySelector(':invalid');
            if (bad) bad.focus();
        }
    });

    // Pehle ke items wapas dikhayen (validation fail ke baad), warna ek Biryani item.
    let initial = [];
    try { initial = JSON.parse($('initialItems').textContent) || []; } catch (e) { initial = []; }
    if (initial.length) initial.forEach(i => addItem(i.type, i, true));
    else addItem('Biryani');
    update();

    // Order save hone ke baad popup oper dikhayen.
    const modal = $('successModal');
    if (modal && modal.dataset.show === '1') {
        bootstrap.Modal.getOrCreateInstance(modal).show();
    }
})();
