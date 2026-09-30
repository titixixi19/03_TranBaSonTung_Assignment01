// Popup dialogs for Create/Update and confirmation for Delete.
(function () {
    const modalEl = document.getElementById('appModal');
    const appModal = new bootstrap.Modal(modalEl);
    const confirmModal = new bootstrap.Modal(document.getElementById('confirmModal'));
    let pendingForm = null;

    function setModalBody(html) {
        const body = modalEl.querySelector('.modal-body');
        body.innerHTML = html;
        const form = body.querySelector('form');
        if (form && $.validator && $.validator.unobtrusive) {
            $.validator.unobtrusive.parse(form);
        }
    }

    function isLoginRedirect(response) {
        return response.redirected && response.url.indexOf('/Auth/') !== -1;
    }

    // Open a popup: <button data-modal-url="/Accounts/Create" data-modal-title="New account">
    $(document).on('click', '[data-modal-url]', async function (e) {
        e.preventDefault();
        const url = this.getAttribute('data-modal-url');
        modalEl.querySelector('.modal-title').textContent = this.getAttribute('data-modal-title') || '';
        setModalBody('<div class="text-center py-4"><div class="spinner-border text-primary"></div></div>');
        appModal.show();

        const response = await fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } });
        if (isLoginRedirect(response)) { window.location.href = response.url; return; }
        setModalBody(await response.text());
    });

    // Submit popup forms via AJAX; JSON {success:true} closes and reloads, HTML re-renders the form with errors
    $(document).on('submit', 'form.ajax-form', async function (e) {
        e.preventDefault();
        const form = this;
        if ($(form).valid && !$(form).valid()) return;

        const submit = form.querySelector('[type=submit]');
        if (submit) submit.disabled = true;
        try {
            const response = await fetch(form.action, {
                method: 'POST',
                body: new FormData(form),
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            });
            if (isLoginRedirect(response)) { window.location.href = response.url; return; }

            const contentType = response.headers.get('content-type') || '';
            if (contentType.indexOf('application/json') !== -1) {
                const data = await response.json();
                if (data.success) {
                    appModal.hide();
                    window.location.reload();
                    return;
                }
            }
            setModalBody(await response.text());
        } finally {
            if (submit) submit.disabled = false;
        }
    });

    // Delete confirmation: <form class="confirm-form" data-confirm="Delete this item?">
    $(document).on('submit', 'form.confirm-form', function (e) {
        if (this.dataset.confirmed === 'true') return;
        e.preventDefault();
        pendingForm = this;
        document.getElementById('confirmMessage').textContent = this.getAttribute('data-confirm') || 'Are you sure?';
        confirmModal.show();
    });

    document.getElementById('confirmOk').addEventListener('click', function () {
        if (!pendingForm) return;
        pendingForm.dataset.confirmed = 'true';
        confirmModal.hide();
        pendingForm.submit();
    });
})();
