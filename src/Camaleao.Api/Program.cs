using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace Camaleao.Api {
    public class Program {
        private static string _environmentName;

        public static void Main(string[] args) {
            try {

                var webHost = BuildWebHost(args);

                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .AddJsonFile($"appsettings.{_environmentName}.json", optional: true, reloadOnChange: true)
                            .Build();

                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();


                Log.Information("Start Application");
                webHost.Run();
           
            }
            catch (Exception ex) {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally {
                Log.Information("Finish Application");
                Log.CloseAndFlush();
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, config) => {
                    config.ClearProviders();
                    _environmentName = hostingContext.HostingEnvironment.EnvironmentName;
                })
                .UseStartup<Startup>().UseUrls("http://*:5000")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .Build();
    }
}
