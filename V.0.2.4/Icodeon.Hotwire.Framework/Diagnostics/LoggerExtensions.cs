﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using NLog;

namespace Icodeon.Hotwire.Framework.Diagnostics
{
    public static class LoggerExtensions
    {

        public static void LoggedExecution(this Logger logger,string title, Action code)
        {
            try
            {
                // newlines added here because LoggedExecution is most commonly used
                // in fixture setups, where there is no "traceTitle", so we don't want
                // this output to be seen as part of some other unit test

                logger.Debug("--- {0} // ----",title);
                code();
                logger.Debug("---- // {0} ---",title);
            }
            catch (Exception ex)
            {
                // don't need to re-log Logged exception, as it's already been logged.
                if (ex is LoggedException) throw;
                throw new LoggedException(logger, ex);
            }
        }

        public static void TraceParameters(this Logger logger, NameValueCollection coll)
        {
            foreach (var key in coll.AllKeys)
            {
                logger.Trace("{0,-30}:{1}", key, coll[key]);
            }
        }

    }
}
