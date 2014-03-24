var log = require('npmlog'),
    uglifyjs = require('uglify-js'),
    FILE_ENCODING = 'utf-8',
    helper = require('../../helpers');

exports.defaultConfig = {
    moduleId:'uglifyjs',
    uglifyjs:{
        compressor:{},
        output:{}
    }
};

exports.build = function(context, buildConfig, taskConfig){
    log.info('uglifyjs', 'Uglifying ' + taskConfig.files.length.toString() + ' files.');

    return helper.asyncForEach(taskConfig.files, function(filePath){
        return helper.readFile(filePath, FILE_ENCODING).then(function(buf){
            var ast = uglifyjs.parse(buf);

            ast.figure_out_scope();

            var compressor = uglifyjs.Compressor(taskConfig.uglifyjs.compressor);
            var compressed_ast = ast.transform(compressor);

            compressed_ast.figure_out_scope();
            compressed_ast.compute_char_frequency();
            compressed_ast.mangle_names();

            if(taskConfig.uglifyjs.output.source_map){
                var source_map = uglifyjs.SourceMap(taskConfig.uglifyjs.output.source_map);
                taskConfig.uglifyjs.output.source_map = source_map;
            }

            var stream = uglifyjs.OutputStream(taskConfig.uglifyjs.output);
            compressed_ast.print(stream);
            var code = stream.toString();

            return helper.writeFile(filePath, code, FILE_ENCODING).then(function(){
                log.info('uglifyjs', 'Uglified ' + filePath + '.');
            });
        }).fail(function(error){
            log.error('uglifyjs', error);
            throw new Error(error);
        });
    });
}