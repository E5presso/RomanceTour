using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using RomanceTour.Middlewares;

namespace RomanceTour
{
	public class Program
	{
		public static void Main(string[] args)
		{
			ServiceInitializer.InitializeAsync();
			CreateHostBuilder(args).Build().Run();
		}
		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}