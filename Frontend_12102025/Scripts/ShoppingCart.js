// Tăng giảm sản phẩm
const quantityElement = document.querySelector('.quantity');
const btnPlus = document.querySelector('.plus');
const btnMinus = document.querySelector('.minus');
const subtotal = document.querySelector(".product-subtotal span");
const price = document.querySelector(".product-subtotal span");
const unitPrice = parseInt(price.textContent.replace(/\D/g, ''));

btnPlus.addEventListener("click", () => {
    let currentQuantity = parseInt(quantityElement.value) || 0;
    const newQuantity = currentQuantity + 1;

    quantityElement.value = newQuantity;
    const total = unitPrice * newQuantity;
    subtotal.textContent = total.toLocaleString('vi-VN') + 'đ';
});

btnMinus.addEventListener("click", () => {
    let currentQuantity = parseInt(quantityElement.value) || 0;

    if (currentQuantity > 1) {
        const newQuantity = currentQuantity - 1;
        quantityElement.value = newQuantity;
        const total = unitPrice * newQuantity;
        subtotal.textContent = total.toLocaleString('vi-VN') + 'đ';
    }
    else {
        quantityElement.value = 0;
        subtotal.textContent = '';
    }
});

// Button update cart
const btnUpdateCart = document.getElementById("button-update-cart");
const subtotalRightCol = document.getElementById("subtotal-right-column");
const total = document.getElementById("total");
btnUpdateCart.addEventListener("click", () => {
    subtotalRightCol.textContent = subtotal.textContent;
    total.textContent = subtotal.textContent;
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

    // check coupon nhập đúng và giá trị đơn hàng trên 500k
    if (couponCode == "BUY5") {
        if (currentSubtotal >= 500000) {

            // Cập nhật giảm giá 10%s
            discountTotal = Math.floor(currentSubtotal * 0.1);
            const finalTotal = currentSubtotal - discountTotal;

            // Cập nhật lại giá hiển thị
            total.textContent = finalTotal.toLocaleString('vi-VN') + 'đ';

            alert('Áp dụng mã giảm giá thành công! Giảm ' + discountTotal.toLocaleString('vi-VN') + 'đ');

            // Sau khi đã nhập coupon thì không cho nhập nữa
            couponInput.value = '';
            btnApplyCoupon.disabled = true;
        }
        else {
            alert('Mã giảm giá chỉ áp dụng cho hóa đơn trên 500.000đ');
        }
    }
    else if (couponCode === "") {
        alert('Vui lòng nhập mã giảm giá!');
    }
});