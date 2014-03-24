var log = require('npmlog'),
    jshint = require('jshint'),
    helper = require('../../helpers');

exports.defaultConfig = {
    moduleId:'jshint',
    stopOnErrors:false,
    jshint:{
        options:undefined,
        globals:undefined
    }
};

exports.build = function(context, buildConfig, taskConfig){
    log.info('jshint', 'Linting ' + taskConfig.files.length.toString() + ' files.');

    return helper.asyncForEach(taskConfig.files, function(filePath){
        return helper.readFile(filePath, 'utf-8').then(function(buf){
            buf = buf.replace(/^\uFEFF/, '');  // remove Byte Order Mark

            var success = jshint.JSHINT(buf, taskConfig.jshint.options, taskConfig.jshint.globals);

            if(success){
                log.info('jshint', 'Passed for ' + filePath + '.');
            }else{
                log.error('jshint', 'Found ' + jshint.JSHINT.errors.length.toString() + ' issues while linting ' + filePath + '.');

                if(context.options.verbose){
                    jshint.JSHINT.errors.forEach(function(error){
                        log.error('jshint', error);
                    });
                }

                if(taskConfig.stopOnErrors){
                    log.error('jshint', 'Errors detected. Stopped.');
                    process.exit(1);
                }
            }
        }).fail(function(error){
                log.error('jshint', error);
                throw new Error(error);
        });
    });
}