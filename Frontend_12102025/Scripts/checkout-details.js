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
