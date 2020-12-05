using System;
using System.Collections.Generic;
using System.Text;
using Log4net.Extension.AspNetCore;

namespace Microsoft.Extensions.Logging
{
    public static class Log4NetExtensions
    {
        public static ILoggerFactory AddLog4Net(this ILoggerFactory factory, string log4NetConfigFile)
        {
            factory.AddProvider(new Log4NetProvider(log4NetConfigFile));
            return factory;
        }

        public static ILoggerFactory AddLog4Net(this ILoggerFactory factory)
        {
            return AddLog4Net(factory, "log4net.config");
        }
    }
}
