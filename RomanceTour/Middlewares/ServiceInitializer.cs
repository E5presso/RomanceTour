using System;
using System.IO;
using System.Linq;
using Core.Security;
using Core.Utility;
using Microsoft.EntityFrameworkCore;
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

			// 휴면계정 관리
			Function.SetInterval(async () =>
			{
				var db = new RomanceTourDbContext();
				var matched = await db.User.Where(x => x.LastLogin.AddMonths(6) <= DateTime.Now).ToListAsync();
				matched.ForEach(x => x.Status = UserStatus.GREY);
				db.User.UpdateRange(matched);
				await db.SaveChangesAsync();
			}, TimeSpan.FromDays(1));
			// 지나간 예약 정리
			Function.SetInterval(async () =>
			{
				var db = new RomanceTourDbContext();
				var products = await db.Product
					.Include(x => x.DateSession)
					.ToListAsync();
				products.ForEach(product =>
				{
					var matched = product.DateSession.Where(x => x.Date < DateTime.Now).ToList();
					matched.ForEach(session => product.DateSession.Remove(session));
				});
				db.Product.UpdateRange(products);
				await db.SaveChangesAsync();
			}, TimeSpan.FromDays(1));
			// 암호화 키 교체
			Function.SetInterval(() =>
			{
				var db = new RomanceTourDbContext();
				var users = db.User.ToList();
				var appointments = db.Appointment.ToList();
				var hosts = db.Host.ToList();
				XmlConfiguration.SecretKey = KeyGenerator.GenerateString(32);
				XmlConfiguration.SaveChanges();
				db.User.UpdateRange(users);
				db.Appointment.UpdateRange(appointments);
				db.Host.UpdateRange(hosts);
				db.SaveChanges();
			}, TimeSpan.FromDays(15));
		}
	}
}