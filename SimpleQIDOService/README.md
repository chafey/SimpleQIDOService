
Introdution:
This is a simple implementation of QIDO.  By default, it reads all DICOM files in the directory c:\Qido and creates an in memory index of the studies, series and instances.  This
in memory index is used to respond to QIDO queries.  If you put a lot of files in c:\QIDO, it will take a long time to startup.

Currently only study level queries are supported but there is quite a bit of reusable code so it should be straightforward to add support for series and instance level queries


How this app was built:

create new webapp
 -> choose "Empty: template
use nuget to install fo-dicom
 -> remove Dicom.Native64
Add controller for handling studies queries
 -> right click on controllers folder and add controller
    -> Web API 2 Controller - Empty
use nuget to install Microsoft.AspNet.WebApi.Cors
