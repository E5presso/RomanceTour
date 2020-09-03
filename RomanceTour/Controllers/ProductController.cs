using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using RomanceTour.Models;
using RomanceTour.Middlewares;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Http;
using RomanceTour.ViewModels;
using System.Collections.Generic;

namespace RomanceTour.Controllers
{
	public class ProductController : BaseController
	{
		public async Task<IActionResult> ListProduct(int category, DateTime? date, string keyword, bool? confirmed)
		{
			try
			{
				using var db = new RomanceTourDbContext();

				ViewBag.CategoryList = await db.Category.Where(x => true).ToListAsync();
				ViewBag.Category = category;
				ViewBag.Date = date;
				ViewBag.Keyword = keyword;
				ViewBag.Confirmed = confirmed;
				ViewBag.Back = Back;
				return View();
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return RedirectToAction("Error", "Home");
			}
		}
		public async Task<IActionResult> GetProduct(int id)
		{
			try
			{
				using var db = new RomanceTourDbContext();
				var matched = await db.Product
						.Include(x => x.Category)
						.Include(x => x.ProductBilling)
							.ThenInclude(x => x.Billing)
						.Include(x => x.ProductDeparture)
							.ThenInclude(x => x.Departure)
						.Include(x => x.ProductHost)
							.ThenInclude(x => x.Host)
						.Include(x => x.ProductPriceRule)
							.ThenInclude(x => x.PriceRule)
						.Include(x => x.DateSession)
						.SingleOrDefaultAsync(x => x.Id == id && x.Visible);
				if (matched != null)
				{
					ViewBag.Back = Back;
					return View(matched);
				}
				else return RedirectToAction("PageNotFound", "Home");
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return RedirectToAction("Error", "Home");
			}
		}

		public async Task<IActionResult> AddFile(bool action, string responseType, IFormFile upload)
		{
			try
			{
				responseType += action; // ???????
				if (upload.Length <= 0) return Json(new
				{
					uploaded = 0,
					error = new
					{
						message = "사진을 선택해주세요."
					}
				});
				var fileName = Guid.NewGuid() + Path.GetExtension(upload.FileName).ToLower();
				var path = Path.Combine(XmlConfiguration.FileDirectory, fileName);
				using var stream = new FileStream(path, FileMode.Create);
				await upload.CopyToAsync(stream);

				return Json(new
				{
					uploaded = 1,
					fileName,
					url = $@"/Product/GetFile?fileName={fileName}",
					error = new
					{
						message = "업로드되었습니다."
					}
				});
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return Json(new Response
				{
					Result = ResultType.SYSTEM_ERROR,
					Error = e
				});
			}
		}

