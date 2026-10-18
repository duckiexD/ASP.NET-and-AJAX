
// cart.js — добавление в корзину через AJAX

// Делегирование клика: работает и для динамически
// добавленных кнопок (после AJAX-поиска)
document.addEventListener('click', function (event) {
    const btn = event.target.closest('.add-to-cart');
    if (!btn) return;
    if (btn.disabled) return;

    const productId = btn.dataset.productId;
    if (!productId) return;

    event.preventDefault();
    addToCart(productId, btn);
});


// Основная функция
async function addToCart(productId, button) {
    const originalText = button.textContent;

    button.disabled = true;
    button.innerHTML =
        '<span class="spinner-border spinner-border-sm me-1"></span>Добавление...';

    try {
        const response = await fetch('/Catalog/AddToCart', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'Accept': 'application/json'
            },
            body: 'id=' + encodeURIComponent(productId)
        });

        if (!response.ok) throw new Error('HTTP ' + response.status);

        const data = await response.json();

        if (!data.success) {
            alert('Ошибка: ' + (data.message || 'неизвестная'));
            resetButton(button, originalText);
            return;
        }

        // Обновляем бейдж числом из ответа
        const badge = document.getElementById('cartBadge');
        if (badge) badge.textContent = data.cartCount;

        // Визуальный отклик
        button.innerHTML = 'Добавлено ✓';
        button.classList.remove('btn-primary');
        button.classList.add('btn-success');

        setTimeout(function () {
            resetButton(button, originalText);
        }, 2000);

    } catch (error) {
        console.error(error);
        button.textContent = 'Ошибка';
        button.disabled = false;

        setTimeout(function () {
            resetButton(button, originalText);
        }, 2000);
    }
}


function resetButton(button, originalText) {
    button.textContent = originalText;
    button.classList.remove('btn-success');
    button.classList.add('btn-primary');
    button.disabled = false;
}