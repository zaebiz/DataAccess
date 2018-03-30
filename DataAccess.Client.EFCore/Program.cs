using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DataAccess.Client.EFCore.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DataAccess.Client.EFCore
{
    public class Program
    {
        public static ILoggerFactory _loggerFactory;
        public static IConfiguration _configuration;

        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (String.IsNullOrWhiteSpace(environment))
                throw new ArgumentNullException("Environment not found in ASPNETCORE_ENVIRONMENT");

            Console.WriteLine("Environment: {0}", environment);

            var dict = new Dictionary<string, string>
            {
                {"ConnectionStrings:InMemoryBlogContext", ""},
            };

            var services = new ServiceCollection();

            // Set up configuration sources.
            // todo use inmemory collection
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(dict)
                .Build();

            _loggerFactory = new LoggerFactory()
                .AddConsole(_configuration.GetSection("Logging"))
                .AddDebug();

            //services.AddDbContext<BlogContext>()
            //    UseI

            //services.AddTransient<IPackageFileService, PackageFileServiceImpl>();

            //var serviceProvider = services.BuildServiceProvider();

            //var packageFileService = serviceProvider.GetRequiredService<IPackageFileService>();
        }
    }
}
