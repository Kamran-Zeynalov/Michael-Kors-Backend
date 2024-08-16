$(document).ready(function () {
    $(".small-filter .filter").click(function () {
        $(".left-side").each(function () {
            if ($(this).css('display') === 'none') {
                $(".small-filter .filter p").html("Close");
                $(this).css('display', 'block');
                $(".pro-col").css('display', 'none');
                $("#Two-Shop").css('display', 'none');
            } else {
                $(".small-filter .filter p").html("Filter");
                $(this).css('display', 'none');
                $(".pro-col").css('display', 'block');
                $("#Two-Shop").css('display', 'block');
            }
        });
    });
});