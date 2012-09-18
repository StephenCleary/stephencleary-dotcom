$(function () {
    $('img').each(function () {
        $(this).attr('src', $(this).attr('data-src'));
    });

    ko.applyBindings(my.vm);

    $('input, textarea').placeholder();
    $('#main').removeClass('hidden');
});
