// Tăng giảm sản phẩm
const quantityElement = document.querySelector('.quantity');
const btnPlus = document.querySelector('.plus');
const btnMinus = document.querySelector('.minus');
const subtotal = document.querySelector(".product-subtotal span");
const price = document.querySelector(".product-price span");
const unitPrice = parseInt(price.textContent.replace(/\D/g, ''));

// Lấy thông tin sản phẩm
const productName = document.querySelector('.product-title')?.textContent || 'Product';
const productImage = document.querySelector('.product-image img')?.src || '';

// Function lưu cart vào localStorage
function saveCartToLocalStorage(quantity, subtotalValue, totalValue, couponCode = null, discount = 0) {
    const cartData = {
        productName: productName,
        productImage: productImage,
        unitPrice: unitPrice,
        quantity: quantity,
        subtotal: subtotalValue,
        total: totalValue,
        couponCode: couponCode,
        discount: discount,
        timestamp: new Date().getTime()
    };

    localStorage.setItem('cartData', JSON.stringify(cartData));
    console.log('Cart saved to localStorage:', cartData);
}

// Plus button
btnPlus.addEventListener("click", () => {
    let currentQuantity = parseInt(quantityElement.value) || 0;
    const newQuantity = currentQuantity + 1;

    quantityElement.value = newQuantity;
    const total = unitPrice * newQuantity;
    const formattedTotal = total.toLocaleString('vi-VN') + 'đ';
    subtotal.textContent = formattedTotal;

    // Lưu vào localStorage
    saveCartToLocalStorage(newQuantity, formattedTotal, formattedTotal);
});

// Minus button
btnMinus.addEventListener("click", () => {
    let currentQuantity = parseInt(quantityElement.value) || 0;

    if (currentQuantity > 1) {
        const newQuantity = currentQuantity - 1;
        quantityElement.value = newQuantity;
        const total = unitPrice * newQuantity;
        const formattedTotal = total.toLocaleString('vi-VN') + 'đ';
        subtotal.textContent = formattedTotal;

        // Lưu vào localStorage
        saveCartToLocalStorage(newQuantity, formattedTotal, formattedTotal);
    }
    else {
        quantityElement.value = 0;
        subtotal.textContent = '';

        // Lưu empty cart
        saveCartToLocalStorage(0, '', '');
    }
});

// Button update cart
const btnUpdateCart = document.getElementById("button-update-cart");
const subtotalRightCol = document.getElementById("subtotal-right-column");
const total = document.getElementById("total");

btnUpdateCart.addEventListener("click", () => {
    subtotalRightCol.textContent = subtotal.textContent;
    total.textContent = subtotal.textContent;

    // Lưu updated cart data
    const quantity = parseInt(quantityElement.value) || 0;
    const savedCart = JSON.parse(localStorage.getItem('cartData')) || {};

    saveCartToLocalStorage(
        quantity,
        subtotal.textContent,
        total.textContent,
        savedCart.couponCode || null,
        savedCart.discount || 0
    );

    alert('Cart updated and saved!');
});

// Update giá tiền khi thêm coupon
let discountTotal = 0;
const btnApplyCoupon = document.getElementById("apply-coupon");
const couponInput = document.getElementById("coupon-code");

btnApplyCoupon.addEventListener("click", (e) => {
    e.preventDefault();

    // Lấy coupon từ input
    const couponCode = couponInput.value.trim().toUpperCase();

    // Lấy ra total hiện tại
    const currentSubtotal = parseInt(subtotalRightCol.textContent.replace(/\D/g, ''));

    if (couponCode === "") {
        alert('Vui lòng nhập mã giảm giá!');
        return;
    }

    // check coupon nhập đúng và giá trị đơn hàng trên 500k
    if (couponCode === "BUY5") {
        if (currentSubtotal >= 500000) {
            // Cập nhật giảm giá 10%
            discountTotal = Math.floor(currentSubtotal * 0.1);
            const finalTotal = currentSubtotal - discountTotal;

            const formattedSubtotal = currentSubtotal.toLocaleString('vi-VN') + 'đ';
            const formattedTotal = finalTotal.toLocaleString('vi-VN') + 'đ';

            // Cập nhật lại giá hiển thị
            total.textContent = formattedTotal;

            // Lưu vào localStorage với coupon
            const quantity = parseInt(quantityElement.value) || 0;
            saveCartToLocalStorage(quantity, formattedSubtotal, formattedTotal, couponCode, discountTotal);

            alert('Áp dụng mã giảm giá thành công! Giảm ' + discountTotal.toLocaleString('vi-VN') + 'đ');

            // Disable coupon input
            couponInput.value = couponCode;
            couponInput.disabled = true;
            btnApplyCoupon.disabled = true;

            // Hiển thị discount message
            const couponSection = document.querySelector('.coupon-section');
            if (couponSection && !document.querySelector('.discount-display')) {
                const discountDisplay = document.createElement('p');
                discountDisplay.className = 'discount-display';
                discountDisplay.style.cssText = 'color: #28a745; font-size: 14px; margin-top: 10px; font-weight: bold;';
                discountDisplay.textContent = `✓ Đã áp dụng mã: ${couponCode} (Giảm ${discountTotal.toLocaleString('vi-VN')}đ)`;
                couponSection.appendChild(discountDisplay);
            }
        }
        else {
            alert('Mã giảm giá chỉ áp dụng cho hóa đơn trên 500.000đ');
        }
    } else {
        alert('Mã giảm giá không hợp lệ!');
    }
});

// Load cart từ localStorage khi page load
window.addEventListener('load', function () {
    const savedCart = localStorage.getItem('cartData');
    if (savedCart) {
        const cartData = JSON.parse(savedCart);
        console.log('Loaded cart from localStorage:', cartData);

        if (cartData.quantity > 0) {
            quantityElement.value = cartData.quantity;
            subtotal.textContent = cartData.subtotal;
            if (subtotalRightCol) subtotalRightCol.textContent = cartData.subtotal;
            if (total) total.textContent = cartData.total;

            // Restore coupon nếu có
            if (cartData.couponCode && cartData.discount) {
                couponInput.value = cartData.couponCode;
                couponInput.disabled = true;
                btnApplyCoupon.disabled = true;

                // Hiển thị discount message
                const couponSection = document.querySelector('.coupon-section');
                if (couponSection && !document.querySelector('.discount-display')) {
                    const discountDisplay = document.createElement('p');
                    discountDisplay.className = 'discount-display';
                    discountDisplay.style.cssText = 'color: #28a745; font-size: 14px; margin-top: 10px; font-weight: bold;';
                    discountDisplay.textContent = `✓ Đã áp dụng mã: ${cartData.couponCode} (Giảm ${cartData.discount.toLocaleString('vi-VN')}đ)`;
                    couponSection.appendChild(discountDisplay);
                }
            }
        }
    }
});
