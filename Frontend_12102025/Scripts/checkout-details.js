document.addEventListener('DOMContentLoaded', function () {
    console.log('JavaScript loaded successfully!');


    //COUPON
    const couponToggle = document.querySelector('.coupon-toggle');
    const couponForm = document.querySelector('.coupon-form');

    if (couponToggle && couponForm) {
        couponToggle.addEventListener('click', function (event) {
            event.preventDefault();

            // Toggle visibility
            if (couponForm.style.display === 'block') {
                couponForm.style.display = 'none';
            } else {
                couponForm.style.display = 'block';
            }
        });
    }

   

    // VALIDATION SETUP 
    // Billing required fields
    const billingRequiredFields = [
        'first-name',
        'last-name',
        'country',
        'street-address',
        'town-city',
        'state',
        'zip-code',
        'email'
    ];

    // Shipping required fields
    const shippingRequiredFields = [
        'shipping-first-name',
        'shipping-last-name',
        'shipping-country',
        'shipping-street-address',
        'shipping-town-city',
        'shipping-state',
        'shipping-zip-code'
    ];

    // Thêm required attribute cho billing fields
    billingRequiredFields.forEach(fieldId => {
        const field = document.getElementById(fieldId);
        if (field) {
            field.setAttribute('required', 'required');
        }
    });

    // Function để thêm validation cho shipping fields
    function addShippingValidation() {
        shippingRequiredFields.forEach(fieldId => {
            const field = document.getElementById(fieldId);
            if (field) {
                field.setAttribute('required', 'required');
            }
        });
    }

    // Function để xóa validation cho shipping fields
    function removeShippingValidation() {
        shippingRequiredFields.forEach(fieldId => {
            const field = document.getElementById(fieldId);
            if (field) {
                field.removeAttribute('required');
                field.classList.remove('input-error');

                // Xóa error message nếu có
                const errorMsg = field.parentElement.querySelector('.error-message');
                if (errorMsg) {
                    errorMsg.remove();
                }
            }
        });
    }

    // VALIDATION FUNCTION 
    function validateForm() {
        let isValid = true;
        const errors = [];

        // Xóa error messages cũ
        document.querySelectorAll('.error-message').forEach(el => el.remove());
        document.querySelectorAll('.input-error').forEach(el => el.classList.remove('input-error'));

        // Get all fields to validate (billing + shipping if checked)
        let fieldsToValidate = [...billingRequiredFields];

        // Nếu shipping checkbox được check, thêm shipping fields vào validation
        if (shippingCheckbox && shippingCheckbox.checked) {
            fieldsToValidate = [...fieldsToValidate, ...shippingRequiredFields];
        }

        // Check tất cả required fields
        fieldsToValidate.forEach(fieldId => {
            const field = document.getElementById(fieldId);
            if (field && !field.value.trim()) {
                isValid = false;
                field.classList.add('input-error');

                // Tạo error message
                const errorMsg = document.createElement('span');
                errorMsg.className = 'error-message';
                errorMsg.textContent = 'không được để trống';
                field.parentElement.appendChild(errorMsg);

                if (errors.length === 0) {
                    errors.push(field);
                }
            }
        });

        // Validate email format
        const emailField = document.getElementById('email');
        if (emailField && emailField.value) {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(emailField.value)) {
                isValid = false;
                emailField.classList.add('input-error');

                // Xóa error message cũ nếu có
                const existingError = emailField.parentElement.querySelector('.error-message');
                if (existingError) {
                    existingError.remove();
                }

                const errorMsg = document.createElement('span');
                errorMsg.className = 'error-message';
                errorMsg.textContent = 'Hãy nhập chính xác mail';
                emailField.parentElement.appendChild(errorMsg);

                if (errors.length === 0) {
                    errors.push(emailField);
                }
            }
        }

        // Scroll to first error
        if (!isValid && errors.length > 0) {
            errors[0].scrollIntoView({ behavior: 'smooth', block: 'center' });
            errors[0].focus();
        }

        return isValid;
    }

    // PAYMENT BUTTONS 
    const paypalButton = document.querySelector('.paypal-button');
    const orderButton = document.querySelector('.order-button');

    // Validate khi click PayPal button
    if (paypalButton) {
        paypalButton.addEventListener('click', function (e) {
            e.preventDefault();

            if (validateForm()) {
                // Nếu validation pass, cho phép redirect
                window.location.href = this.getAttribute('href');
            }
        });
    }

    // Validate khi click Order button
    if (orderButton) {
        orderButton.addEventListener('click', function (e) {
            e.preventDefault();

            if (validateForm()) {
                window.location.href = this.getAttribute('href');
            }
        });
    }

    // REMOVE ERROR ON INPUT
    document.querySelectorAll('input, select, textarea').forEach(field => {
        field.addEventListener('input', function () {
            if (this.classList.contains('input-error')) {
                this.classList.remove('input-error');
                const errorMsg = this.parentElement.querySelector('.error-message');
                if (errorMsg) {
                    errorMsg.remove();
                }
            }
        });
    });
});
//SHIP TO DIFFERENT ADDRESS 
const shippingCheckbox = document.getElementById('ship-different-address');
const shippingForm = document.querySelector('.ship-different-address-form');

if (shippingCheckbox && shippingForm) {
    shippingCheckbox.addEventListener('change', function () {
        if (this.checked) {
            shippingForm.style.display = 'block';
            // Thêm required cho shipping fields khi checked
            addShippingValidation();
        } else {
            shippingForm.style.display = 'none';
            // Xóa required và errors khi unchecked
            removeShippingValidation();
        }
    });
}
