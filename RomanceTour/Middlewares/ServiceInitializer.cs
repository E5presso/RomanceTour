using System.IO;
using RomanceTour.Models;

namespace RomanceTour.Middlewares
{
	public static class ServiceInitializer
	{
		public static void InitializeAsync()
		{
			if (!Directory.Exists(XmlConfiguration.RootDirectory)) Directory.CreateDirectory(XmlConfiguration.RootDirectory);
			if (!Directory.Exists(XmlConfiguration.FormDirectory)) Directory.CreateDirectory(XmlConfiguration.FormDirectory);
			if (!Directory.Exists(XmlConfiguration.FileDirectory)) Directory.CreateDirectory(XmlConfiguration.FileDirectory);
			if (!Directory.Exists(XmlConfiguration.LogDirectory)) Directory.CreateDirectory(XmlConfiguration.LogDirectory);
		}
	}
}