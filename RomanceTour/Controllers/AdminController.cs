using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RomanceTour.Models;
using RomanceTour.Middlewares;

namespace RomanceTour.Controllers
{
	public class AdminController : BaseController
	{
		public async Task<IActionResult> Dashboard()
		{
			try
			{
				if (IsAdministrator)
				{
					ViewBag.Back = Back;
					return View();
				}
				else return RedirectToAction("AccessDenied", "Home");
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return RedirectToAction("Error", "Home");
			}
		}
		public async Task<IActionResult> ManageUser()
		{
			try
			{
				if (IsAdministrator)
				{
					ViewBag.Back = Back;
					return View();
				}
				else return RedirectToAction("AccessDenied", "Home");
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return RedirectToAction("Error", "Home");
			}
		}
		public async Task<IActionResult> ManageAppointment()
		{
			try
			{
				if (IsAdministrator)
				{
					using var db = new RomanceTourDbContext();
					var list = await db.Category.Where(x => true).ToListAsync();
					var departure = await db.Departure.Where(x => true).ToListAsync();
					var PriceRule = await db.PriceRule.Where(x => true).ToListAsync();

					ViewBag.Back = Back;
					ViewBag.Departure = departure;
					ViewBag.PriceRule = PriceRule;
					return View(list);
				}
				else return RedirectToAction("AccessDenied", "Home");
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return RedirectToAction("Error", "Home");
			}
		}
		public async Task<IActionResult> ManageProduct()
		{
			try
			{
				if (IsAdministrator)
				{
					using var db = new RomanceTourDbContext();
					var list = await db.Category.Where(x => true).ToListAsync();

					ViewBag.Back = Back;
					return View(list);
				}
				else return RedirectToAction("AccessDenied", "Home");
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return RedirectToAction("Error", "Home");
			}
		}
		public async Task<IActionResult> ManageEtc()
		{
			try
			{
				if (IsAdministrator)
				{
					using var db = new RomanceTourDbContext();
					ViewBag.Back = Back;
					return View();
				}
				else return RedirectToAction("AccessDenied", "Home");
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return RedirectToAction("Error", "Home");
			}
		}
		public async Task<IActionResult> WriteProduct()
		{
			try
			{
				if (IsAdministrator)
				{
					using var db = new RomanceTourDbContext();
					var billing = await db.Billing.Where(x => true).ToListAsync();
					var category = await db.Category.Where(x => true).ToListAsync();
					var departure = await db.Departure.Where(x => true).ToListAsync();
					var PriceRule = await db.PriceRule.Where(x => true).ToListAsync();
					var host = await db.Host.Where(x => true).ToListAsync();

					ViewBag.Billing = billing;
					ViewBag.Category = category;
					ViewBag.Departure = departure;
					ViewBag.PriceRule = PriceRule;
					ViewBag.Host = host;

					ViewBag.Back = Back;
					return View();
				}
				else return RedirectToAction("AccessDenied", "Home");
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return RedirectToAction("Error", "Home");
			}
		}
		public async Task<IActionResult> EditProduct(int id)
		{
			try
			{
				if (IsAdministrator)
				{
					using var db = new RomanceTourDbContext();
					var matched = await db.Product
						.Include(x => x.Category)
						.Include(x => x.ProductBilling)
						.Include(x => x.ProductDeparture)
						.Include(x => x.ProductPriceRule)
						.Include(x => x.ProductHost)
						.Include(x => x.DateSession)
						.SingleOrDefaultAsync(x => x.Id == id);

					var billing = await db.Billing.Where(x => true).ToListAsync();
					var category = await db.Category.Where(x => true).ToListAsync();
					var departure = await db.Departure.Where(x => true).ToListAsync();
					var priceRule = await db.PriceRule.Where(x => true).ToListAsync();
					var host = await db.Host.Where(x => true).ToListAsync();

					ViewBag.Billing = billing;
					ViewBag.Category = category;
					ViewBag.Departure = departure;
					ViewBag.PriceRule = priceRule;
					ViewBag.Host = host;

					ViewBag.Back = Back;
					return View(matched);
				}
				else return RedirectToAction("AccessDenied", "Home");
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return RedirectToAction("Error", "Home");
			}
		}
		public async Task<IActionResult> SendMessage()
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					ViewBag.Users = await db.User.Where(x => x.AllowMarketingPromotions).ToListAsync();
					return View();
				}
				else return RedirectToAction("AccessDenied", "Home");
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return RedirectToAction("Error", "Home");
			}
		}

		public async Task<IActionResult> SendCustomMessage(string[] contacts, string subject, string content)
		{
			try
			{
				if (IsAdministrator)
				{
					var result = await MessageSender.SendCustomMessage(contacts, subject, content);
					return Json(new Response
					{
						Result = ResultType.SUCCESS,
						Model = result
					});
				}
				else return Json(new Response
				{
					Result = ResultType.ACCESS_DENIED
				});
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return Json(new Response
				{
					Error = e,
					Result = ResultType.SYSTEM_ERROR
				});
			}
		}
	}
}