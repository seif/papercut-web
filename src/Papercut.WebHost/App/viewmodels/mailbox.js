define(['plugins/http', 'durandal/app', 'momentDateBinding'], function (http, app) {
	var router = require('plugins/router');
	var ko = require('knockout');
    
	return {
		displayName: "Mailbox",
		emails: ko.observableArray([]),
		links: ko.observableArray([]),
		currentPageNumber: ko.observable(1),
		totalPages: ko.observable(1),

		activate: function (requestPage) {
		    var page = requestPage || 1;
		    return this.getEmails(page);
		},

		open: function (email) {
			router.navigate('email/' + encodeURIComponent(email.Id), false);
			email.viewUrl = 'views/email';
			app.showDialog(email).then(function() {
			    router.navigateBack();
			});
		},

		getEmails: function (page) {
			var that = this;
            
			router.navigate('mailbox/' + page, false);

		    var url = 'emails/?format=json&page=' + page;

			//Return a promise so durandal gets all the data from the async response. Ref: http://stackoverflow.com/questions/15083516/how-to-use-observables-in-durandal
			var promise = $.getJSON(url).success(function (data) {
				that.emails(data.Emails);
				that.currentPageNumber(data.Page);
				that.totalPages(data.Pages);
			});

			return promise;
		}
	};
});