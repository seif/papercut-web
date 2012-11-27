#Papercut#


This is a fork of [http://papercut.codeplex.com/](http://papercut.codeplex.com/ "papercut project"), to allow papercut to run as a windows service and provide a Web UI for viewing the recieved emails.

##Current status##

It works, there is a Http API that allows viewing/deleting emails, currently the UI only allows viewing.

Probably not very performant, as all it does is load the .eml files stored in a folder and return the contents.

##Usage##

1. Build the solution

2. Copy the binaries from WebHost and SmtpHost somewhere.

3. Open each hosts config file and configure values.

4. Either run the host so that it would run as a console app or install it as a service, e.g.:

        Papercut.SmtpHost.exe install


##Technology stack##

- Papercut
- Topshelf
- ServiceStack