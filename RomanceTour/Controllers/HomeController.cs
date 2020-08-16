using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RomanceTour.Models;
using RomanceTour.Middlewares;

namespace RomanceTour.Controllers
{
	public class HomeController : BaseController
	{
		public async Task<IActionResult> Index()
		{
			try
			{
				using var db = new RomanceTourDbContext();
				var list = await db.Category.Where(x => true).ToListAsync();
				var product = await db.Product
					.Include(x => x.Category)
					.Where(x => x.Visible && x.Expose)
					.ToListAsync();

				ViewBag.Back = Back;
				ViewBag.List = list;
				return View(product);
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return RedirectToAction("Error", "Home");
			}
		}
		public async Task<IActionResult> AccessDenied()
		{
			try
			{
				return View();
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return RedirectToAction("Error", "Home");
			}
		}
		public async Task<IActionResult> LoginRequired()
		{
			try
			{
				if (IsLoggedIn) return RedirectToAction("Index", "Home");
				else return View();
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return RedirectToAction("Error", "Home");
			}
		}
		public async Task<IActionResult> PageNotFound()
		{
			try
			{
				return View();
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return RedirectToAction("Error", "Home");
			}
		}
		public async Task<IActionResult> Error()
		{
			try
			{
				return View();
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return RedirectToAction("Index", "Home");
			}
		}

		public async Task<IActionResult> ExtendSessionTime()
		{
			try
			{
				if (IsLoggedIn) return Json(new Response
				{
					Result = ResultType.SUCCESS,
					Model = true
				});
				else return Json(new Response
				{
					Result = ResultType.SUCCESS,
					Model = false
				});
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return RedirectToAction("Error", "Home");
			}
		}
	}
}