document.addEventListener('DOMContentLoaded', function () {
    console.log('Detail page loaded!');

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
        const oldNotification = document.querySelector('.detail-notification');
        if (oldNotification) {
            oldNotification.remove();
        }

        const notification = document.createElement('div');
        notification.className = 'detail-notification';

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

        if (!document.querySelector('#detail-notification-style')) {
            const style = document.createElement('style');
            style.id = 'detail-notification-style';
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

    // Lấy thông tin sản phẩm từ DOM
    const productNameElement = document.querySelector('.Book-info h2');
    const priceElement = document.querySelector('.Book-info .price');
    const imageElement = document.querySelector('.Book-image img');

    if (!productNameElement || !priceElement || !imageElement) {
        console.error('Product info elements not found');
        return;
    }

    const productName = productNameElement.textContent.trim();
    const priceText = priceElement.textContent.trim();
    const productPrice = parseInt(priceText.replace(/\D/g, ''));
    const productImage = imageElement.getAttribute('src');

    console.log('Product info:', { productName, productPrice, productImage });

    // ============== XỬ LÝ NÚT THÊM VÀO GIỎ ==============
    const btnAddToCart = document.querySelector('.btn-cart');

    if (btnAddToCart) {
        btnAddToCart.addEventListener('click', function (e) {
            e.preventDefault();

            console.log('Adding to cart...');

            // Thêm vào cart
            const cartData = addToCart(productName, productPrice, productImage);

            // Hiển thị notification
            showNotification(`Đã thêm "${productName}" vào giỏ hàng! (SL: ${cartData.quantity})`, 'success');
        });
    }

    // ============== XỬ LÝ NÚT MUA NGAY ==============
    const btnBuyNow = document.querySelector('.btn-buy');

    if (btnBuyNow) {
        btnBuyNow.addEventListener('click', function (e) {
            e.preventDefault();

            console.log('Buy now...');

            // Thêm vào cart
            addToCart(productName, productPrice, productImage);

            // Redirect tới Checkout Details
            window.location.href = '/Home/CheckoutDetails';
        });
    }

    // ============== XỬ LÝ RELATED ITEMS (Optional) ==============
    const relatedButtons = document.querySelectorAll('.related-item button');

    relatedButtons.forEach((button, index) => {
        button.addEventListener('click', function (e) {
            e.preventDefault();

            // Có thể redirect tới detail page của sản phẩm đó
            // hoặc thêm logic khác tùy ý
            console.log('Related item clicked:', index);

            // Ví dụ: Reload page (trong thực tế sẽ redirect tới detail page khác)
            // window.location.href = '/Home/Detail?id=' + productId;
        });
    });
});
