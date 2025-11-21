document.addEventListener("DOMContentLoaded", function () {
    // Lấy tất cả các slider areas
    const sliderAreas = document.querySelectorAll('.slider-area');

    sliderAreas.forEach(function (sliderArea) {
        const slider = sliderArea.querySelector('.list-container');
        const prevBtn = sliderArea.querySelector('.slider-prev');
        const nextBtn = sliderArea.querySelector('.slider-next');

        if (!slider || !prevBtn || !nextBtn) return;

        const items = slider.children;
        let perPage = 4;
        let currentPage = 0;

        function updatePerPage() {
            if (window.innerWidth <= 600) perPage = 2;
            else if (window.innerWidth <= 900) perPage = 3;
            else perPage = 4;
        }

        function showPage(page) {
            // Ẩn tất cả items
            for (let i = 0; i < items.length; i++) {
                items[i].style.display = "none";
            }

            // Hiển thị items của trang hiện tại
            const start = page * perPage;
            const end = Math.min((page + 1) * perPage, items.length);

            for (let i = start; i < end; i++) {
                items[i].style.display = "flex";
            }

            // Update button states
            prevBtn.disabled = (currentPage === 0);
            nextBtn.disabled = (end >= items.length);

            // Optional: Thêm class để style disabled
            if (prevBtn.disabled) {
                prevBtn.classList.add('disabled');
            } else {
                prevBtn.classList.remove('disabled');
            }

            if (nextBtn.disabled) {
                nextBtn.classList.add('disabled');
            } else {
                nextBtn.classList.remove('disabled');
            }
        }

        function nextPage() {
            if ((currentPage + 1) * perPage < items.length) {
                currentPage++;
                showPage(currentPage);
            }
        }

        function prevPage() {
            if (currentPage > 0) {
                currentPage--;
                showPage(currentPage);
            }
        }

        function onResize() {
            const oldPerPage = perPage;
            updatePerPage();
            if (oldPerPage !== perPage) {
                currentPage = 0; // Reset về trang đầu khi thay đổi
            }
            showPage(currentPage);
        }

        // Gắn events cho slider này
        prevBtn.addEventListener('click', prevPage);
        nextBtn.addEventListener('click', nextPage);
        window.addEventListener("resize", onResize);

        // Khởi tạo ban đầu
        updatePerPage();
        showPage(currentPage);
    });
});
