var TaskConfigurationHook = require('./taskConfigurationHook');

var ctor = function(name){
    this.config = {
        name:name,
        tasks:[]
    }

    this.task = new TaskConfigurationHook(this);
};

ctor.prototype.workingDirectory = function(dir){
    this.config.workingDirectory = dir;
    return this;
}

module.exports = ctor;