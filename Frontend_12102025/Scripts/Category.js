document.addEventListener('DOMContentLoaded', function () {
    console.log('Category page loaded!');

    // Function lấy cart từ localStorage
    function getCartData() {
        const savedCart = localStorage.getItem('cartData');
        return savedCart ? JSON.parse(savedCart) : null;
    }

    // Function lưu cart vào localStorage
    function saveCartData(cartData) {
        localStorage.setItem('cartData', JSON.stringify(cartData));
        console.log('Cart saved:', cartData);

        // Trigger event để update cart badge
        window.dispatchEvent(new Event('cartUpdated'));
    }

    // Function thêm sản phẩm vào cart
    function addToCart(productName, productPrice, productImage) {
        let cartData = getCartData();

        if (cartData) {
            // Nếu đã có cart, tăng quantity
            const newQuantity = cartData.quantity + 1;
            const newSubtotal = cartData.unitPrice * newQuantity;
            const formattedSubtotal = newSubtotal.toLocaleString('vi-VN') + 'đ';

            cartData.quantity = newQuantity;
            cartData.subtotal = formattedSubtotal;
            cartData.total = formattedSubtotal;
            cartData.timestamp = new Date().getTime();
        } else {
            // Tạo cart mới
            const formattedPrice = productPrice.toLocaleString('vi-VN') + 'đ';

            cartData = {
                productName: productName,
                productImage: productImage,
                unitPrice: productPrice,
                quantity: 1,
                subtotal: formattedPrice,
                total: formattedPrice,
                couponCode: null,
                discount: 0,
                timestamp: new Date().getTime()
            };
        }

        saveCartData(cartData);
        return cartData;
    }

    // Function hiển thị notification
    function showNotification(message, type = 'success') {
        const oldNotification = document.querySelector('.category-notification');
        if (oldNotification) {
            oldNotification.remove();
        }

        const notification = document.createElement('div');
        notification.className = 'category-notification';

        const bgColor = type === 'success' ? '#28a745' : '#dc3545';

        notification.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            padding: 15px 25px;
            background-color: ${bgColor};
            color: white;
            border-radius: 5px;
            box-shadow: 0 4px 8px rgba(0,0,0,0.2);
            z-index: 10000;
            font-size: 14px;
            font-weight: 500;
            animation: slideIn 0.3s ease-out;
        `;
        notification.innerHTML = `<span style="margin-right: 8px;">✓</span>${message}`;

        if (!document.querySelector('#notification-style')) {
            const style = document.createElement('style');
            style.id = 'notification-style';
            style.textContent = `
                @keyframes slideIn {
                    from { transform: translateX(400px); opacity: 0; }
                    to { transform: translateX(0); opacity: 1; }
                }
            `;
            document.head.appendChild(style);
        }

        document.body.appendChild(notification);

        setTimeout(() => {
            notification.style.transition = 'opacity 0.3s, transform 0.3s';
            notification.style.opacity = '0';
            notification.style.transform = 'translateX(400px)';
            setTimeout(() => notification.remove(), 300);
        }, 2500);
    }

    // Lấy tất cả buttons
    const addToCartButtons = document.querySelectorAll('.btn-secondary'); // Button giỏ hàng
    const buyNowButtons = document.querySelectorAll('.btn-primary'); // Button Mua ngay

    console.log('Found', addToCartButtons.length, 'Add to Cart buttons');
    console.log('Found', buyNowButtons.length, 'Buy Now buttons');

    // ============== XỬ LÝ NÚT GIỎ HÀNG (🛒) ==============
    addToCartButtons.forEach((button, index) => {
        button.addEventListener('click', function (e) {
            e.preventDefault();

            // Tìm Book card chứa button này
            const BookCard = this.closest('.Book-card');

            if (!BookCard) {
                console.error('Book card not found for button', index);
                return;
            }

            // Lấy thông tin sản phẩm
            const titleElement = BookCard.querySelector('.title');
            const priceElement = BookCard.querySelector('.Book-price');
            const imageElement = BookCard.querySelector('.Book-image img');

            if (!titleElement || !priceElement || !imageElement) {
                console.error('Product info not found');
                return;
            }

            const productName = titleElement.textContent.trim();
            const priceText = priceElement.textContent.trim();
            const productPrice = parseInt(priceText.replace(/\D/g, ''));
            const productImage = imageElement.getAttribute('src');

            console.log('Adding to cart:', { productName, productPrice, productImage });

            // Thêm vào cart
            const cartData = addToCart(productName, productPrice, productImage);

            // Hiển thị notification
            showNotification(`Đã thêm "${productName}" vào giỏ hàng! (SL: ${cartData.quantity})`, 'success');
        });
    });

    // ============== XỬ LÝ NÚT MUA NGAY ==============
    buyNowButtons.forEach((button, index) => {
        button.addEventListener('click', function (e) {
            e.preventDefault();

            // Tìm Book card chứa button này
            const BookCard = this.closest('.Book-card');

            if (!BookCard) {
                console.error('Book card not found for button', index);
                return;
            }

            // Lấy thông tin sản phẩm
            const titleElement = BookCard.querySelector('.title');
            const priceElement = BookCard.querySelector('.Book-price');
            const imageElement = BookCard.querySelector('.Book-image img');

            if (!titleElement || !priceElement || !imageElement) {
                console.error('Product info not found');
                return;
            }

            const productName = titleElement.textContent.trim();
            const priceText = priceElement.textContent.trim();
            const productPrice = parseInt(priceText.replace(/\D/g, ''));
            const productImage = imageElement.getAttribute('src');

            console.log('Buy now:', { productName, productPrice, productImage });

            // Thêm vào cart
            addToCart(productName, productPrice, productImage);

            // Redirect tới Checkout Details page
            window.location.href = '/Home/CheckoutDetails';
        });
    });
});
