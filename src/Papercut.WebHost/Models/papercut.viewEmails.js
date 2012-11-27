function ViewEmailsModel(mailboxName) {
    var $this = this;

    $this.template = 'emails';
    $this.emails = ko.observableArray();
    $this.mailboxName = ko.observable('');
    $this.mailboxes = ko.observableArray();

    $this.emailSortFunction = function (a, b) {
        return new Date(a.value.email.date) > new Date(b.value.email.date) ? -1 : 1;
    };

    $this.chooseMailbox = function (key) {
        var url = 'mailboxes/' + encodeURIComponent(key);
        $.get(url).success(function (data) {
            $this.mailboxName(key);

            var rows = data.doc.rows;

            $this.emails(rows);
        });
    };

    $this.sortedEmails = ko.dependentObservable(function () {
        return $this.emails.slice().sort($this.emailSortFunction);
    }, $this.emails);

    $.get('mailboxes/default').success(function (data) {
        $this.emails(data.doc.rows);
    });

    $.get('mailboxes').success(function (data) {
        $this.mailboxes(data.rows);
    });

    $this.chooseMailbox(mailboxName || 'default');
};