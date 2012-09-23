/// <reference path="/scripts/lib/jquery-1.7.1.js" />
/// <reference path="/scripts/lib/jquery.mobile-1.1.1.js" />
/// <reference path="/scripts/lib/knockout-2.1.0.debug.js" />
/// <reference path="/scripts/lib/moment.js" />
$(function () {
    $('img').each(function () {
        $(this).attr('src', $(this).attr('data-src'));
    });

    ko.applyBindings(my.vm);

    $('input, textarea').placeholder();

    jQuery.fn.selectText = function () {
        var doc = document
            , element = this[0]
            , range, selection
        ;
        if (doc.body.createTextRange) {
            range = document.body.createTextRange();
            range.moveToElementText(element);
            range.select();
        } else if (window.getSelection) {
            selection = window.getSelection();
            range = document.createRange();
            range.selectNodeContents(element);
            selection.removeAllRanges();
            selection.addRange(range);
        }
    };
    $("pre code.csharp").click(function() {
        $(this).selectText();
    });

    $('#main').removeClass('hidden');
});
