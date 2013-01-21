function ViewEmailsModel() {
    var $this = this;
    $this.template = 'emails';
    $this.emails = ko.observableArray();
    $this.links = Array();
    $this.currentPage = ko.observable();
    $this.totalPages = ko.observable();

    $this.emailSortFunction = function(a, b) {
        return new Date(a.Date) > new Date(b.Date) ? -1 : 1;
    };

    $this.chooseEmail = function(id) {
        $.routes.find('email').routeTo({ id: id });
    };

    $this.next = function() {
        $this.get($this.links[0].Href);
    };

    $this.previous = function() {
        $this.get($this.links[1].Href);
    };

    $this.loadEmails = function () {	
        var url = 'emails';
        $this.get(url);
    };

    $this.get = function (url) {
        $.getJSON(url).success(function (data) {
            console.debug("Got emails from server");
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

    $this.loadEmails();
};