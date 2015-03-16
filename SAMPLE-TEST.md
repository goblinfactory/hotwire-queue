#### Sample acceptance test 

This is a sample acceptance test showing the style of acceptance testing I used throughout the project. This sample code is the acceptance test I wrote for one part of the simple security layer I implemented for hotwire to prevent man in the middle replay requests. [This code uses "storyParser" also written and available as part of the hotwire project.](https://github.com/goblinfactory/hotwire-queue/blob/master/V.0.2.4/Icodeon.Hotwire.TestFramework/StoryParser.cs)

The code demonstrates a simple test parameter "parser". I put a great deal of effort into ensuring that test output was very readable, so that the output of tests would be a valuable artifact and could be referred to during discussions with the client, rather than referring to the code itself. 

Below is the test itself, and below that is the console and output that appears in the build and test logs for this test.

        public class ValidMacRequiredIfEndpointIsSecuredRow : RestScenario
        {
            public bool MacHeaderProvided { get; set; }
            public bool EndpointSecuredWithSimpleMac { get; set; }
            public bool CorrectKeyUsedToSign { get; set; }
            public bool Signed { get; set; }
        }

```cs
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
