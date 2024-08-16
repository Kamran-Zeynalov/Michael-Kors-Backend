document.addEventListener('DOMContentLoaded', function () {
  var shoppingBagLink = document.getElementById('shoppingBagLink');
  var myFavoritesLink = document.getElementById('myFavoritesLink');
  var favoritesDiv = document.getElementById('favoritesDiv');

  function updateActiveState() {
      var favoritesDivTop = favoritesDiv.getBoundingClientRect().top;

      if (favoritesDivTop < window.innerHeight / 2) {
          myFavoritesLink.parentElement.classList.add('active');
          shoppingBagLink.parentElement.classList.remove('active');
      } else {
          myFavoritesLink.parentElement.classList.remove('active');
          shoppingBagLink.parentElement.classList.add('active');
      }
  }

  myFavoritesLink.addEventListener('click', function (event) {
      event.preventDefault(); 

      myFavoritesLink.parentElement.classList.add('active');
      shoppingBagLink.parentElement.classList.remove('active');

      favoritesDiv.scrollIntoView({ behavior: 'smooth' });

      window.removeEventListener('scroll', updateActiveState);

      setTimeout(function () {
          window.addEventListener('scroll', updateActiveState);
      }, 500);
  });

  window.addEventListener('scroll', updateActiveState);

  updateActiveState();
});
