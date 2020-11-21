using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace RomanceTour.Middlewares
{
	public static class CompanyInformation
	{
		private static readonly string ConfigPath = "company-info.xml";
		private static readonly XmlDocument config = new XmlDocument();

		public static string Name => config["information"]["name"].InnerText;
		public static string Representative => config["information"]["representative"].InnerText;
		public static string Address => config["information"]["address"].InnerText;
		public static string CompanyRegistrationNumber => config["information"]["company-registration-number"].InnerText;
		public static string Telephone => config["information"]["telephone"].InnerText;
		public static string Fax => config["information"]["fax"].InnerText;
		public static string Email => config["information"]["e-mail"].InnerText;
		public static string TelemarketingRegistrationNumber => config["information"]["telemarketing-registration-number"].InnerText;
		public static string Copyright => config["information"]["copyright"].InnerText;

		static CompanyInformation()
		{
			config.Load(ConfigPath);
		}
	}
}