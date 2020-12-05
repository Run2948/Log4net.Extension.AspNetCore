# Log4net.Extension.AspNetCore

Lightweight Logging Extension implementation of `log4net`

### Install Package

[https://www.nuget.org/packages/Log4net.Extension.AspNetCore](https://www.nuget.org/packages/Log4net.Extension.AspNetCore)

### Configure

* Add the `AddLog4Net()` call into your `Configure` method of the `Startup` class.

```csharp
using Microsoft.Extensions.Logging;

public class Startup
{
    //...

    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
        //...

        loggerFactory.AddLog4Net(); 

        //...
    }
}
```

* Add a `log4net.config` file with the content:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <!-- This section contains the log4net configuration settings -->
  <log4net>
    <appender name="DebugAppender" type="log4net.Appender.ConsoleAppender,log4net">
      <layout type="log4net.Layout.PatternLayout,log4net">
        <param name="ConversionPattern" value="%d [%t] %-5p %c - %m%n" />
      </layout>
    </appender>
    <appender name="InfoAppender" type="log4net.Appender.RollingFileAppender,log4net">
      <param name="File" value="Logs/" />
      <param name="AppendToFile" value="true" />
      <param name="RollingStyle" value="Date" />
      <param name="DatePattern" value="&quot;Logs_&quot;yyyyMMdd&quot;_All.txt&quot;" />
      <param name="StaticLogFileName" value="false" />
      <layout type="log4net.Layout.PatternLayout,log4net">
        <param name="ConversionPattern" value="%d [%t] %-5p %c - %m%n" />
      </layout>
    </appender>
    <appender name="ErrorAppender" type="log4net.Appender.RollingFileAppender,log4net">
      <param name="File" value="Logs/" />
      <param name="AppendToFile" value="true" />
      <param name="RollingStyle" value="Date" />
      <param name="DatePattern" value="&quot;Logs_&quot;yyyyMMdd&quot;.txt&quot;" />
      <param name="StaticLogFileName" value="false" />
      <layout type="log4net.Layout.PatternLayout,log4net">
        <param name="ConversionPattern" value="%d [%t] %-5p %c - %m%n" />
      </layout>
      <filter type="log4net.Filter.LevelRangeFilter">
        <levelMin value="ERROR" />
        <levelMax value="FATAL" />
      </filter>
    </appender>
    <root>
      <level value="ALL" />
      <appender-ref ref="ErrorAppender" />
      <appender-ref ref="InfoAppender" />
    </root>
    <logger name="WebLogger">
      <appender-ref ref="ErrorAppender" />
    </logger>
  </log4net>
</configuration>
```

* Add a global ExceptionFilter:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    //...

    services.AddControllers(options =>
    {
        options.Filters.Add<ExceptionFilter>();
    });

    //...
}
```

* `appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

* `appsettings.Development.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```

* Add the log directory as static, so we can view the logs online.

```csharp
using Microsoft.Extensions.Logging;

public class Startup
{
    //...

    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
        //...

        loggerFactory.AddLog4Net(); 

        //...

        app.UseFileServer(new FileServerOptions()
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Logs")),
            RequestPath = new PathString("/Log"),
            EnableDirectoryBrowsing = true
        });

        //...
    }
}
```

### Global Exception Filter

```csharp
public class ExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ExceptionFilter> _logger;
    public ExceptionFilter(ILogger<ExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext filterContext)
    {
        var log = new StringBuilder();

        //log the url
        if (filterContext.HttpContext.Request.GetDisplayUrl() != null)
            log.AppendLine(filterContext.HttpContext.Request.GetDisplayUrl());

        log.AppendLine($"\tIP: {filterContext.HttpContext.Connection.RemoteIpAddress}");

        foreach (var key in filterContext.HttpContext.Request.Headers.Keys)
        {
            log.AppendLine($"\t{key}: {filterContext.HttpContext.Request.Headers[key]}");
        }

        var exception = filterContext.Exception;
        log.AppendLine("\tError Message:" + exception.Message);
        if (exception.InnerException != null)
        {
            PrintInnerException(exception.InnerException, log);
        }
        log.AppendLine("\tError HelpLink:" + exception.HelpLink);
        log.AppendLine("\tError StackTrace:" + exception.StackTrace);

        _logger.LogError(log.ToString());
    }

    private void PrintInnerException(Exception ex, StringBuilder log)
    {
        log.AppendLine("\tError InnerMessage:" + ex.Message);
        if (ex.InnerException != null)
        {
            PrintInnerException(ex.InnerException, log);
        }
    }
}
```

