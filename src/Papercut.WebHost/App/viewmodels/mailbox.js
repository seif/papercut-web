define(['durandal/http', 'durandal/app'], function (http, app) {
    var router = require('durandal/plugins/router');
    
    return {
        displayName: "Mailbox",
        emails: ko.observableArray(),
        links: ko.observableArray(),
        currentPage: ko.observable(),
        totalPages: ko.observable(),
        
        activate: function () {
            var that = this;
            $.getJSON('emails').success(function (data) {
                that.emails(data.Emails);
                that.links(data.Links);
                that.currentPage(data.Page);
                that.totalPages(data.Pages);
            }).error(function (e) {
                console.log(e);
            });
        },
        
        open: function (email) {
            router.navigateTo('#/email/' + email.Id, 'skip');
            email.viewUrl = 'views/email';
            app.showModal(email);
        }
    };
});