using System;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using RomanceTour.Models;

namespace RomanceTour
{
	public class Startup
	{
		private readonly IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromHours(1);
			});
			services.AddResponseCompression();
			services.AddControllers().AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
				options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
				options.JsonSerializerOptions.PropertyNamingPolicy = null;
			});
			services.AddControllersWithViews();
			services.AddRazorPages().AddRazorRuntimeCompilation();
			services.AddDbContext<RomanceTourDbContext>();
		}
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}
			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseRouting();
			app.UseAuthorization();
			app.UseSession();
			app.UseResponseCompression();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute
				(
					name: "default",
					pattern: "{controller=Home}/{action=Index}"
				);
			});
		}
	}
}