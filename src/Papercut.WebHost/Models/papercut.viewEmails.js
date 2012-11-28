function ViewEmailsModel(mailboxName) {
    var $this = this;
    $this.template = 'emails';
    $this.emails = ko.observableArray();
    $this.selectedMailbox = ko.observable();

    $this.emailSortFunction = function (a, b) {
        return new Date(a.Date) > new Date(b.Date) ? -1 : 1;
    };

    $this.chooseMailbox = function (mailboxName) {
        var mailboxName = mailboxName.replace(/\//g, '_z_').replace(/ /g, '_');

        $.routes.find('mailbox').routeTo({ name: mailboxName });
    };

    $this.openMailbox = function (name) {
        var url = 'mailboxes/' + encodeURIComponent(name);
        $.getJSON(url).success(function (data) {
            $this.selectedMailbox(name);

            $this.emails(data.Emails);
        }).error(function (e) {
            console.log(e);
        });
    };

    $this.sortedEmails = ko.dependentObservable(function () {
        return $this.emails.slice().sort($this.emailSortFunction);
    }, $this.emails);

    $this.openMailbox(mailboxName || 'default');
};