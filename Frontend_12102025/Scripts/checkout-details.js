document.addEventListener('DOMContentLoaded', function () {
    console.log('JavaScript loaded successfully!');

    // LOAD CART DATA FROM LOCALSTORAGE 
    const savedCart = localStorage.getItem('cartData');
    if (savedCart) {
        const cartData = JSON.parse(savedCart);
        console.log('Loaded cart data from localStorage:', cartData);

        // Update product info
        const productNameEl = document.querySelector('.product-name');
        const productImage = document.querySelector('.product-thumbnail');
        const productSubtotal = document.querySelector('.product-subtotal');
        const subtotalRow = document.querySelector('.order-summary tbody tr:nth-child(2) td');
        const shippingRow = document.querySelector('.order-summary tbody tr:nth-child(3)');
        const totalRow = document.querySelector('.total-row td');

        if (productNameEl && cartData.productName) {
            productNameEl.textContent = cartData.productName + ' × ' + cartData.quantity;
        }

        if (productImage && cartData.productImage) {
            productImage.src = cartData.productImage;
        }

        if (productSubtotal && cartData.subtotal) {
            productSubtotal.textContent = cartData.subtotal;
        }

        if (subtotalRow && cartData.subtotal) {
            subtotalRow.textContent = cartData.subtotal;
        }

        if (totalRow && cartData.total) {
            totalRow.textContent = cartData.total;
        }

        // Nếu có coupon từ shopping cart
        if (cartData.couponCode && cartData.discount) {
            const couponInput = document.getElementById('coupon-code');
            const btnApplyCoupon = document.getElementById('button-apply-coupon');

            // Disable coupon input
            if (couponInput) {
                couponInput.value = cartData.couponCode;
                couponInput.disabled = true;
            }

            if (btnApplyCoupon) {
                btnApplyCoupon.disabled = true;
                btnApplyCoupon.textContent = 'Coupon đã áp dụng';
            }

            // Thêm discount row vào table
            if (shippingRow && !document.querySelector('.discount-row')) {
                const discountRow = document.createElement('tr');
                discountRow.className = 'discount-row';
                discountRow.innerHTML = `
                    <th style="color: #28a745;">Discount (${cartData.couponCode})</th>
                    <td style="color: #28a745;">-${cartData.discount.toLocaleString('vi-VN')}đ</td>
                `;
                shippingRow.parentNode.insertBefore(discountRow, shippingRow.nextSibling);
            }

            // Hiển thị success message
            const couponBanner = document.querySelector('.coupon-banner');
            if (couponBanner && !document.querySelector('.coupon-success-message')) {
                const appliedMessage = document.createElement('div');
                appliedMessage.className = 'coupon-success-message';
                appliedMessage.style.cssText = 'padding: 10px; background-color: #d4edda; border: 1px solid #c3e6cb; color: #155724; margin-top: 10px; border-radius: 4px;';
                appliedMessage.innerHTML = `
                    ✓ Coupon <strong>${cartData.couponCode}</strong> đã được áp dụng! 
                    Bạn được giảm <strong>${cartData.discount.toLocaleString('vi-VN')}đ</strong>
                `;
                couponBanner.appendChild(appliedMessage);
            }
        }
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

    // SHIP TO DIFFERENT ADDRESS 
    const shippingCheckbox = document.getElementById('ship-different-address');
    const shippingForm = document.querySelector('.ship-different-address-form');

    if (shippingCheckbox && shippingForm) {
        shippingCheckbox.addEventListener('change', function () {
            if (this.checked) {
                shippingForm.style.display = 'block';
                addShippingValidation();
            } else {
                shippingForm.style.display = 'none';
                removeShippingValidation();
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
                errorMsg.textContent = 'Không được để trống';
                errorMsg.style.color = 'red';
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
