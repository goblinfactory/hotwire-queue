#### Roadmap - Product Backlog

* (done 21.05.2011) replace NLog.Logger with LoggerBase? + NullableLogger? implementation. (This will simplify tests as so many classes require Logger. I want to be able to pass in null for the logger during unit tests)
* (done 23.05.2011) Refactor ModuleBase? (http module base class) so that custom httpModules can be unit testable. Includes wrappers for httpContext, ApplicationState?, and Response)
* Acceptance tests to serve as project documentation ( I have acceptance tests, but need to sanitise them and bring them into the project)
* Move roadmap (Product backlog) to Agilezen. (apply for free o/s account).
* (done, own private CI server) CI Server? Not sure how this will work with an OS project on google code. (make a plan)
* Unit test for code coverage (same as the acceptance tests.)
* Folder watcher utility : Monitors folders and processes all enqueued items.
* Install hotwire via nuget (prerequisite better website, samples and "why use/what problem solve?")
* Fluent (code) configuration. (Currently hotwire is configured with xml in web.config.) DONE
* HTML5 front end for monitoring the queue
* Zero configuration option (Instead of explicitly telling Hotwire that class X in assembly Y implements FileProcess?, I want to use StructureMap? or similar to automatically detect it.) DONE, SUPPORTS MIX, DEFAULTS WITH FLUENT OVERRIDES
* Support for running hotwire services as a WCF service and/or under MVC. (depending on complexity. Will spike this first and implement whichever is least complex.) INVESTIGATING
* Implement Queue priority, will include HTML 5 admin tool to pause, requeue and edit queued item priorities.
* Email notification of fileProcess errors. DONE
* Use a configurable "pipeline" of process steps instead of a set of the existing hard coded folders. (this will remove a lot of code duplication in the filesProvider).
* Support multiple fileprocessors, and be able to specify which file processor processes which configured "endpoint".
* Endpoint to be mapped to a named, configured pipeline (or "named" set of directories).
* Use Structuremap or other dependancy injection framework. (was originally excluded from this project due to possible Open source licence challenges. need to review and if OK will use).
* Integration with Elmah or similar for unhandled error notification. Check licence compatibility, requirements etc. DONE
* replace filesProvider with pluggable provider and use database instead of file system.

#### Wishlist
* Contact Oauth authors and see if I can get permission to include assembly in a ready-to-use built download.
* Contact GB and see if Microsoft will allow us to include Microsoft.Http in a download as this will make testing so much neater and simpler.
