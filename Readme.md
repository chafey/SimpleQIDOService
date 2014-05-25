SimpleQIDOService
=================

SimpleQIDOService is a very simple implementation of the DICOM QIDO-RS standard.  
On startup, it reads all DICOM files in the C:\QIDO directory and creates an in memory
database of all studies, series and instances to respond to QIDO queries from.  The project
is written in C# with Visual Studio 2013 Update 2, ASP.NET MVC 5 WebApi and the fo-dicom
open source library.  It also uses the Microsoft.AspNet.WebApi.Cors library to support
[Cross Origin Resource Sharing (CORS)](http://en.wikipedia.org/wiki/Cross-origin_resource_sharing)
so web browsers can query it without going through a proxy.

How To Use
----------

This service should work with any QIDO-RS compliant client:

* The [cornerstoneQIDORSWorklist] (https://github.com/chafey/cornerstoneQIDORSWorklist).  Enter the URL this server is listening on in QIDO-RS Root URL:
* [cURL](http://curl.haxx.se/)
* Any rest client test application e.g. "Advanced REST client" available from the Chrome appstore

Caveats
-------

* Changes to C:\QIDO require a startup to be recognized
* The more files you have in C:\QIDO, the longer it takes to startup 
* The pixel data is currently cached so more memory is used that necessary.  This will be fixed once I figure out how to tell fo-dicom to ignore pixel data or free it

Features
--------

* All non-relational queries:
  * Search for Studies: /studies[?query]
  * Search for Series: /studies/{studyInstanceUid}/series[?query]
  * Search for Instances: /studies/{studyInstanceUid}/series/{seriesInstanceUid}/instances[?query]
  * Search for Instances: /studies/{studyInstanceUid}/instances[?query]
* Support for application/json results
* Support for includefield={attributeId}
* Support for {attributeId} as {dicomKeyword} or {dicomTag}
* Support for limit and offset
* Support for CORS (cross origin resource sharing)

Not Supported
-------------

* Relational queries
  * Search for Series: /series[?query]
  * Search for Instances: /instances[?query]
* multipart/related; type=application/dicom+xml responses
* fuzzymatching
* Matches on sequences (e.g. RequestAttributeSequence)
* Returning sequences attribuets in responses (e.g. RequestAttributeSequence)
* includefield=all
* wildcard matching for the following study attributes
  * StudyDate
  * StudyTIme
  * Modalities in study
  * Instance Availability
* wildcard matching for the following series attributes
  * Modality
  * Series Number
  * Performed Procedure Step Start Date
  * Performed Procedure Step Start Time
  * Instance Availability
* wildcard matching for the following instance attributes
  * Instance Number
  * Instance Availability

FAQ
===

_Why don't you support application/dicom+xml responses?_

I am focused on building browser based applications and application/json responses are easier to work with in JavaScript.
If you want to add this, please contact me to discuss.

_Why don't you support relational queries?_

I have no current need for this.  If you want to add this, please contact me to discuss.

_Why don't you support wildcard matching for all attributes?_

I only implemented wildcard matching for the attributes I might actually use it on.  Some of the attributes didn't seem to make
sense to do wildcard matching on - for example, series number or modality.  If someone can explain to me why wildcard
matching is important on these, I might add it.

_Why don't you support includefield=all?_

I don't currently have a need for this.  It should be straightforward to add this though as I added some plumbing for it already.
Contact me to discuss.

_Why don't you support sequences?_

I have no need for this right now.  If you want to add this, please submit a pull request

_Why don't you support fuzzymatching?_

I don't know what this is and the current matching functionality meets my immediate needs.  If you want to add this, please contact me
to discuss.

Copyright
=========

Copyright 2014 Chris Hafey [chafey@gmail.com](mailto:chafey@gmail.com)

