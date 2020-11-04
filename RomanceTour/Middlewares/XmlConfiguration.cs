using System;
using System.Collections.Generic;
using System.Xml;

namespace RomanceTour.Middlewares
{
	public class Administrator
	{
		public int Id { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string HashSalt { get; set; }
		public string Name { get; set; }
	}
	public class NCloudAPI
	{
		public string URL { get; set; }
		public string URI { get; set; }
		public string RequestURI { get; set; }
		public string MessageURI { get; set; }
		public string ServiceId { get; set; }
		public string AccessKey { get; set; }
		public string SecretKey { get; set; }
		public string From { get; set; }
	}
	public class Verification
	{
		public string Template { get; set; }
		public int TimeLimit { get; set; }
		public int CodeLength { get; set; }
		public int MaxRequest { get; set; }
	}
	public static class XmlConfiguration
	{
		private static readonly string ConfigPath = "config.xml";
		private static readonly XmlDocument config = new XmlDocument();

		public static string Domain => config["configuration"]["domain"].InnerText;
		public static string Key
		{
			get => config["configuration"]["key"].InnerText;
			set => config["configuration"]["key"].InnerText = value;
		}

		public static string RootDirectory => config["configuration"]["directories"]["root"].InnerText;
		public static string FormDirectory => config["configuration"]["directories"]["form"].InnerText;
		public static string FileDirectory => config["configuration"]["directories"]["file"].InnerText;
		public static string LogDirectory => config["configuration"]["directories"]["log"].InnerText;

		public static Administrator Administrator => new Administrator
		{
			Id = int.Parse(config["configuration"]["administrator"]["session-id"].InnerText),
			UserName = config["configuration"]["administrator"]["username"].InnerText,
			Password = config["configuration"]["administrator"]["password"].InnerText,
			HashSalt = config["configuration"]["administrator"]["hashsalt"].InnerText,
			Name = config["configuration"]["administrator"]["name"].InnerText
		};
		public static NCloudAPI NCloudAPI => new NCloudAPI
		{
			URL = config["configuration"]["ncloud"]["url"].InnerText,
			URI = config["configuration"]["ncloud"]["uri"].InnerText,
			ServiceId = config["configuration"]["ncloud"]["service-id"].InnerText,
			AccessKey = config["configuration"]["ncloud"]["access-key"].InnerText,
			SecretKey = config["configuration"]["ncloud"]["sceret-key"].InnerText,
			From = config["configuration"]["ncloud"]["from"].InnerText
		};
		public static Verification Verification => new Verification
		{
			Template = config["configuration"]["verification"]["template"].InnerText,
			TimeLimit = int.Parse(config["configuration"]["verification"]["time-limit"].InnerText),
			CodeLength = int.Parse(config["configuration"]["verification"]["code-length"].InnerText),
			MaxRequest = int.Parse(config["configuration"]["verification"]["max-request"].InnerText)
		};
		public static string AppointmentSubject => config["configuration"]["appointment"]["subject"].InnerText;
		public static string AppointmentTemplate => config["configuration"]["appointment"]["template"].InnerText;

		public static string KakaoAPI => config["configuration"]["kakao"]["api-key"].InnerText;

		static XmlConfiguration()
		{
			config.Load(ConfigPath);
		}

		public static void SaveChanges()
		{
			config.Save(ConfigPath);
		}
	}
}