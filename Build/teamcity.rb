#Needed for rake/gem '= 0.9.2.2'
Rake::TaskManager.record_task_metadata = true
module TeamCity
	def teamcity_progress(task)
        teamcity_service_message 'progressStart', task
        yield
        teamcity_service_message 'progressFinish', task
    end

    def teamcity_service_message(message, message_value)
        puts "##teamcity[#{message} '#{message_value}']"
    end
end

class Rake::Task
    include TeamCity
       old_execute = self.instance_method(:execute)
       define_method(:execute) do |args|
         teamcity_progress("#{comment}") { old_execute.bind(self).call(args) }
	end
end

class NUnitTestRunner    
	attr_accessor(:xml_output)
    old_execute = self.instance_method(:execute)
    
	define_method(:execute) do
	
	@options << '/xml=' + @xml_output unless @xml_output.nil?
	
	 #args.options "/xml=WHATEVER"	 
	 old_execute.bind(self).call()
	 puts "##teamcity[importData type='nunit' path='#{@xml_output}']"
	 
	end
end
