document.addEventListener("DOMContentLoaded", function () {
    const toggleButtons = document.querySelectorAll(".toggle-password");

    toggleButtons.forEach(function (button) {
        button.addEventListener("click", function () {
            const passwordInput = button.parentElement.querySelector(".toggleable-password");
            const hideButton = button.parentElement.querySelector(".toggle-password-hide");
            const showButton = button.parentElement.querySelector(".toggle-password-show");

            if (passwordInput.type === "password") {
                passwordInput.type = "text";
                hideButton.classList.add("d-none");
                showButton.classList.remove("d-none");
            } else {
                passwordInput.type = "password";
                showButton.classList.add("d-none");
                hideButton.classList.remove("d-none");
            }
        });
    });
});