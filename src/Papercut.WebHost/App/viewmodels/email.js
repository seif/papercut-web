function ViewEmailModel(id) {
    var $this = this;
    $this.template = 'email';
    
    $this.Id = ko.observable();
    $this.From = ko.observable();
    $this.To = ko.observable();
    $this.Subject = ko.observable();
    $this.Body = ko.observable();
    $this.Date = ko.observable();
    
    $this.openEmail = function (id) {
        var url = 'email/' + encodeURIComponent(id);
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
    };

    $this.openEmail(id);
};