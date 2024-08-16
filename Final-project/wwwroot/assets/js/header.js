document.addEventListener("DOMContentLoaded", function () {
    var clickDivs = document.querySelectorAll(".categories .hoverr");
    var menuX = document.querySelector('.menu-x');
    var menu = document.querySelector('.menum');
    var srch = document.querySelector('.srch input');
    var srch2 = document.querySelector('.srch2 input');
    var searchDrop = document.querySelector('.search-drop');
    var sideMenu = document.querySelector('.side-menu');
    var menuLi = document.querySelectorAll('.menu-category .menu-li');
    var slideDown = document.querySelectorAll('.menu-category .slideDownJ');

    clickDivs.forEach(function (clickDiv) {
        clickDiv.addEventListener("click", function () {
            var targetDiv = clickDiv.querySelector(".dropDown");
            var allTargetDivs = document.querySelectorAll(".dropDown");
            allTargetDivs.forEach(function (div) {
                if (div !== targetDiv && window.getComputedStyle(div).display !== "none") {
                    div.classList.remove("animate__slideInLeft");
                    div.classList.add("animate__slideOutRight");
                    setTimeout(function () {
                        div.style.display = 'none';
                    }, 900);
                }
            });

            if (window.getComputedStyle(targetDiv).display === "none") {
                targetDiv.classList.add("animate__slideInLeft");
                targetDiv.classList.remove("animate__slideOutRight");
                targetDiv.style.display = "block";

            } else {
                targetDiv.classList.remove("animate__slideInLeft");
                targetDiv.classList.add("animate__slideOutRight");
                setTimeout(function () {
                    targetDiv.style.display = 'none';
                }, 900);
            }
        });


        menu.addEventListener('click', function () {
            menu.style.display = 'none';
            menuX.style.display = 'inline-block';
            sideMenu.style.display = 'block';
        });
        menuX.addEventListener('click', function () {
            menu.style.display = 'inline-block';
            menuX.style.display = 'none';
            sideMenu.style.display = 'none';
        });
    });

    $(menuLi).click(function () {
        $(this).parent().find(slideDown).slideToggle();
        if (($(this).find(".menu-up").css("display") === "none")) {
            $(this).find(".menu-up").css("display", "inline-block");
            $(this).find(".menu-down").css("display", "none");
        }
        else {
            $(this).find(".menu-down").css("display", "inline-block");
            $(this).find(".menu-up").css("display", "none");

        }
    });

});