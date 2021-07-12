using System;
using System.Collections.Concurrent;
using System.IO;
using System.Xml;

namespace Microsoft.Extensions.Logging
{
    public class Log4NetProvider : ILoggerProvider
    {
        private readonly string _log4NetConfigFile;
        private readonly ConcurrentDictionary<string, Log4NetLogger> _loggers = new ConcurrentDictionary<string, Log4NetLogger>();
        public Log4NetProvider(string log4NetConfigFile)
        {
            _log4NetConfigFile = log4NetConfigFile;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, CreateLoggerImplementation);
        }

        public void Dispose()
        {
            _loggers.Clear();
            GC.SuppressFinalize(this);
        }

        private Log4NetLogger CreateLoggerImplementation(string name)
        {
            return new Log4NetLogger(name, ParseLog4NetConfigFile(_log4NetConfigFile));
        }

        private static XmlElement ParseLog4NetConfigFile(string filename)
        {
            var log4NetConfig = new XmlDocument();
            if (!File.Exists(filename))
                filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
            log4NetConfig.Load(File.OpenRead(filename));
            var configuration = log4NetConfig["configuration"];
            return configuration == null ? log4NetConfig["log4net"] : configuration["log4net"];
        }
    }
}
