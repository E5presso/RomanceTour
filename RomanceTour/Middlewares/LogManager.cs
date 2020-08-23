using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Core.Collections;

using RomanceTour.Models;

namespace RomanceTour.Middlewares
{
	public static class LogManager
	{
		private static readonly RingBuffer<(DateTime, string)> queue = new RingBuffer<(DateTime, string)>();
		private static readonly Thread WriteLog;

		static LogManager()
		{
			WriteLog = new Thread(new ThreadStart(() =>
			{
				while (true)
				{
					if (queue.Count > 0)
					{
						(var time, var message) = queue.Read();
						var path = $@"{XmlConfiguration.LogDirectory}\log-{time.ToShortDateString()}.log";
						using FileStream file = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
						using StreamWriter writer = new StreamWriter(file);
						writer.WriteLine(message);
					}
					Thread.Sleep(100);
				}
			}));
			WriteLog.Start();
		}
		public static async Task LogAsync(string ip, int? userId, string controller, string action, string parameter)
		{
			try
			{
				var now = DateTime.Now;
				using var db = new RomanceTourDbContext();
				if (Session.IsAdministrator(userId))
				{
					db.Log.Add(new Log
					{
						TimeStamp = now,
						IpAddress = ip,
						IsAdministrator = true,
						Controller = controller,
						Action = action,
						Parameter = parameter
					});
				}
				else
				{
					db.Log.Add(new Log
					{
						TimeStamp = now,
						IpAddress = ip,
						IsAdministrator = false,
						UserId = userId,
						Controller = controller,
						Action = action,
						Parameter = parameter
					});
				}
				await db.SaveChangesAsync().ConfigureAwait(false);
				if (userId != null)
				{
					if (Session.IsAdministrator(userId)) queue.Write((now, $"[LOG]\t{now}\t{XmlConfiguration.Administrator.UserName}@{ip}\t{($"{controller}/{action}")}{parameter}"));
					else queue.Write((now, $"[LOG]\t{now}\t{db.User.SingleOrDefault(x => x.Id == userId).UserName}@{ip}\t{($"{controller}/{action}")}{parameter}"));
				}
				else queue.Write((now, $"[LOG]\t{now}\t{ip}\t\t{($"{controller}/{action}")}{parameter}"));
			}
			catch (Exception e)
			{
				await ErrorAsync(e);
			}
		}
		public static void Log(string client, string type, string url, string query = "")
		{
			lock (queue)
			{
				var now = DateTime.Now;
				queue.Write((now, $"[LOG]\t{now}\t{client}\t{type}\t{url}{(query == "" ? "" : $"?{query}")}"));
			}
		}
		public static void Log(string client, string type, string username, string url, string query = "")
		{
			lock (queue)
			{
				var now = DateTime.Now;
				queue.Write((now, $"[LOG]\t{now}\t{client}\t{type}\t{username}\t{url}{(query == "" ? "" : $"?{query}")}"));
			}
		}
		public static async Task ErrorAsync(Exception e)
		{
			try
			{
				using var db = new RomanceTourDbContext();
				db.Error.Add(new Error
				{
					TimeStamp = DateTime.Now,
					Code = e is ExternalException ex ? ex.ErrorCode : -1,
					Message = e.Message,
					StackTrace = e.StackTrace
				});
				await db.SaveChangesAsync().ConfigureAwait(false);
				lock (queue)
				{
					var now = DateTime.Now;
					queue.Write((now, $"[ERROR]\t{now}\t{e.Message}\t{e.StackTrace}"));
				}
			}
			catch { throw e; }
		}
	}
}