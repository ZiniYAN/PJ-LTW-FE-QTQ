//  CART BADGE UPDATE FUNCTION 
function updateCartBadge() {
    const cartBadge = document.getElementById('cart-count');

    if (!cartBadge) {
        console.log('Cart badge element not found');
        return;
    }

    // Lấy cart data từ localStorage
    const savedCart = localStorage.getItem('cartData');

    if (savedCart) {
        const cartData = JSON.parse(savedCart);
        const quantity = cartData.quantity || 0;

        // Update badge
        cartBadge.textContent = quantity;

        // Show/hide badge dựa trên quantity
        if (quantity > 0) {
            cartBadge.classList.remove('hidden');
        } else {
            cartBadge.classList.add('hidden');
        }

        console.log('Cart badge updated:', quantity);
    } else {
        // Không có cart, hide badge
        cartBadge.textContent = '0';
        cartBadge.classList.add('hidden');
    }
}

// Update cart badge khi page load
document.addEventListener('DOMContentLoaded', function () {
    updateCartBadge();

    // Listen for storage changes từ other tabs/windows
    window.addEventListener('storage', function (e) {
        if (e.key === 'cartData') {
            updateCartBadge();
        }
    });
});

// Custom event để trigger update từ same page
window.addEventListener('cartUpdated', function () {
    updateCartBadge();
});
