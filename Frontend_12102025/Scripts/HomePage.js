document.addEventListener('DOMContentLoaded', function () {
    const sliderSections = document.querySelectorAll('.slider-section');

    sliderSections.forEach(section => {
        const pages = section.querySelectorAll('.slides-page');
        const leftBtn = section.querySelector('.clickLeft');
        const rightBtn = section.querySelector('.clickRight');

        if (pages.length === 0) return;

        let currentIndex = 0;

        function updateSlider() {
            // Ẩn tất cả pages
            pages.forEach(page => page.classList.remove('show'));
            // Hiện page hiện tại
            pages[currentIndex].classList.add('show');

            // Update buttons
            leftBtn.disabled = currentIndex === 0;
            rightBtn.disabled = currentIndex === pages.length - 1;

            // Log để debug
            console.log(section.id + ' - Page ' + (currentIndex + 1) + ' (ID: ' + pages[currentIndex].id + ')');
        }

        leftBtn.addEventListener('click', function () {
            if (currentIndex > 0) {
                currentIndex--;
                updateSlider();
            }
        });

        rightBtn.addEventListener('click', function () {
            if (currentIndex < pages.length - 1) {
                currentIndex++;
                updateSlider();
            }
        });

        updateSlider();
    });

    // Function lấy cart từ localStorage
    function getCartData() {
        const savedCart = localStorage.getItem('cartData');
        return savedCart ? JSON.parse(savedCart) : null;
    }

    // Function hiển thị thông báo
    function showCartNotification(message) {
        // Xóa notification cũ nếu có
        const oldNotification = document.querySelector('.cart-notification-popup');
        if (oldNotification) {
            oldNotification.remove();
        }

        // Tạo notification mới
        const notification = document.createElement('div');
        notification.className = 'cart-notification-popup';
        notification.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            padding: 15px 25px;
            background-color: #28a745;
            color: white;
            border-radius: 5px;
            box-shadow: 0 4px 8px rgba(0,0,0,0.2);
            z-index: 10000;
            font-size: 14px;
            font-weight: 500;
            animation: slideIn 0.3s ease-out;
        `;
        notification.innerHTML = `<span style="margin-right: 8px;">✓</span>${message}`;

        // Add animation style if not exists
        if (!document.querySelector('#cart-notification-style')) {
            const style = document.createElement('style');
            style.id = 'cart-notification-style';
            style.textContent = `
                @keyframes slideIn {
                    from { transform: translateX(400px); opacity: 0; }
                    to { transform: translateX(0); opacity: 1; }
                }
            `;
            document.head.appendChild(style);
        }

        document.body.appendChild(notification);

        // Tự động xóa sau 2.5 giây
        setTimeout(() => {
            notification.style.transition = 'opacity 0.3s, transform 0.3s';
            notification.style.opacity = '0';
            notification.style.transform = 'translateX(400px)';
            setTimeout(() => notification.remove(), 300);
        }, 2500);
    }

    // Lấy tất cả Add to Cart buttons
    const addToCartButtons = document.querySelectorAll('#btn-addtocart');

    console.log('Found ' + addToCartButtons.length + ' Add to Cart buttons');

    addToCartButtons.forEach((button, index) => {
        button.addEventListener('click', function (e) {
            e.preventDefault();

            // Tìm container chứa sản phẩm
            const container = this.closest('.silde-container');

            if (!container) {
                console.error('Container not found for button', index);
                return;
            }

            // Lấy thông tin sản phẩm từ DOM
            const titleElement = container.querySelector('.title h3');
            const priceElement = container.querySelector('.price span');
            const imageElement = container.querySelector('img');

            if (!titleElement || !priceElement || !imageElement) {
                console.error('Product info elements not found');
                return;
            }

            const productName = titleElement.textContent.trim();
            const priceText = priceElement.textContent.trim();
            const productPrice = parseInt(priceText.replace(/\D/g, ''));
            const productImage = imageElement.getAttribute('src');

            console.log('Adding to cart:', { productName, productPrice, productImage });

            // Lấy cart hiện tại từ localStorage
            let cartData = getCartData();

            if (cartData) {
                // Nếu đã có cart, tăng quantity
                const newQuantity = cartData.quantity + 1;
                const newSubtotal = cartData.unitPrice * newQuantity;
                const formattedSubtotal = newSubtotal.toLocaleString('vi-VN') + 'đ';

                // Cập nhật cart data
                cartData.quantity = newQuantity;
                cartData.subtotal = formattedSubtotal;
                cartData.total = formattedSubtotal;
                cartData.timestamp = new Date().getTime();

                console.log('Cart updated - Quantity:', newQuantity);
            } else {
                // Tạo cart mới với quantity = 1
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

                console.log('New cart created');
            }

            // Lưu vào localStorage
            localStorage.setItem('cartData', JSON.stringify(cartData));

            // Hiển thị notification
            showCartNotification(`Đã thêm "${productName}" vào giỏ hàng! (SL: ${cartData.quantity})`);

            window.dispatchEvent(new Event('cartUpdated'));

            console.log('Cart saved to localStorage:', cartData);
        });
    });
});
