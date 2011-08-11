﻿using NLog;

namespace Icodeon.Hotwire.Framework.Diagnostics
{
    public class LoggingErrorHandler : IExceptionHandler
    {
        private static Logger _logger;

        public LoggingErrorHandler()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void HandleException(object sender, ExceptionEventArgs args)
        {
            _logger.Error("-------------------------------------------------");
            _logger.ErrorException(args.Exception.Message,args.Exception);
            _logger.Error(args.Exception.GetType().ToString());
            _logger.Error("Request : {0}", args.Request);
            _logger.Error("Section: {0}", args.Section);
            
        }
    }
}