function writeId(type, id) {
    var parsedId = parseInt(id);
    var tag;
    if (type === 'size') {
        tag = document.getElementById("clickedSizeId");
        tag.innerText = type.toUpperCase() + " ID: " + parsedId;
    } else if (type === 'color') {
        tag = document.getElementById("clickedColorId");
        tag.innerText = type.toUpperCase() + " ID: " + parsedId;
    }
}


function setClickedIds(colorId, sizeId) {
    document.getElementById("clickedColorId").innerText = colorId;
    document.getElementById("clickedSizeId").innerText = sizeId;
}



document.querySelectorAll('.color-name').forEach(colorName => {
    colorName.addEventListener('click', function () {
        document.querySelectorAll('.size-button').forEach(button => {
            button.style.display = 'none';
        });
        const colorValue = this.getAttribute('data-color');
        document.querySelectorAll(`.size-button[data-color="${colorValue}"]`).forEach(button => {
            button.style.display = 'inline-block';
        });
    });
});

document.querySelectorAll('.size-button').forEach(sizeButton => {
    sizeButton.addEventListener('click', function () {
        document.querySelectorAll('.size-button').forEach(button => {
            button.classList.remove('activeSize');
            button.style.borderWidth = '0.7px';
        });
        this.classList.add('activeSize');
        this.style.borderWidth = '2px';
    });
});