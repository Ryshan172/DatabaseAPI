using System;

namespace DatabaseApi
{

    public class Program

    {

        public static void Main(string[] args)

        {
            // Create a host, build it, and run the application
            CreateHostBuilder(args).Build().Run();
        }

        // Method to create an instance of IHostBuilder

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            // Default host builder
            Host.CreateDefaultBuilder(args)
                // Configure the web host using default settings
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // Specify Startup class for web app
                    webBuilder.UseStartup<Startup>();
                });

    }
}


