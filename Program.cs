using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace LaoS
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new WebHostBuilder()
             .UseKestrel()
             .UseUrls("http://*:80")
             .UseStartup<Startup>()
             .Build();

            host.Run();
        }
    }
}