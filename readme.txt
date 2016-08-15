1. Deployment Instruction
=========================

1.1 When deploying the hosting web site to live server, convert the folder to Application and select ASP.NET v4.0 
as Application Pool Identity. Grant read & execute permissions for "IIS AppPool\ASP.NET v4.0" to the physical folder.

1.2 When hosting a WCF service under IIS, IIS creates one base address for you based on the URI to 
the virtual directory that contains the application.

1.3 set debug="false" in web.config for WebHost when deploying to test and prod


