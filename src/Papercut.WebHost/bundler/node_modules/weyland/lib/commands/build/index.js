var log = require('npmlog'),
    util = require('util'),
    helper = require('../../helpers'),
    path = require('path');

exports.invoke = function(context){
    var builds = context.config.build,
        originalWorkingDirectory = process.cwd();

    if(!Array.isArray(builds)){
        builds = [builds];
    }

    if(context.options.verbose){
        log.info("build", "config", util.inspect(builds, { depth: null, colors:true }));
    }

    return helper.asyncForEach(builds, function (buildConfig){
        if(buildConfig.workingDirectory){
            process.chdir(buildConfig.workingDirectory);
        }else{
            buildConfig.workingDirectory = process.cwd();
        }

        buildConfig.buildDirectory = path.join(buildConfig.workingDirectory, 'build');
        helper.forceRemoveDirectory(buildConfig.buildDirectory);

        return helper.glob("**/*.{js,html}", {}).then(function(files) {
            return helper.asyncForEach(files, function(src){
                var dest = path.join('build', src);
                return helper.safeCopy(src, dest);
            });
        }).then(function(){
                process.chdir(buildConfig.buildDirectory);

                return helper.asyncForEach(buildConfig.tasks, function(taskConfig){
                    var task = require('../../tasks/' + taskConfig.moduleId);
                    var includes = taskConfig.include || [];
                    var excludes = taskConfig.exclude || [];

                    if(!Array.isArray(includes)){
                        includes = [includes];
                    }

                    if(!Array.isArray(excludes)){
                        excludes = [excludes];
                    }

                    var allFiles = [];
                    var excludedFiles = [];

                    return helper.asyncForEach(includes, function(glob){
                        return helper.glob(glob, {}).then(function(files) {
                            files.forEach(function(file){ allFiles.push(file); });
                        });
                    }).then(function(){
                            return helper.asyncForEach(excludes, function(glob){
                                return helper.glob(glob, {}).then(function(files) {
                                    files.forEach(function(file){ excludedFiles.push(file); });
                                });
                            });
                        }).then(function(){
                            allFiles = allFiles.filter(function(item){ return excludedFiles.indexOf(item) == -1; });

                            if(taskConfig.files){
                                taskConfig.files.forEach(function(item){ allFiles.push(item); });
                            }

                            taskConfig.files = helper.distinct(allFiles);

                            return task.build(context, buildConfig, taskConfig);
                        })
                });
            }).then(function(){
                process.chdir(originalWorkingDirectory);
            });
    }).end();
};
