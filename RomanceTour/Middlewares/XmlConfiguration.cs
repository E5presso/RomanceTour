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
		public string ServiceId { get; set; }
		public string AccessKey { get; set; }
		public string SecretKey { get; set; }
		public string From { get; set; }
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

		public static Administrator[] Administrators
		{
			get
			{
				var administrators = new List<Administrator>();
				var list = config["configuration"]["administrators"].GetElementsByTagName("account");
				for (int i = 0; i < list.Count; i++)
				{
					administrators.Add(new Administrator
					{
						Id = Convert.ToInt32(list.Item(i)["sessionid"].InnerText),
						UserName = list.Item(i)["username"].InnerText,
						Password = list.Item(i)["password"].InnerText,
						HashSalt = list.Item(i)["hashsalt"].InnerText,
						Name = list.Item(i)["name"].InnerText
					});
				}
				return administrators.ToArray();
			}
		}
		public static NCloudAPI NCloudAPI => new NCloudAPI
		{
			URL = config["configuration"]["ncloud"]["url"].InnerText,
			URI = config["configuration"]["ncloud"]["uri"].InnerText,
			ServiceId = config["configuration"]["ncloud"]["service-id"].InnerText,
			AccessKey = config["configuration"]["ncloud"]["access-key"].InnerText,
			SecretKey = config["configuration"]["ncloud"]["secret-key"].InnerText,
			From = config["configuration"]["ncloud"]["from"].InnerText
		};
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