document.addEventListener('DOMContentLoaded', function() {

    // Toggle for the Coupon Form
    const couponToggle = document.querySelector('.coupon-toggle');
    const couponForm = document.querySelector('.coupon-form');

    if (couponToggle)
    {
        couponToggle.addEventListener('click', function(event) {
            event.preventDefault(); // Prevent the link from navigating
            // Toggle visibility of the coupon form
            if (couponForm.style.display === 'block')
            {
                couponForm.style.display = 'none';
            }
            else
            {
                couponForm.style.display = 'block';
            }
        });
    }

    // Toggle for the "Ship to a different address" Form
    const shippingCheckbox = document.getElementById('ship-different-address');
    const shippingForm = document.querySelector('.ship-different-address-form');

    if (shippingCheckbox)
    {
        shippingCheckbox.addEventListener('change', function() {
            // Show or hide the form based on whether the checkbox is checked
            if (this.checked) {
                    shippingForm.style.display = 'block';
            } else
            {
                    shippingForm.style.display = 'none';
                }
                });
        }

    });


document.addEventListener('DOMContentLoaded', function () {
    // Lấy form và payment buttons
    const form = document.querySelector('.billing-details form');
    const paypalButton = document.querySelector('.paypal-button');
    const orderButton = document.querySelector('.order-button');

    // Validation function
    function validateForm() {
        let isValid = true;
        const requiredFields = form.querySelectorAll('input[required], select[required]');

        // Xóa error messages cũ
        document.querySelectorAll('.error-message').forEach(el => el.remove());
        document.querySelectorAll('.input-error').forEach(el => el.classList.remove('input-error'));

        requiredFields.forEach(field => {
            if (!field.value.trim()) {
                isValid = false;

                // Thêm class error cho input
                field.classList.add('input-error');

                // Tạo error message
                const errorMsg = document.createElement('span');
                errorMsg.className = 'error-message';
                errorMsg.textContent = 'This field is required';
                errorMsg.style.color = 'red';
                errorMsg.style.fontSize = '12px';
                errorMsg.style.display = 'block';
                errorMsg.style.marginTop = '5px';

                // Thêm error message sau input
                field.parentElement.appendChild(errorMsg);
            }
        });

        // Validate email format
        const emailField = document.getElementById('email');
        if (emailField && emailField.value) {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(emailField.value)) {
                isValid = false;
                emailField.classList.add('input-error');

                const errorMsg = document.createElement('span');
                errorMsg.className = 'error-message';
                errorMsg.textContent = 'Please enter a valid email address';
                errorMsg.style.color = 'red';
                errorMsg.style.fontSize = '12px';
                errorMsg.style.display = 'block';
                errorMsg.style.marginTop = '5px';

                emailField.parentElement.appendChild(errorMsg);
            }
        }

        return isValid;
    }

    // Validate khi click PayPal button
    if (paypalButton) {
        paypalButton.addEventListener('click', function (e) {
            e.preventDefault();

            if (validateForm()) {
                // Nếu validation pass, redirect hoặc submit
                window.location.href = '/Home/OrderComplete';
            } else {
                // Scroll to first error
                const firstError = document.querySelector('.input-error');
                if (firstError) {
                    firstError.scrollIntoView({ behavior: 'smooth', block: 'center' });
                }
            }
        });
    }

    // Validate khi click Order button
    if (orderButton) {
        orderButton.addEventListener('click', function (e) {
            e.preventDefault();

            if (validateForm()) {
                window.location.href = '/Home/OrderComplete';
            } else {
                const firstError = document.querySelector('.input-error');
                if (firstError) {
                    firstError.scrollIntoView({ behavior: 'smooth', block: 'center' });
                }
            }
        });
    }

    // Remove error khi user bắt đầu nhập
    form.addEventListener('input', function (e) {
        if (e.target.classList.contains('input-error')) {
            e.target.classList.remove('input-error');
            const errorMsg = e.target.parentElement.querySelector('.error-message');
            if (errorMsg) {
                errorMsg.remove();
            }
        }
    });
});
