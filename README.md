*update 16.03.2015, I've committed a quick update today so that the project will build. (quite a few changes since this project was originally written. It's a quick hack because while I won't use hotwire for writing restful services, (servicestack and webapi, or mvc are much better suited and were not stable or available at the time of writing hotwire). I've updated hotwire so that the project can be cloned and the tests can be run and used as a reference of tests I'm proud of, and enjoyed writing. (All the unit tests should pass, except 2 or 3 I think, that will only pass when run in debug mode as they rely on the built in webserver to start in debug mode.) ;-D*

### Hotwire is a generalised download and queue workflow for large files:

#### Hotwire project objectives
Hotwire was primarily built to avoid a restful service provider having to invent solutions to deal with the challenge of what to do when an API requires a "file" or "document" as a parameter. Typical solutions include accepting a stream, or byte array, or by using an out of band mechanism, e.g. FTP, and simply including the filename as the parameter.

Another problem that large files present, is that accepting uploads of very large files can use up valuable bandwidth, and an overlap of even a few overlapping (long running) requests could result in a very brittle API, problems with timeouts, as well as difficulty with testing.

Hotwire attempts to address some of these problems by providing a framework that allows document publishers to "enqueue a request to download a file that's ready for processing" at a public or private uri. Hotwire provides the tools to manage a simple file based queue, and to download all the files queued for downloading, and optionally (if configured) to invoke a consumer's implementation of IProcessFile.

The core Hotwire enqueue service runs as a HttpModule? under IIS and requires only a small amount of web.config configuration, specifying such items as the folders to use for processing, downloading, queue etc, and which assembly and class implements IFileProcess.

The Uri's that the HttpModule? responds to are configurable in order that Hotwire can be configured to run as part of your existing restful API without requiring that the consumer of your API need know anything about hotwire other than the parameters required for enqueue'ing a request to download and process a document.

**Below is a sample service configuration: (not the whole configuration)**
```xml
  <hotwire>
    <queues active="true" rootServiceName="demoService" methodValidation="afterUriValidation"> 
      <endpoints>
        <add name="q1" active="true" uriTemplate="queues" action="ENQUEUE-REQUEST" httpMethods="POST" mediaType="json" security="oauth" />
        <add name="q2" active="true" uriTemplate="queues.xml" action="ENQUEUE-REQUEST" httpMethods="POST" mediaType="xml" security="oauth" />
        <add name="q3" active="true" uriTemplate="queues.json" action="ENQUEUE-REQUEST" httpMethods="POST" mediaType="json" security="oauth" />
      </endpoints>
    </queues>
    <!--  
```

**Sample folder configuration showing the filesystem based Queue repository:**

```xml
    <folders
        solutionFolderMarkerFile="HotwireSolutionFolderMarkerFile.txt"
        downloadErrorFolder =   "Icodeon.Hotwire.TestAspNet\App_Data\HotwireFolders\DownloadError"
        testDataFolder =        "Icodeon.Hotwire.TestAspNet\App_Data\HotwireFolders\TestFiles"
        downloadQueueFolder =   "Icodeon.Hotwire.TestAspNet\App_Data\HotwireFolders\DownloadQueue"
        processQueueFolder =    "Icodeon.Hotwire.TestAspNet\App_Data\HotwireFolders\ProcessQueue"
        processingFolder =      "Icodeon.Hotwire.TestAspNet\App_Data\HotwireFolders\Processing"
        processedFolder =       "Icodeon.Hotwire.TestAspNet\App_Data\HotwireFolders\Processed"
        processErrorFolder =    "Icodeon.Hotwire.TestAspNet\App_Data\HotwireFolders\ProcessError"
        downloadingFolder =     "Icodeon.Hotwire.TestAspNet\App_Data\HotwireFolders\Downloading"
      />
```

Note: This codebase does not include any oAuth implementations. In a later update which I will be providing shortly, if you wish to use OAuth authentication then you will need to provide a wrapper for an Oauth provider. Your wrapper must be around whichever OAuth codebase you want to use, and implement IOAuthProvider. As a result building this project using "acceptance test driven development" and of not being able to distribute certain code that are part of the acceptance tests, (but not part of the open source project) I am currently unable to provide any unit tests for this project as well as Oauth authentication. Unit tests are coming soon as a high priority, and if I can get permission from the authors of a certain Oauth provider then I'll be very pleased to include some oauth authenticated tests.

new : QuickAssert is a test class used to help produce more readable unit tests.

#### requirements
* Visual studio 2010 sp1 (Nuget package manager, which comes with sp1 is required in order to use the included packages file. ) If you don't have Nuget, you can still download the assemblies manually.
The following packages are required in order to build the solution and are included.

```xml
<packages>
  <package id="NLog" version="2.0.0.0" />
</packages>
NLog licence is in "/Icodeon.Hotwire.Solution/packages/NLog.2.0.0.0/LICENCE.txt"
```

#### [Roadmap - Product Backlog and wishlist](ROADMAP.md)

#### Copyright
**Copyright (c) Icodeon Limited 2011.**

This project is available free to use in commercial or non-commercial projects under a [Mozilla Public License 1.1.](http://www.mozilla.org/MPL/MPL-1.1-annotated.html)

*Hotwire was developed for Icodeon Limited by Net-Catalogue Ltd, (trading as Goblinfactory) using European Union funding for the Adopting Standards and Specifications for Educational Content Project No. ECP 417008. The project was initially built using Microsoft C# technologies.*

---

#### QuickAssert.cs [stupidly simple assertion framework (1 file) I wrote while writing hotwire] (QUICK-ASSERT.md)

#### [SAMPLE-TEST.md Sample Acceptance tests with sample test output] (SAMPLE-TEST.md)