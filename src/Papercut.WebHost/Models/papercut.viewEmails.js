function ViewEmailsModel(mailboxName) {
    var $this = this;
    $this.template = 'emails';
    $this.emails = ko.observableArray();
    $this.selectedMailbox = ko.observable();
    $this.links = Array();
    $this.currentPage = ko.observable();
    $this.totalPages = ko.observable();

    $this.emailSortFunction = function(a, b) {
        return new Date(a.Date) > new Date(b.Date) ? -1 : 1;
    };

    $this.chooseMailbox = function(mailboxName) {
        var mailboxName = mailboxName.replace( /\//g , '_z_').replace( / /g , '_');

        $.routes.find('mailbox').routeTo({ name: mailboxName });
    };

    $this.chooseEmailItem = function(id) {
        $.routes.find('email').routeTo({ name: mailboxName, id: id });
    };

    $this.next = function() {
        $this.get($this.links[1].Href);
    };

    $this.previous = function() {
        $this.get($this.links[2].Href);
    };

    $this.openMailbox = function (name) {
        var url = 'mailboxes/' + encodeURIComponent(name);
        $this.get(url);
    };

    $this.get = function (url) {
        $.getJSON(url).success(function (data) {
            $this.selectedMailbox(name);
            $this.emails(data.Emails);
            $this.links = data.Links;
            $this.currentPage(data.Page);
            $this.totalPages(data.Pages);
        }).error(function (e) {
            console.log(e);
        });
    };

    $this.sortedEmails = ko.dependentObservable(function () {
        return $this.emails.slice().sort($this.emailSortFunction);
    }, $this.emails);

    $this.openMailbox(mailboxName || 'default');
};