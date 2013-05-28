ko.bindingHandlers.date = {
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var value = valueAccessor();
        var date = moment(value, "YYYY-MM-DDTHH:mm:ss Z").fromNow();
        $(element).text(date);
    }
};