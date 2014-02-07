define(['durandal/http', 'durandal/app'], function (http, app) {

	return {
		displayName: 'Email',

		Id: ko.observable(),
		From: ko.observable(),
		To: ko.observable(),
		Subject: ko.observable(),
		Body: ko.observable(),
		Date: ko.observable(),
		activate: function (params) {
			var self = this;
			var url = 'email/' + encodeURIComponent(params.id);

			//Return a promise so durandal gets all the data from the async response. Ref: http://stackoverflow.com/questions/15083516/how-to-use-observables-in-durandal
			var promise = $.getJSON(url).success(function (data) {
				self.Id(data.Id);
				self.From(data.From);
				self.To(data.To);
				self.Subject(data.Subject);
				self.Body(data.Body);
				self.Date(data.Date);
			}).error(function (e) {
				console.log(e);
			});

			return promise;
		}
	};
});
   