document.getElementById('registerForm').addEventListener('submit', function (e) {
    e.preventDefault();

    const password = document.getElementById('password').value;
    const confirmPassword = document.getElementById('confirmPassword').value;

    if (password !== confirmPassword) {
        alert('Mật khẩu xác nhận không khớp!');
        return false;
    }

    if (password.length < 8) {
        alert('Mật khẩu phải có ít nhất 8 ký tự!');
        return false;
    }

    const terms = document.getElementById('terms').checked;
    if (!terms) {
        alert('Bạn phải đồng ý với điều khoản sử dụng!');
        return false;
    }
});