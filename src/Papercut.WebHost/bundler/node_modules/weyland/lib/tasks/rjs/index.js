var log = require('npmlog'),
    Q = require('kew'),
    util = require('util'),
    rjs = require('requirejs'),
    path = require('path'),
    helper = require('../../helpers');

exports.defaultConfig = {
    moduleId:'rjs',
    rjs:{
        exclude : [],
        include :[]
    }
};

function sortPaths(original){
    if(!original){
        return [];
    }

    var final = [];

    for(var key in original){
        final.push({
            key:key,
            path:original[key]
        });
    }

    final.sort(function(a, b){
        return b.path.length - a.path.length; // ASC -> a - b; DESC -> b - a
    });

    return final;
}

exports.build = function(context, buildConfig, taskConfig){
    var dfd = Q.defer(),
        config = taskConfig.rjs,
        files = taskConfig.files,
        paths = sortPaths(taskConfig.rjs.paths);

    log.info('rjs', 'Packaging ' + taskConfig.files.length.toString() + ' files.');

    for(var i = 0, len = files.length; i < len; i++){
        var current = files[i];

        if(current == config.out){
            continue;
        }

        current = path.relative(config.baseUrl, current).replace(/\\/g, '/');

        for(var j = 0, len2 = paths.length; j < len2; j++){
            var value = paths[j];
            if(current.indexOf(value.path) == 0){
                current = current.replace(value.path, value.key);
                break;
            }
        }

        for(var extension in taskConfig.loaderPluginExtensionMaps){
            if(current.indexOf(extension, current.length - extension.length) !== -1){
                current = taskConfig.loaderPluginExtensionMaps[extension] + '!' + current;
            }else{
                current = current.replace('.js', '');
            }
        }

        console.log(current);
        config.include.push(current);
    }

    var destination = path.join(buildConfig.workingDirectory, config.out);

    rjs.optimize(config, function(){
        helper.readFile(config.out).then(function(data){
            var text = data.toString();

            text = text.replace("define('main',\"jquery\",[]", "define('jquery',[]");
            text = text.replace("define('main','jquery',[]", "define('jquery',[]");

            helper.writeFile(destination, text).then(function(){
                console.log('Optimized build created at "' + destination + '".');
                dfd.resolve();
            });
        });

    });

    return dfd.promise;
}