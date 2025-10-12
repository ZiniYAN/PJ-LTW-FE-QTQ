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
});
