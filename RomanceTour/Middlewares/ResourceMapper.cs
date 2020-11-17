using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace RomanceTour.Middlewares
{
	public static class ResourceMapper
	{
		private static readonly string path = @"resource-map.xml";
		private static readonly XmlDocument config = new XmlDocument();

		static ResourceMapper()
		{
			config.Load(path);
		}
		public static string GetControllerResource(string controllerName) => config["resource-map"]["controller"][controllerName].InnerText;
		public static string GetActionResource(string actionName) => config["resource-map"]["action"][actionName].InnerText;
	}
}