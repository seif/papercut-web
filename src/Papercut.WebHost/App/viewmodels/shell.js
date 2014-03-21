define(['plugins/router', 'durandal/app'], function (router, app) {

    return {
        router: router,
        activate: function () {
            router.map([
                { route: '', title: 'Mailbox', moduleId: 'viewmodels/mailbox', nav: true },
                { route: 'email/:id', moduleId: 'viewmodels/email' },
                { route: 'mailbox/:page', title: 'Mailbox', moduleId: 'viewmodels/mailbox', hash: '#mailbox/page/' }
            ]).buildNavigationModel();

            return router.activate();
        }
    };
});