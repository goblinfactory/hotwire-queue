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

#### requirements
* Visual studio 2010 sp1 (Nuget package manager, which comes with sp1 is required in order to use the included packages file. ) If you don't have Nuget, you can still download the assemblies manually.
The following packages are required in order to build the solution and are included.

```xml
<packages>
  <package id="NLog" version="2.0.0.0" />
</packages>
NLog licence is in "/Icodeon.Hotwire.Solution/packages/NLog.2.0.0.0/LICENCE.txt"
```

#### Copyright
**Copyright (c) Icodeon Limited 2011.**

This project is available free to use in commercial or non-commercial projects under a [Mozilla Public License 1.1.](http://www.mozilla.org/MPL/MPL-1.1-annotated.html)

*Hotwire was developed for Icodeon Limited by Net-Catalogue Ltd, (trading as Goblinfactory) using European Union funding for the Adopting Standards and Specifications for Educational Content Project No. ECP 417008. The project was initially built using Microsoft C# technologies.*

---

### Product Roadmap
[Roadmap - Product Backlog and wishlist] (ROADMAP.md)

#### QuickAssert.cs ( stupidly simple assertion framework (1 file) I wrote while writing hotwire )

I've just started using Fluent Assertions, (which I love by the way), however, there are some places where I find both the NUnit approach and the Fluent approach results in code that I find is a little cluttered and not very scannable.

QuickAssert.cs is an extension class that allows me to go from this: (fluent style)

```cs
config.Active.Should().BeTrue();
config.RootServiceName.Should().Be("test-animals");
config.MethodValidation.Should().Be(MethodValidation.afterUriValidation);
var endpoints = config.Endpoints;
endpoints.Should().NotBeNull().And.HaveCount(2);
```

to this:

```cs
config.Ensure(c => c.Active,
              c => c.RootServiceName == "test-animals",
              c => c.MethodValidation == MethodValidation.afterUriValidation,
              c => c.Endpoints != null && c.Endpoints.Count() == 2);
```

QuickAssert also fails (if error is detected) and outputs the text representation of the exact code used to check the property, without having to provide a string description. This is not a replacement for Fluent assertions, which will do a better job of expressing very complex requirements. This is for when I need something simple, e.g. checking long lists of properties. I may simply be using Fluent Assertions properly, so please send me comments if there is a better way to do this. Cheers, Al

**Here is the code for QuickAssert so you can use it if you want, without having to get the entire Hotwire codebase**

```cs
using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace Icodeon.Hotwire.Tests.Framework
{
    public static class QuickAssert
    {
        public static void Ensure<TSource>(this TSource source, params Expression<Func<TSource, bool>>[] actions)
        {
            foreach (var expression in actions)
            {
                Ensure(source,expression);
            }
        }

        public static void Ensure<TSource>(this TSource source, Expression<Func<TSource, bool>> action)
        {
            var propertyCaller = action.Compile();
            bool result = propertyCaller(source);
            if (result) return;
            Assert.Fail("Property check failed -> " + action.ToString());
        }
    }
}
```


### SAMPLE-TEST.md
[This is a sample acceptance test showing the style of acceptance testing I used throughout the project. This sample code is the acceptance test I wrote for one part of the simple security layer I implemented for hotwire to prevent man in the middle replay requests.] (SAMPLE-TEST.md)


