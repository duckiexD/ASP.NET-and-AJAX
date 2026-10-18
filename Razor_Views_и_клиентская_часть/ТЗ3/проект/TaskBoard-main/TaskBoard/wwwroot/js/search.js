// ============================================================
// search.js — live-поиск (подключён только на /catalog)
// ============================================================

document.addEventListener('DOMContentLoaded', function () {
    const input = document.getElementById('searchInput');
    const grid = document.getElementById('catalogGrid');

    if (!input || !grid) return;

    let timeoutId;

    input.addEventListener('input', function () {
        clearTimeout(timeoutId);
        const query = input.value.trim();

        if (query.length === 0) {
            window.location.reload();
            return;
        }

        timeoutId = setTimeout(function () {
            searchProducts(query, grid);
        }, 300);
    });
});

async function searchProducts(query, grid) {
    // 1. Спиннер
    grid.innerHTML =
        '<div class="text-center py-5 w-100">' +
        '  <div class="spinner-border text-primary" role="status"></div>' +
        '</div>';

    try {
        // 2. Запрос к серверу
        const url = '/Catalog/Search?query=' + encodeURIComponent(query);
        const response = await fetch(url, {
            headers: { 'Accept': 'text/html' }
        });

        if (!response.ok) throw new Error('HTTP ' + response.status);
        // 3. HTML-фрагмент
        const html = await response.text();

        // 4. Вставляем в сетку
        if (!html.trim()) {
            grid.innerHTML =
                '<div class="alert alert-info w-100">Ничего не найдено</div>';
        } else {
            grid.innerHTML = html;
        }
    } catch (error) {
        grid.innerHTML =
            '<div class="alert alert-danger w-100">Ошибка поиска</div>';
        console.error(error);
    }
}