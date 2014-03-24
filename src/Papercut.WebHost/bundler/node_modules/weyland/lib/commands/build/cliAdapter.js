var build = require('./index'),
    program = require('commander'),
    path = require('path'),
    fs = require('fs'),
    DSL = require('../../dsl');

var command = program
    .command('build')
    .description('Build your app for deploy.')
    .option('-c, --config <path>', 'the path to the weyland-config.js file')
    .action(doBuild);

function doBuild(){
    var configPath = command.config || path.join(process.cwd(), "weyland-config.js"),
        configModule = require(configPath),
        context = {
            options:{
                config:configPath,
                verbose:program.verbose
            }
        };

    if(typeof configModule.config == 'function'){
        var dsl = new DSL();
        configModule.config(dsl);
        context.config = dsl.config;
    }else{
        context.config = configModule.config;
    }

    build.invoke(context);
};