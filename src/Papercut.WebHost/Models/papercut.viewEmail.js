function ViewEmailModel(mailboxName, id) {
    var $this = this;
    $this.template = 'email';
    
    $this.Id = ko.observable();
    $this.From = ko.observable();
    $this.To = ko.observable();
    $this.Subject = ko.observable();
    $this.Body = ko.observable();
    $this.Date = ko.observable();

    $this.chooseMailboxItem = function (mailboxName, id) {
        var mailboxName = mailboxName.replace(/\//g, '_z_').replace(/ /g, '_');
        $.routes.find('email').routeTo({ name: mailboxName, id: id });
    };

    $this.openMailboxItem = function (mailboxName, id) {
        var url = 'mailboxes/' + encodeURIComponent(mailboxName) + '/' + encodeURIComponent(id);
        $.getJSON(url).success(function (data) {
            $this.Id(data.Id);
            $this.From(data.From);
            $this.To(data.To);
            $this.Subject(data.Subject);
            $this.Body(data.Body.replace(/\n/g, '<br />'));
            $this.Date(data.Date);
        }).error(function (e) {
            console.log(e);
        });
    };

    $this.openMailboxItem(mailboxName, id);
};