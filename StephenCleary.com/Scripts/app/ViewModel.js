/// <reference path="/scripts/lib/jquery-1.7.1.js" />
/// <reference path="/scripts/lib/jquery.mobile-1.1.1.js" />
/// <reference path="/scripts/lib/knockout-2.1.0.debug.js" />
/// <reference path="/scripts/lib/moment.js" />
var my = my || {};
my.vm = (function () {
    var x = 'an',
        y = 'steph',
        email = 'mailto:' + y + 'en' + x + 'dm' + x + 'dy@' + 'gma' + 'il.' + 'com';
    var guidInput = ko.observable(''),
        decodeGuid = function (value, event) {
            var guid = $(event.currentTarget).attr('data-guid');
            if (guid)
                guidInput(guid);
            else
                guid = guidInput();
            guidDecoderResult({ message: 'Decoding GUID...' });
            $.ajax({
                url: 'http://stephenclearyapi.apphb.com/api/GuidDecoder',
                dataType: 'jsonp',
                cache: true,
                data: { guid: guidInput() },
                success: function (result) {
                    guidDecoderResult(result);
                },
                error: function (xhr, status, error) {
                    guidDecoderResult({ message: 'An error occurred.' });
                }
            });
        },
        guidDecoderResult = ko.observable({}),
        guidDecoderResults = ko.computed(function () {
            if (guidDecoderResult().message)
                return guidDecoderResult().message;
            if (!guidDecoderResult().value)
                return '';
            var ret = 'GUID: ' + guidDecoderResult().value + '\n';
            ret += 'Variant: ' + guidDecoderResult().variant + '\n';
            if (guidDecoderResult().version)
                ret += 'Version: ' + guidDecoderResult().version + '\n';
            if (guidDecoderResult().createTime) {
                var m = moment.utc(guidDecoderResult().createTime);
                m.local();
                ret += 'Created ' + m.fromNow() + ': ' + m.toString() + '\n';
            }
            if (guidDecoderResult().nodeIsMAC)
                ret += 'MAC Address: ' + guidDecoderResult().node + '\n';
            return ret;
        });

    var refreshTextAreas = function () {
        $('textarea').keyup();
    };

    var codeInput = ko.observable(''),
        formatCode = function () {
            codeFormatterResult('Formatting code...');
            $.ajax({
                url: 'http://stephenclearyapi.apphb.com/api/CSharpFormatter',
                dataType: 'jsonp',
                cache: true,
                data: { code: codeInput() },
                success: function (result) {
                    codeFormatterResult(result.result);
                    $('textarea').keyup();
                    $('#applications_CodeFormatter_result').select();
                },
                error: function (xhr, status, error) {
                    codeFormatterResult('An error occurred.');
                    $('textarea').keyup();
                }
            });
        },
        codeFormatterResult = ko.observable('');
    codeFormatterResult.subscribe(refreshTextAreas);

    return {
        email: email,
        guidInput: guidInput,
        decodeGuid: decodeGuid,
        guidDecoderResults: guidDecoderResults,
        codeInput: codeInput,
        formatCode: formatCode,
        codeFormatterResult: codeFormatterResult,
    };
})();