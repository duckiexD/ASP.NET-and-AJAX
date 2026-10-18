// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Уровень 2 (Карточки товаров). Подсветка тег-фильтра
document.addEventListener('DOMContentLoaded', function () {
    const pills = document.querySelectorAll('.category-pill');

    pills.forEach(function (pill) {
        pill.addEventListener('click', function () {
            pills.forEach(p => {
                p.classList.remove('active', 'btn-primary');
                p.classList.add('btn-outline-secondary');
            });
            pill.classList.add('active', 'btn-primary');
            pill.classList.remove('btn-outline-secondary');
        });
    });
});

// Задание 5. Обновление бейджа корзины при загрузке страницы
document.addEventListener('DOMContentLoaded', async function () {
    await refreshCartBadge();
});

// Универсальная функция обновления бейджа.
async function refreshCartBadge() {
    const badge = document.getElementById('cartBadge');
    if (!badge) return;

    try {
        const response = await fetch('/Catalog/GetCartCount', {
            method: 'GET',
            headers: { 'Accept': 'application/json' }
        });

        if (!response.ok) throw new Error('HTTP ' + response.status);

        const data = await response.json();
        badge.textContent = (data.count ?? 0).toString();
    } catch (error) {
        console.error('Не удалось получить количество товаров:', error);
    }
}

// Делаем функцию доступной глобально — её будет вызывать cart.js
window.refreshCartBadge = refreshCartBadge;





