define(['durandal/http', 'durandal/app'], function(http, app) {

    return {
        displayName: 'Email',
    
        Id: ko.observable(),
        From: ko.observable(),
        To: ko.observable(),
        Subject: ko.observable(),
        Body: ko.observable(),
        Date: ko.observable(),
        activate: function (params) {
            var $this = this;
            var url = 'email/' + encodeURIComponent(params.id);
            $.getJSON(url).success(function (data) {
                $this.Id(data.Id);
                $this.From(data.From);
                $this.To(data.To);
                $this.Subject(data.Subject);
                $this.Body(data.Body);
                $this.Date(data.Date);
            }).error(function (e) {
                console.log(e);
            });
        }
    };
});
   