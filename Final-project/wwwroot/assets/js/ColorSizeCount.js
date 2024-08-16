document.addEventListener("DOMContentLoaded", function () {
    var colorSelect = document.getElementById("01");
    var sizeSelect = document.getElementById("02");
    var countInput = document.querySelector('input[name="Count"]');
    var colorSizeCountInput = document.querySelector('input[name="ColorSizeCount"]');

    function updateColorSizeCount() {
        var colorId = colorSelect.value;
        var sizeId = sizeSelect.value;
        var count = countInput.value;

        if (colorId && sizeId && count && count > 0) {
            var colorSizeCountValue = colorId + "-" + sizeId + "-" + count;

            if (colorSizeCountInput.value && colorSizeCountInput.value !== colorSizeCountValue) {
                colorSizeCountInput.value += "," + colorSizeCountValue;
            } else {
                colorSizeCountInput.value = colorSizeCountValue;
            }
        }
    }
    var addInput = document.getElementById("add-inp");
    addInput.addEventListener("click", updateColorSizeCount);

    var delInput = document.getElementById("del-inp");
    var colorSizeCountInput = document.querySelector('input[name="ColorSizeCount"]');

    delInput.addEventListener("click", function () {
        var colorSizeCountValue = colorSizeCountInput.value;

        if (colorSizeCountValue) {
            var lastIndex = colorSizeCountValue.lastIndexOf(",");
            if (lastIndex !== -1) {
                colorSizeCountInput.value = colorSizeCountValue.substring(0, lastIndex);
            } else {
                colorSizeCountInput.value = "";
            }
        }
    });
});