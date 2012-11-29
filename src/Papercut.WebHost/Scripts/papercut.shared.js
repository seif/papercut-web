var viewModel = {
    mailboxes: ko.observableArray(),
    current: ko.observable({ template: 'emails' })
};

$(function () {
    
    
    $('.sidebar-nav li').click(function () {

        $(this).siblings().removeClass('active');
        $(this).addClass('active');
    });

    $.getJSON("mailboxes").success(function (data) {

        viewModel.mailboxes(data);

        var templateEngine = new ko.jqueryTmplTemplateEngine();
        templateEngine.makeTemplateSource = function (template) {
            if (typeof template == "string") {
                var node = document.getElementById(template);
                if (node == null) {
                    var templateHtml = null;

                    $.ajax({
                        async: false,
                        url: 'templates/' + template + '.html',
                        dataType: "html",
                        type: "GET",
                        timeout: 0,
                        success: function (response) {
                            templateHtml = response;
                        },
                        error: function (exception) {
                            if (this['useDefaultErrorTemplate'])
                                templateHtml = this['defaultErrorTemplateHtml'].replace('{STATUSCODE}', exception.status).replace('{TEMPLATEID}', templateId).replace('{TEMPLATEURL}', templatePath);
                        } .bind(this)
                    });

                    if (templateHtml === null)
                        throw new Error("Cannot find template with ID=" + template);

                    var node = document.createElement("script");
                    node.type = "text/html";
                    node.id = template;
                    node.text = templateHtml;
                    document.body.appendChild(node);
                }
                return new ko.templateSources.domElement(node);
            } else if ((template.nodeType == 1) || (template.nodeType == 8)) {
                // Anonymous template
                return new ko.templateSources.anonymousTemplate(template);
            } else
                throw new Error("Unrecognised template type: " + template);
        };
        ko.setTemplateEngine(templateEngine);

        $.routes.add("/{name:word}/{id:word}/", 'email', function () {
            viewModel.current(new ViewEmailModel(this.name, this.id));
        });

        $.routes.add("/{name:word}/", 'mailbox', function () {
            viewModel.current(new ViewEmailsModel(this.name));
        });

        $.routes.add("/", 'root', function () {
            viewModel.current(new ViewEmailsModel());
        });

        $('a.time').prettyDate();

        viewModel.current(new ViewEmailsModel('default'));
        
        ko.applyBindings(viewModel.current());
    });
});