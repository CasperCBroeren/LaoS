using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace LaoS
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new WebHostBuilder()            
             .UseContentRoot(Path.Combine(Directory.GetCurrentDirectory(),"wwwroot"))
             .UseKestrel()
             .UseUrls("http://*:5000")
             .UseStartup<Startup>()
             .Build();

            host.Run();
        }
    }
}