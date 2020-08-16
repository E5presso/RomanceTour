using RomanceTour.Middlewares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RomanceTour
{
	public class Session
	{
		public const string Id = "SessionID";
		public const string Name = "UserName";
		public static bool IsAdministrator(int? userId) => XmlConfiguration.Administrators.SingleOrDefault(x => x.Id == userId) != null;
	}
}