const searchForm = document.querySelector('.search');
const searchInput = searchForm.querySelector('.search-input');


searchForm.addEventListener('submit', (e) => {
    e.preventDefault();
    const searchQuery = searchInput.value.trim();
    fetch(`/Admin/Product/searchresult?search=${encodeURIComponent(searchQuery)}`)
        .then(response => response.text())
        .then(data => {
            const partialContainer = document.getElementById('partials');
            partialContainer.innerHTML = data;
        })
        .catch(error => console.error('Fetch hatası:', error));
});