$(document).ready(function () {
    $("#profileEditLink, #editButton").click(function (e) {
        e.preventDefault();
        $(".edit").show();
        $(".changeP").hide();
    });

    $("#changePasswordLink").click(function (e) {
        e.preventDefault();
        $(".changeP").show();
        $(".edit").hide();
    });

    $("#cancelButton").click(function () {
        $(".edit").hide();
    });

    $("#cancelChangePButton").click(function () {
        $(".changeP").hide();
    });
});