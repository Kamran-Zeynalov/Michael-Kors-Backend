$(document).ready(function () {
  $(".cs .text-left").on("click", function () {
    toggleAnswer(this);
  });
});

function toggleAnswer(element) {
  var answer = $(element).parent().find(".answer");
  var plusIcon = $(element).parent().find(".plus i");

  answer.slideToggle(500);

  if (plusIcon.hasClass("bi-plus")) {
    plusIcon.removeClass("bi-plus").addClass("bi-dash");
  } else {
    plusIcon.removeClass("bi-dash").addClass("bi-plus");
  }
}

$(".head-plus").click(function () {
  var $ul = $(this).next("ul");
  $ul.slideToggle(function () {
    var $p = $(this).prev(".head-plus").find("p");
    if ($ul.is(":visible")) {
      $p.html("-");
    } else {
      $p.html("+");
    }
  });
});

$(".my-menu").click(function () {
  var $ul = $(this).next(".left-side");
  $ul.slideToggle(function () {
    var $p = $(this).prev(".my-menu").find("div p");
    if ($ul.is(":visible")) {
      $p.html("-");
    } else {
      $p.html("+");
    }
  });
});