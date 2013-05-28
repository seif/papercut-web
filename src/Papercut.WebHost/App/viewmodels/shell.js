define(['durandal/plugins/router', 'durandal/app'], function (router, app) {

    return {
        router: router,
        activate: function () {
            router.mapAuto();
            router.mapRoute("email/:id");
            router.mapRoute({
                url: /#\/mailbox\/page\/(.+)/,
                moduleId: "viewmodels/mailbox",
                name: "Mailbox",
                hash: "#/mailbox/page/"
            });
            return router.activate('mailbox');
        }
    };
});