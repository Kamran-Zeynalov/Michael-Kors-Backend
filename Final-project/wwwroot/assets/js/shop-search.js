const searchForm = document.querySelector('.searching');
const searchInput = searchForm.querySelector('.searching-input');


searchForm.addEventListener('submit', (e) => {
    e.preventDefault();
    const searchQuery = searchInput.value.trim();
    fetch(`/Shop/searchresult?search=${encodeURIComponent(searchQuery)}`)
        .then(response => response.text())
        .then(data => {
            const partialContainer = document.getElementById('partials');
            partialContainer.innerHTML = data;
        })
        .catch(error => console.error('Fetch hatası:', error));
});