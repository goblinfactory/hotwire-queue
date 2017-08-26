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

---

# QuickAssert.cs 

### stupidly simple assertion framework (1 file) I wrote while writing hotwire

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
---

#### Sample acceptance test 

***This is a sample acceptance test showing the style of acceptance testing I used throughout the project. *** This sample code is the acceptance test I wrote for one part of the simple security layer I implemented for hotwire to prevent man in the middle replay requests. [This code uses "storyParser" also written and available as part of the hotwire project.](https://github.com/goblinfactory/hotwire-queue/blob/master/V.0.2.4/Icodeon.Hotwire.TestFramework/StoryParser.cs)

The code demonstrates a simple test parameter "parser". I put a great deal of effort into ensuring that test output was very readable, so that the output of tests would be a valuable artifact and could be referred to during discussions with the client, rather than referring to the code itself. 

**simple scenario DTOs**
```cs

    public class ScenarioRow
    {
        public int SequenceNo { get; set; }
        public string Title { get; set; }
    }

    public class RestScenario : ScenarioRow
    {
        public Uri Uri { get; set; }
        public int Response { get; set; }
        public string ResponseTextToContain { get; set; }
    }

```
Below is the test itself, and below that is the console and output that appears in the build and test logs for this test.

```cs

        public class ValidMacRequiredIfEndpointIsSecuredRow : RestScenario
        {
            public bool MacHeaderProvided { get; set; }
            public bool EndpointSecuredWithSimpleMac { get; set; }
            public bool CorrectKeyUsedToSign { get; set; }
            public bool Signed { get; set; }
        }


        [Test]
        public void ValidMacRequiredIfEndpointIsSecured()
        {
            TraceTitle("ValidMacRequiredIfEndpointIsSecured()");
            string story = @"

            |Title                                              |MacHeaderProvided |EndpointSecuredWithSimpleMac |Signed | CorrectKeyUsedToSign | Response    | ResponseTextToContain               |#
            |---------------------------------------------------|------------------|-----------------------------|-------|----------------------|-------------|-------------------------------------|
            |signed with correct key                            |yes               |yes                          |yes    |yes                   |200          |helloWorld                           |1
            |signed with invalid key                            |yes               |yes                          |yes    |no                    |401          |Unauthorized                         |2
            |not signed at all                                  |yes               |yes                          |yes    |no                    |401          |Unauthorized                         |3
            |mac header missing                                 |no                |yes                          |yes    |yes                   |401          |No valid MAC was found in the headers|4
            |access unsecured endpoint using signed request     |yes               |no                           |yes    |yes                   |200          |helloWorld                           |5
            |access unsecured endpoint using unsigned request   |yes               |no                           |no     |yes                   |200          |helloWorld                           |6
            |unsigned accessing signed endpoint                 |na                |yes                          |no     |na                    |401          |No valid MAC was found in the headers|7";

            StoryParser.Parse<ValidMacRequiredIfEndpointIsSecuredRow>(story).ForEach(scenario =>
            {
                MockModule module;
                string privateKey = "4497BB2A-6782-4403-970C-1A7F50BBC7CB";
                string requestPrivateKey = scenario.CorrectKeyUsedToSign ? privateKey : "AAEEDDCC";
                string macSalt = Guid.NewGuid().ToString();
                Trace("");
                TraceTitle(scenario.Title);
                Trace("Given an endpoint " + (scenario.EndpointSecuredWithSimpleMac ? " configured with simple mac" : "with no security"));
                var moduleConfiguration = scenario.EndpointSecuredWithSimpleMac
                    ? _givenAnEchoModuleWithAnEndpointThatIsConfiguredWithSimpleMac(out module, privateKey  )
                    : _givenAnEchoModuleWithAnEndpointThatIsNotConfiguredWithSimpleMac(out module);

                if (!scenario.EndpointSecuredWithSimpleMac)
                {
                    moduleConfiguration.Endpoints.First().Security.Should().Be(SecurityType.none);
                }

                if (scenario.EndpointSecuredWithSimpleMac)
                {
                    //todo: NB! it's not currently obvious (without knowing the code) that OauthRequestAuthenticator requires an instance of ISimpleMacDAL. Change module base to accept the authenticators as dependancies ! That way it will be obvious! doh..bad. ... accepts a lambda that creates the requestAuthenticator... this will eliminate the really bad factory initialisation required below.

                    ObjectFactory.Initialize(x => {
                                x.For<IDateTime>().Use<DateTimeWrapper>();
                                x.For<ISimpleMacDAL>().Use(new SimpleMacDal(new DateTimeWrapper(), new HotwireContext(ConnectionStringManager.HotwireConnectionString)));
                    });
                }

                var requestParameters = new NameValueCollection();
                requestParameters.Add("parameter1", "value1");
                requestParameters.Add("parameter2", "value2");

                MockStreamingContext context = null;

                
                if (scenario.Signed)
                {
                    Trace("And a signed context");
                    // would be excellent if I could create a mock streaming context from a configured httpClient
                    // then the "test" code could be exactly the same for integration tests as it is for unit tests...hmmm?
                    context = new MockStreamingContext(requestParameters, "http://localhost/test/echo/helloWorld", moduleConfiguration);
                    var dateTime = new DateTimeWrapper();
                    var simpleMacSigner = new SimpleMacAuthenticator(dateTime, new SimpleMacDal(new DateTimeWrapper(), new HotwireContext(ConnectionStringManager.HotwireConnectionString)));
                    int timeStamp = dateTime.SecondsSince1970;
                    simpleMacSigner.SignRequestAddToHeaders(context.Headers, requestPrivateKey, requestParameters, context.HttpMethod, context.Url, macSalt, timeStamp);

                }

                if (!scenario.Signed)
                {
                    Trace("And an unsigned context");
                    context = new MockStreamingContext(requestParameters, "http://localhost/test/echo/helloWorld", moduleConfiguration);
                }

                if (!scenario.MacHeaderProvided)
                {
                    context.Headers.Remove(SimpleMACHeaders.HotwireMacHeaderKey);
                }


                Trace("When the module processes the request");
                module.BeginRequest(context);
                context.Should().NotBeNull();

                Trace("Then the response should be :" + scenario.Response);
                context.HttpWriter.ShouldBe(scenario.Response);

                Trace("and the response text should contain '" + scenario.ResponseTextToContain + "'");
                context.MockWriter.ToString().Should().Contain(scenario.ResponseTextToContain);
            });


        }
```

**Test output:**

```
ValidMacRequiredIfEndpointIsSecured()
-------------------------------------
> 
signed with correct key
-----------------------
> Given an endpoint  configured with simple mac
> Given an echo module with an endpoint that is configured with simpleMAC security and a privateKey
> And a signed context
> When the module processes the request
> Then the response should be :200
> and the response text should contain 'helloWorld'
> 
signed with invalid key
-----------------------
> Given an endpoint  configured with simple mac
> Given an echo module with an endpoint that is configured with simpleMAC security and a privateKey
> And a signed context
> When the module processes the request
> Then the response should be :401
> and the response text should contain 'Unauthorized'
> 
not signed at all
-----------------
> Given an endpoint  configured with simple mac
> Given an echo module with an endpoint that is configured with simpleMAC security and a privateKey
> And a signed context
> When the module processes the request
> Then the response should be :401
> and the response text should contain 'Unauthorized'
> 
mac header missing
------------------
> Given an endpoint  configured with simple mac
> Given an echo module with an endpoint that is configured with simpleMAC security and a privateKey
> And a signed context
> When the module processes the request
> Then the response should be :401
> and the response text should contain 'No valid MAC was found in the headers'
> 
access unsecured endpoint using signed request
----------------------------------------------
> Given an endpoint with no security
> Given an echo module with an endpoint that is not configured with simpleMAC security and a privateKey
> And a signed context
> When the module processes the request
> Then the response should be :200
> and the response text should contain 'helloWorld'
> 
access unsecured endpoint using unsigned request
------------------------------------------------
> Given an endpoint with no security
> Given an echo module with an endpoint that is not configured with simpleMAC security and a privateKey
> And an unsigned context
> When the module processes the request
> Then the response should be :200
> and the response text should contain 'helloWorld'
> 
unsigned accessing signed endpoint
----------------------------------
> Given an endpoint  configured with simple mac
> Given an echo module with an endpoint that is configured with simpleMAC security and a privateKey
> And an unsigned context
> When the module processes the request
> Then the response should be :401
> and the response text should contain 'No valid MAC was found in the headers'


```



