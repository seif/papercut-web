var BuildConfigurationBuilder = require('./buildConfigurationBuilder');

var ctor = function(){
    this.config = {
        build:[]
    }
}

ctor.prototype.build = function(name){
    var builder = new BuildConfigurationBuilder(name);
    this.config.build.push(builder.config);
    return builder;
};

module.exports = ctor;