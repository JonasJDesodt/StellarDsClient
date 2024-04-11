using System.Diagnostics;

namespace StellarDsClient.BuilderApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();




            Process.Start(new ProcessStartInfo
            {
                FileName = "https://www.vrtnws.be",
                UseShellExecute = true
            });

            app.Run();
        }
    }
}
