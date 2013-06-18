define(['durandal/http', 'durandal/app'], function (http, app) {
    var router = require('durandal/plugins/router');

    return {
        displayName: "Mailbox",
        emails: ko.observableArray([]),
        links: ko.observableArray([]),
        currentPageNumber: ko.observable(1),
        totalPages: ko.observable(1),

        activate: function (args) {
            var page = args.splat ? args.splat[0] : 1;
            this.getEmails(page);
        },

        open: function (email) {
            router.navigateTo('#/email/' + encodeURIComponent(email.Id), 'skip');
            email.viewUrl = 'views/email';
            app.showModal(email).then(function () {
                router.navigateBack();
            });
        },

        getEmails: function (page) {
            var that = this;
            router.navigateTo('#/mailbox/page/' + page, 'skip');
            http.get('emails/', {
                page: page,
                format: 'json'
            }).then(function (data) {
                setTimeout(function() {
                    that.emails(data.Emails);
                    console.log(that.emails().length);
                    that.currentPageNumber(data.Page);
                    that.totalPages(data.Pages);
                }, 500);

            });
        }
    };
});