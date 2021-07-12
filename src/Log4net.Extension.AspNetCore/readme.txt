How to use Log4net.Extension.AspNetCore in your project?

1. Create a log4net.config file

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


2. Two ways to configure Log4Net component

2.1 Configure Log4Net to logging builder

public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddLog4Net();
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });


2.2 Configure Log4Net to logger factory

// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILoggerFactory loggerFactory)
{
    loggerFactory.AddLog4Net();

    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    ...


3. Use FileServer to browse log online

// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    ...

    app.UseRouting();

    app.UseAuthorization();

    //add the log directory as static, so we can view the log directory
    app.UseFileServer(new FileServerOptions()
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Logs")),
        RequestPath = new PathString("/Log"),
        EnableDirectoryBrowsing = true
    });

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}


4. Use ExceptionFilter to log all exceptions

// This method gets called by the runtime. Use this method to add services to the container.
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers(options =>
    {
        options.Filters.Add<ExceptionFilter>();
    });
}
