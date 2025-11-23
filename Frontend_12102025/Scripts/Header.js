// Cập nhật số lượng giỏ hàng
function updateCartCount() {
    $.ajax({
        url: '/Cart/GetCartCount',
        type: 'GET',
        success: function (response) {
            var cartBadge = $('#cart-count');
            if (response.count > 0) {
                cartBadge.text(response.count);
                cartBadge.show();
            } else {
                cartBadge.hide();
            }
        },
        error: function () {
            console.log('Không thể cập nhật số lượng giỏ hàng');
        }
    });
}

// Gọi hàm khi trang load
$(document).ready(function () {
    // Có thể gọi định kỳ nếu cần
     setInterval(updateCartCount, 5000); // Cập nhật mỗi 5 giây
});
