define(['durandal/http', 'durandal/app'], function (http, app) {

    return {
        displayName: "Mailbox",
        emails: ko.observableArray(),
        links: Array(),
        currentPage: ko.observable(),
        totalPages: ko.observable(),
        
        activate: function() {
            $this = this;
            $.getJSON('emails').success(function (data) {
                console.debug("Got emails from server");
                $this.emails(data.Emails);
                $this.links = data.Links;
                $this.currentPage(data.Page);
                $this.totalPages(data.Pages);
            }).error(function (e) {
                console.log(e);
            });
        },
        
        open: function (email) {
            email.viewUrl = 'views/email';
            app.showModal(email);
        }
    };
});