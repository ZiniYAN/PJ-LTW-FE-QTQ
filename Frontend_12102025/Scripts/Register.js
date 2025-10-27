document.getElementById('registerForm').addEventListener('submit', function (e) {
    e.preventDefault();

    const email = document.getElementById("email");
    const phone = document.getElementById("phone");
    const password = document.getElementById('password').value;
    const confirmPassword = document.getElementById('confirmPassword').value;
    const username = document.getElementById("username");

    const noticeEmail = document.getElementById("notice-email");
    const noticePhone = document.getElementById("notice-phone");
    const noticePass = document.getElementById("notice-pass");
    const noticeConfirmPass = document.getElementById("notice-confirm-pass");
    const noticeUsername = document.getElementById("notice-username");
    const noticeDKSD = document.getElementById("notice-dksd");
    if (password !== confirmPassword) {
        noticeConfirmPass.textContent = "Mật khẩu không khớp";
        return false;
    }
    if (username.value.length < 5) {
        noticeUsername.innerHTML = "<p>Username phải nhiều hơn 5 kí tự</p>";
        noticeDKSD.innerHTML.style.color = "red";
        return false;
    }
    if (password.length < 8) {
        noticePass.innerHTML = "<p>Mật khẩu phải có nhiều hơn 8 kí tự</p>";
        noticeDKSD.innerHTML.style.color = "red";
        return false;
    }

    const terms = document.getElementById('terms').checked;
    if (!terms) {
        noticeDKSD.innerHTML = "<p>Bạn phải đồng ý với điều khoản sử dụng!</p>";
        noticeDKSD.innerHTML.style.color = "red";
        return false;
    }
});