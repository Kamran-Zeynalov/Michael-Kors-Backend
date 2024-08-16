function zoom(e) {
    var zoomer = e.currentTarget;
    e.offsetX ? offsetX = e.offsetX : offsetX = e.touches[0].pageX
    e.offsetY ? offsetY = e.offsetY : offsetX = e.touches[0].pageX
    x = offsetX / zoomer.offsetWidth * 100
    y = offsetY / zoomer.offsetHeight * 100
    zoomer.style.backgroundPosition = x + '% ' + y + '%';
}

document.addEventListener("DOMContentLoaded", function () {
    var links = document.querySelectorAll("a[href='#id1'], a[href='#id2'], a[href='#id3']");
    links.forEach(function (link) {
        link.addEventListener("click", function () {
            var imgs = document.querySelectorAll("img.active");
            imgs.forEach(function (img) {
                img.classList.remove("active");
            });

            var img = link.querySelector("img");
            img.classList.add("active");
        });
    });
});

document.addEventListener('DOMContentLoaded', function () {
    const stars = document.querySelectorAll('.stars-icon i');
    const ratingInput = document.querySelector('input[name="Rating"]');

    stars.forEach((star, index) => {
        star.addEventListener('mouseenter', function () {
            stars.forEach((s, i) => {
                s.classList.remove('hovered');
                if (i <= index) {
                    s.classList.add('hovered');
                }
            });
        });

        star.addEventListener('mouseleave', function () {
            stars.forEach((s) => {
                s.classList.remove('hovered');
            });
        });

        star.addEventListener('click', function () {
            stars.forEach((s, i) => {
                s.classList.remove('selected');
                if (i <= index) {
                    s.classList.add('selected');
                }
            });

            ratingInput.value = index + 1;
        });
    });
});