define(['durandal/plugins/router', 'durandal/app'], function (router, app) {

    return {
        router: router,
        activate: function () {
            router.mapAuto();
            router.mapRoute("email/:id");
            return router.activate('mailbox');
        }
    };
});