using System.Xml;

namespace RomanceTour.Middlewares
{
	public static class TermsAndConditions
	{
		private static readonly string ConfigPath = "terms-and-conditions.xml";
		private static readonly XmlDocument config = new XmlDocument();

		public static string Document => config["terms"].InnerXml;

		static TermsAndConditions()
		{
			config.Load(ConfigPath);
		}
	}
}