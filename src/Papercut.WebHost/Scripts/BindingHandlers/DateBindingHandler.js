define(['knockout', 'moment'], function (ko) {
    ko.bindingHandlers.date = {
        init: ko.bindingHandlers.text.init,
        update: function(element, valueAccessor) {
            var value = valueAccessor();
            var date = moment(value, "YYYY-MM-DDTHH:mm:ss Z").fromNow();
            $(element).text(date);
        }
    };
});
