// Filter orders by status
document.getElementById('statusFilter').addEventListener('change', function () {
    const status = this.value;
    const orders = document.querySelectorAll('.order-card');
    let visibleCount = 0;

    orders.forEach(order => {
        if (status === 'all' || order.dataset.status === status) {
            order.style.display = 'block';
            visibleCount++;
        } else {
            order.style.display = 'none';
        }
    });

    document.querySelector('.no-orders').style.display = visibleCount === 0 ? 'block' : 'none';
});

// Search orders
document.getElementById('searchOrder').addEventListener('input', function () {
    const searchTerm = this.value.toLowerCase();
    const orders = document.querySelectorAll('.order-card');
    let visibleCount = 0;

    orders.forEach(order => {
        const orderNumber = order.querySelector('.order-number .value').textContent.toLowerCase();
        const itemNames = Array.from(order.querySelectorAll('.item-name')).map(el => el.textContent.toLowerCase()).join(' ');

        if (orderNumber.includes(searchTerm) || itemNames.includes(searchTerm)) {
            order.style.display = 'block';
            visibleCount++;
        } else {
            order.style.display = 'none';
        }
    });

    document.querySelector('.no-orders').style.display = visibleCount === 0 ? 'block' : 'none';
});