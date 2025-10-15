window.carouselInterop = {
    initialize: function (carouselSelector, interval) {
        const carousel = document.querySelector(carouselSelector);
        if (!carousel) return;

        const carouselInstance = bootstrap.Carousel.getOrCreateInstance(carousel, {
            interval: interval,
            ride: 'carousel'
        });

        function resizeImages() {
            const footer = document.querySelector('.app-footer');
            const footerHeight = footer ? footer.offsetHeight : 0;

            const containerWidth = carousel.offsetWidth;
            const containerHeight = window.innerHeight - footerHeight;

            carousel.style.height = containerHeight + 'px';

            const images = carousel.querySelectorAll('.carousel-item img');
            images.forEach(img => {
                if (!img.complete) {
                    img.onload = resizeImages;
                    return;
                }

                const imgRatio = img.naturalWidth / img.naturalHeight;
                const containerRatio = containerWidth / containerHeight;

                if (imgRatio > containerRatio) {
                    img.style.width = containerWidth + 'px';
                    img.style.height = 'auto';
                } else {
                    img.style.width = 'auto';
                    img.style.height = containerHeight + 'px';
                }

                img.style.objectFit = 'contain';
                img.style.display = 'block';
                img.style.margin = 'auto';
            });
        }

        setTimeout(() => resizeImages(), 50);
        window.addEventListener('resize', () => setTimeout(resizeImages, 50));
        carousel.addEventListener('slid.bs.carousel', () => setTimeout(resizeImages, 50));
    }
};