		public async Task<IActionResult> GetFile(string fileName)
		{
			try
			{
				if (fileName != null)
				{
					var path = Path.Combine(XmlConfiguration.FileDirectory, fileName);
					if (System.IO.File.Exists(path)) return PhysicalFile(path, Utilities.GetMimeType(fileName), true);
					else return Json(new Response
					{
						Result = ResultType.NOT_FOUND
					});
				}
				else return Json(new Response
				{
					Result = ResultType.NOT_FOUND
				});
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return RedirectToAction("Error", "Home");
			}
		}
		public async Task<IActionResult> GetForm(string fileName)
		{
			try
			{
				if (fileName != null)
				{
					var path = Path.Combine(XmlConfiguration.FormDirectory, fileName);
					if (System.IO.File.Exists(path))
					{
						using FileStream stream = new FileStream(path, FileMode.Open);
						using StreamReader reader = new StreamReader(stream);
						var form = await reader.ReadToEndAsync();
						return Json(new Response
						{
							Result = ResultType.SUCCESS,
							Model = form
						});
					}
					else return Json(new Response
					{
						Result = ResultType.NOT_FOUND
					});
				}
				else return Json(new Response
				{
					Result = ResultType.NOT_FOUND
				});
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return RedirectToAction("Error", "Home");
			}
		}
		public async Task<IActionResult> GetAppointment(int id)
		{
			try
			{
				if (IsAdministrator)
				{
					using var db = new RomanceTourDbContext();

					var matched = await db.Product
						.Include(x => x.DateSession)
						.SingleOrDefaultAsync(x => x.Id == id);
					return Json(new Response
					{
						Result = ResultType.SUCCESS,
						Model = matched.DateSession.ToArray().Select(x => new
						{
							Date = x.Date.ConvertToJavascriptDate()
						})
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
					Result = ResultType.SYSTEM_ERROR,
					Error = e
				});
			}
		}

		public async Task<IActionResult> AjaxGetProduct(int id)
		{
			try
			{
				if (IsAdministrator)
				{
					using var db = new RomanceTourDbContext();

					var matched = await db.Product.SingleOrDefaultAsync(x => x.Id == id);
					return Json(new Response
					{
						Result = ResultType.SUCCESS,
						Model = matched
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
					Result = ResultType.SYSTEM_ERROR,
					Error = e
				});
			}
		}
		public async Task<IActionResult> AjaxListProduct()
		{
			try
			{
				using var db = new RomanceTourDbContext();
				var array = await db.Product
					.Include(x => x.Category)
					.Where(x => true)
					.ToArrayAsync();
				return Json(new Response
				{
					Result = ResultType.SUCCESS,
					Model = array
				});
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return Json(new Response
				{
					Result = ResultType.SYSTEM_ERROR,
					Error = e
				});
			}
		}

		public async Task<IActionResult> FilterProduct(int category, ProductFilterVM filter)
		{
			try
			{
				using var db = new RomanceTourDbContext();
				var array = db.Product
					.Include(x => x.Category)
					.Include(x => x.DateSession)
						.ThenInclude(x => x.Appointment)
					.Where(x => category == 0 ? x.Visible : x.CategoryId == category && x.Visible)
					.Select(x => new ProductVM
					{
						Id = x.Id,
						CategoryId = x.CategoryId,
						CategoryName = x.Category.Name,
						Title = x.Title,
						SubTitle = x.SubTitle,
						Thumbnail = x.Thumbnail,
						Price = x.Price,
						Available = x.DateSession
							.Where(y => y.Status == DateSessionStatus.AVAILABLE)
							.OrderBy(y => y.Date)
							.Select(y => y.Date),
						FastAvailable = x.DateSession
							.Where(y => y.Status == DateSessionStatus.AVAILABLE)
							.Select(y => y.Date)
							.Min(),
						Confirmed = x.DateSession
							.Where(y => y.Status == DateSessionStatus.APPROVED)
							.Count() > 0
					});
				if (filter.Sorting != null) array = filter.Sorting switch
				{
					0 => array.OrderByDescending(x => x.Id),
					1 => array.OrderBy(x => x.Id),
					2 => array.OrderByDescending(x => x.Price),
					3 => array.OrderBy(x => x.Price),
					4 => array.OrderBy(x => x.FastAvailable),
					5 => array.OrderByDescending(x => x.FastAvailable),
					_ => array.OrderByDescending(x => x.Id)
				};
				if (filter.Keyword != null) array = array.Where(x =>
					EF.Functions.Like(x.Title, $"%{filter.Keyword}%")
					|| EF.Functions.Like(x.SubTitle, $"%{filter.Keyword}%"));
				if (filter.FromPrice != null && filter.ToPrice != null) array = array.Where(x => x.Price >= filter.FromPrice && x.Price <= filter.ToPrice);
				if (filter.Date != null) array = array.Where(x => x.Available.Any(x => x.Date.Year == ((DateTime)filter.Date).Year
					&& x.Date.Month == ((DateTime)filter.Date).Month
					&& x.Date.Day == ((DateTime)filter.Date).Day));
				if (filter.Confirmed != null) array = array.Where(x => !(bool)filter.Confirmed || x.Confirmed);

				return Json(new Response
				{
					Result = ResultType.SUCCESS,
					Model = await array.ToArrayAsync()
				});
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return Json(new Response
				{
					Result = ResultType.SYSTEM_ERROR,
					Error = e
				});
			}
		}
		public async Task<IActionResult> AddProduct(PostVM post)
		{
			try
			{
				if (IsAdministrator)
				{
					using var db = new RomanceTourDbContext();

					if (post.PriceRules == null) post.PriceRules = new List<int>();
					if (post.Departures == null) post.Departures = new List<int>();
					if (post.Hosts == null) post.Hosts = new List<int>();
					if (post.Billings == null) post.Billings = new List<int>();
					if (post.Appointments == null) post.Appointments = new List<DateTime>();

					var PriceRules = new List<ProductPriceRule>();
					var departures = new List<ProductDeparture>();
					var hosts = new List<ProductHost>();
					var billings = new List<ProductBilling>();
					var appointments = new List<DateSession>();

					foreach (var PriceRule in post.PriceRules)
						PriceRules.Add(new ProductPriceRule
						{
							PriceRuleId = PriceRule
						});
					foreach (var departure in post.Departures)
						departures.Add(new ProductDeparture
						{
							DepartureId = departure
						});
					foreach (var host in post.Hosts)
						hosts.Add(new ProductHost
						{
							HostId = host
						});
					foreach (var billing in post.Billings)
						billings.Add(new ProductBilling
						{
							BillingId = billing
						});
					foreach (var appointment in post.Appointments)
						appointments.Add(new DateSession
						{
							Date = appointment,
							Status = 0
						});

					var imageFileName = Guid.NewGuid() + Path.GetExtension(post.Thumbnail.FileName).ToLower();
					var imagePath = $"/Product/GetFile?fileName={imageFileName}";
					using var imageStream = new FileStream($@"{XmlConfiguration.FileDirectory}\{imageFileName}", FileMode.Create);
					await post.Thumbnail.CopyToAsync(imageStream);

					var formFileName = $@"{Guid.NewGuid()}.html";
					var formPath = $"/Product/GetForm?fileName={formFileName}";
					using var formStream = new FileStream($@"{XmlConfiguration.FormDirectory}\{formFileName}", FileMode.Create);
					using var formWriter = new StreamWriter(formStream);
					await formWriter.WriteAsync(post.Form);
					formWriter.Close();

					db.Product.Add(new Product
					{
						Title = post.Title,
						SubTitle = post.SubTitle,
						Thumbnail = imagePath,
						Price = post.Price,
						CategoryId = post.CategoryId,
						ProductPriceRule = PriceRules,
						ProductDeparture = departures,
						ProductHost = hosts,
						ProductBilling = billings,
						DateSession = appointments,
						Form = formPath
					});
					await db.SaveChangesAsync();
					return Json(new Response
					{
						Result = ResultType.SUCCESS,
						Model = true
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
					Result = ResultType.SYSTEM_ERROR,
					Error = e
				});
			}
		}
		public async Task<IActionResult> UpdateProduct(PostVM post)
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
						.Include(x => x.ProductHost)
						.Include(x => x.ProductPriceRule)
						.Include(x => x.DateSession)
							.ThenInclude(x => x.Appointment)
						.SingleOrDefaultAsync(x => x.Id == post.Id);

					if (post.PriceRules == null) post.PriceRules = new List<int>();
					if (post.Departures == null) post.Departures = new List<int>();
					if (post.Hosts == null) post.Hosts = new List<int>();
					if (post.Billings == null) post.Billings = new List<int>();
					if (post.Appointments == null) post.Appointments = new List<DateTime>();

					string fileName;
					string path;

					var imagePath = string.Empty;
					if (post.Thumbnail != null)
					{
						fileName = matched.Thumbnail.Split("=")[1];
						path = $@"{XmlConfiguration.FileDirectory}\{fileName}";
						if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
						var imageFileName = Guid.NewGuid() + Path.GetExtension(post.Thumbnail.FileName).ToLower();
						imagePath = $"/Product/GetFile?fileName={imageFileName}";
						using var imageStream = new FileStream($@"{XmlConfiguration.FileDirectory}\{imageFileName}", FileMode.Create);
						await post.Thumbnail.CopyToAsync(imageStream);
					}

					fileName = matched.Form.Split("=")[1];
					path = $@"{XmlConfiguration.FormDirectory}\{fileName}";
					if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
					var formFileName = $@"{Guid.NewGuid()}.html";
					var formPath = $"/Product/GetForm?fileName={formFileName}";
					using var formStream = new FileStream($@"{XmlConfiguration.FormDirectory}\{formFileName}", FileMode.Create);
					using var formWriter = new StreamWriter(formStream);
					await formWriter.WriteAsync(post.Form);
					formWriter.Close();

					matched.Title = post.Title;
					matched.SubTitle = post.SubTitle;
					matched.Thumbnail = imagePath != string.Empty ? imagePath : matched.Thumbnail;
					matched.Price = post.Price;
					matched.CategoryId = post.CategoryId;

					matched.ProductPriceRule.Clear();
					matched.ProductDeparture.Clear();
					matched.ProductBilling.Clear();
					matched.ProductHost.Clear();

					foreach (var PriceRule in post.PriceRules)
					{
						matched.ProductPriceRule.Add(new ProductPriceRule
						{
							PriceRuleId = PriceRule
						});
					}
					foreach (var departure in post.Departures)
					{
						matched.ProductDeparture.Add(new ProductDeparture
						{
							DepartureId = departure
						});
					}
					foreach (var billing in post.Billings)
					{
						matched.ProductBilling.Add(new ProductBilling
						{
							BillingId = billing
						});
					}
					foreach (var host in post.Hosts)
					{
						matched.ProductHost.Add(new ProductHost
						{
							HostId = host
						});
					}

					// OLD에 존재하나 NEW에 존재하지 않는 경우 제거
					var excluded = matched.DateSession.Where(x => !post.Appointments.Contains(x.Date));
					db.DateSession.RemoveRange(excluded);
					// NEW에 존재하나 OLD에 존재하지 않는 경우 추가
					foreach (var appointment in post.Appointments)
					{
						var session = matched.DateSession.SingleOrDefault(x => x.Date == appointment);
						if (session == null)
						{
							matched.DateSession.Add(new DateSession
							{
								Date = appointment,
								Status = DateSessionStatus.AVAILABLE
							});
						}
					}
					matched.Form = formPath;

					db.Product.Update(matched);
					await db.SaveChangesAsync();

					return Json(new Response
					{
						Result = ResultType.SUCCESS,
						Model = true
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
					Result = ResultType.SYSTEM_ERROR,
					Error = e
				});
			}
		}
		public async Task<IActionResult> RemoveProduct(int id)
		{
			try
			{
				if (IsAdministrator)
				{
					using var db = new RomanceTourDbContext();
					var matched = await db.Product
						.Include(x => x.Category)
						.Include(x => x.ProductBilling)
							.ThenInclude(x => x.Billing)
						.Include(x => x.ProductDeparture)
							.ThenInclude(x => x.Departure)
						.Include(x => x.ProductHost)
							.ThenInclude(x => x.Host)
						.Include(x => x.ProductPriceRule)
							.ThenInclude(x => x.PriceRule)
						.Include(x => x.DateSession)
						.SingleOrDefaultAsync(x => x.Id == id);
					if (matched != null)
					{
						var imageFile = matched.Thumbnail.Split("=")[1];
						var imagePath = $@"{XmlConfiguration.FileDirectory}\{imageFile}";
						var formFile = matched.Form.Split("=")[1];
						var formPath = $@"{XmlConfiguration.FormDirectory}\{formFile}";

						if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);
						if (System.IO.File.Exists(formPath)) System.IO.File.Delete(formPath);

						db.Product.Remove(matched);
						await db.SaveChangesAsync();
						return Json(new Response
						{
							Result = ResultType.SUCCESS,
							Model = true
						});
					}
					else return Json(new Response
					{
						Result = ResultType.SUCCESS,
						Model = false
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
					Result = ResultType.SYSTEM_ERROR,
					Error = e
				});
			}
		}
	}
}