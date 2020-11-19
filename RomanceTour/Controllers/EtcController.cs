using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RomanceTour.Middlewares;
using RomanceTour.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RomanceTour.Controllers
{
	public class EtcController : BaseController
	{
		public async Task<IActionResult> ListCategory()
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					var matched = await db.Category.ToArrayAsync();
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
					Error = e,
					Result = ResultType.SYSTEM_ERROR
				});
			}
		}
		public async Task<IActionResult> ListBilling()
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					var matched = await db.Billing.ToArrayAsync();
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
					Error = e,
					Result = ResultType.SYSTEM_ERROR
				});
			}
		}
		public async Task<IActionResult> ListDeparture()
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					var matched = await db.Departure.ToArrayAsync();
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
					Error = e,
					Result = ResultType.SYSTEM_ERROR
				});
			}
		}
		public async Task<IActionResult> ListPriceRule()
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					var matched = await db.PriceRule.ToArrayAsync();
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
					Error = e,
					Result = ResultType.SYSTEM_ERROR
				});
			}
		}
		public async Task<IActionResult> ListHost()
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					var matched = await db.Host.ToArrayAsync();
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
					Error = e,
					Result = ResultType.SYSTEM_ERROR
				});
			}
		}

		public async Task<IActionResult> FilterCategory(string keyword)
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					var matched = await db.Category
						.Where(x =>
							EF.Functions.Like(x.Name, $"%{keyword}%") ||
							EF.Functions.Like(x.Description, $"%{keyword}%")
						)
						.ToArrayAsync();
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
					Error = e,
					Result = ResultType.SYSTEM_ERROR
				});
			}
		}
		public async Task<IActionResult> FilterBilling(string keyword)
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					var matched = await db.Billing
						.Where(x =>
							EF.Functions.Like(x.Name, $"%{keyword}%") ||
							EF.Functions.Like(x.Bank, $"%{keyword}%") ||
							EF.Functions.Like(x.Number, $"%{keyword}%")
						)
						.ToArrayAsync();
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
					Error = e,
					Result = ResultType.SYSTEM_ERROR
				});
			}
		}
		public async Task<IActionResult> FilterDeparture(string keyword)
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					var matched = await db.Departure
						.Where(x => EF.Functions.Like(x.Name, $"%{keyword}%"))
						.ToArrayAsync();
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
					Error = e,
					Result = ResultType.SYSTEM_ERROR
				});
			}
		}
		public async Task<IActionResult> FilterPriceRule(string keyword)
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					var matched = await db.PriceRule
						.Where(x =>
							EF.Functions.Like(x.RuleName, $"%{keyword}%") ||
							EF.Functions.Like(x.Description, $"%{keyword}%")
						)
						.ToArrayAsync();
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
					Error = e,
					Result = ResultType.SYSTEM_ERROR
				});
			}
		}
		public async Task<IActionResult> FilterHost(string keyword)
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					var matched = await db.Host
						.Where(x =>
							EF.Functions.Like(x.Name, $"%{keyword}%") ||
							EF.Functions.Like(x.Address, $"%{keyword}%") ||
							EF.Functions.Like(x.HostName, $"%{keyword}%") ||
							EF.Functions.Like(x.HostPhone, $"%{keyword}%") ||
							EF.Functions.Like(x.HostBank, $"%{keyword}%") ||
							EF.Functions.Like(x.HostBillingNumber, $"%{keyword}%")
						)
						.ToArrayAsync();
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
					Error = e,
					Result = ResultType.SYSTEM_ERROR
				});
			}
		}

		public async Task<IActionResult> GetCategory(int id)
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					var matched = await db.Category.SingleOrDefaultAsync(x => x.Id == id);
					if (matched != null)
					{
						return Json(new Response
						{
							Result = ResultType.SUCCESS,
							Model = new
							{
								Result = true,
								Data = matched
							}
						});
					}
					else return Json(new Response
					{
						Result = ResultType.SUCCESS,
						Model = new
						{
							Result = false
						}
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
		public async Task<IActionResult> GetBilling(int id)
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					var matched = await db.Billing.SingleOrDefaultAsync(x => x.Id == id);
					if (matched != null)
					{
						return Json(new Response
						{
							Result = ResultType.SUCCESS,
							Model = new
							{
								Result = true,
								Data = matched
							}
						});
					}
					else return Json(new Response
					{
						Result = ResultType.SUCCESS,
						Model = new
						{
							Result = false
						}
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
		public async Task<IActionResult> GetDeparture(int id)
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					var matched = await db.Departure.SingleOrDefaultAsync(x => x.Id == id);
					if (matched != null)
					{
						return Json(new Response
						{
							Result = ResultType.SUCCESS,
							Model = new
							{
								Result = true,
								Data = matched
							}
						});
					}
					else return Json(new Response
					{
						Result = ResultType.SUCCESS,
						Model = new
						{
							Result = false
						}
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
		public async Task<IActionResult> GetPriceRule(int id)
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					var matched = await db.PriceRule.SingleOrDefaultAsync(x => x.Id == id);
					if (matched != null)
					{
						return Json(new Response
						{
							Result = ResultType.SUCCESS,
							Model = new
							{
								Result = true,
								Data = matched
							}
						});
					}
					else return Json(new Response
					{
						Result = ResultType.SUCCESS,
						Model = new
						{
							Result = false
						}
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
		public async Task<IActionResult> GetHost(int id)
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					var matched = await db.Host.SingleOrDefaultAsync(x => x.Id == id);
					if (matched != null)
					{
						return Json(new Response
						{
							Result = ResultType.SUCCESS,
							Model = new
							{
								Result = true,
								Data = matched
							}
						});
					}
					else return Json(new Response
					{
						Result = ResultType.SUCCESS,
						Model = new
						{
							Result = false
						}
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

		public async Task<IActionResult> AddCategory(Category category)
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					db.Category.Add(category);
					await db.SaveChangesAsync();
					return Json(new Response
					{
						Result = ResultType.SUCCESS
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
		public async Task<IActionResult> AddBilling(Billing billing)
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					db.Billing.Add(billing);
					await db.SaveChangesAsync();
					return Json(new Response
					{
						Result = ResultType.SUCCESS
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
		public async Task<IActionResult> AddDeparture(Departure departure)
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					db.Departure.Add(departure);
					await db.SaveChangesAsync();
					return Json(new Response
					{
						Result = ResultType.SUCCESS
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
		public async Task<IActionResult> AddPriceRule(PriceRule pricerule)
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					db.PriceRule.Add(pricerule);
					await db.SaveChangesAsync();
					return Json(new Response
					{
						Result = ResultType.SUCCESS
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
		public async Task<IActionResult> AddHost(Host host)
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					db.Host.Add(host);
					await db.SaveChangesAsync();
					return Json(new Response
					{
						Result = ResultType.SUCCESS
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
		
		public async Task<IActionResult> UpdateCategory(int id, Category category)
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					var matched = await db.Category.SingleOrDefaultAsync(x => x.Id == id);
					if (matched != null)
					{
						matched.Name = category.Name;
						matched.Description = category.Description;
						db.Category.Update(matched);
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
					Error = e,
					Result = ResultType.SYSTEM_ERROR
				});
			}
		}
		public async Task<IActionResult> UpdateBilling(int id, Billing billing)
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					var matched = await db.Billing.SingleOrDefaultAsync(x => x.Id == id);
					if (matched != null)
					{
						matched.Name = billing.Name;
						matched.Bank = billing.Bank;
						matched.Number = billing.Number;
						db.Billing.Update(matched);
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
					Error = e,
					Result = ResultType.SYSTEM_ERROR
				});
			}
		}
		public async Task<IActionResult> UpdateDeparture(int id, Departure departure)
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					var matched = await db.Departure.SingleOrDefaultAsync(x => x.Id == id);
					if (matched != null)
					{
						matched.Name = departure.Name;
						matched.Latitude = departure.Latitude;
						matched.Longitude = departure.Longitude;
						db.Departure.Update(matched);
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
					Error = e,
					Result = ResultType.SYSTEM_ERROR
				});
			}
		}
		public async Task<IActionResult> UpdatePriceRule(int id, PriceRule pricerule)
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					var matched = await db.PriceRule.SingleOrDefaultAsync(x => x.Id == id);
					if (matched != null)
					{
						matched.RuleType = pricerule.RuleType;
						matched.RuleName = pricerule.RuleName;
						matched.Description = pricerule.Description;
						matched.Price = pricerule.Price;
						db.PriceRule.Update(matched);
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
					Error = e,
					Result = ResultType.SYSTEM_ERROR
				});
			}
		}
		public async Task<IActionResult> UpdateHost(int id, Host host)
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					var matched = await db.Host.SingleOrDefaultAsync(x => x.Id == id);
					if (matched != null)
					{
						matched.Type = host.Type;
						matched.Name = host.Name;
						matched.Address = host.Address;
						matched.HostName = host.HostName;
						matched.HostPhone = host.HostPhone;
						matched.HostBank = host.HostBank;
						matched.HostBillingNumber = host.HostBillingNumber;
						db.Host.Update(matched);
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
					Error = e,
					Result = ResultType.SYSTEM_ERROR
				});
			}
		}

		public async Task<IActionResult> RemoveCategory(int id)
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					var matched = await db.Category.SingleOrDefaultAsync(x => x.Id == id);
					if (matched != null)
					{
						db.Category.Remove(matched);
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
					Error = e,
					Result = ResultType.SYSTEM_ERROR
				});
			}
		}
		public async Task<IActionResult> RemoveBilling(int id)
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					var matched = await db.Billing.SingleOrDefaultAsync(x => x.Id == id);
					if (matched != null)
					{
						db.Billing.Remove(matched);
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
					Error = e,
					Result = ResultType.SYSTEM_ERROR
				});
			}
		}
		public async Task<IActionResult> RemoveDeparture(int id)
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					var matched = await db.Departure.SingleOrDefaultAsync(x => x.Id == id);
					if (matched != null)
					{
						db.Departure.Remove(matched);
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
					Error = e,
					Result = ResultType.SYSTEM_ERROR
				});
			}
		}
		public async Task<IActionResult> RemovePriceRule(int id)
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					var matched = await db.PriceRule.SingleOrDefaultAsync(x => x.Id == id);
					if (matched != null)
					{
						db.PriceRule.Remove(matched);
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
					Error = e,
					Result = ResultType.SYSTEM_ERROR
				});
			}
		}
		public async Task<IActionResult> RemoveHost(int id)
		{
			try
			{
				if (IsAdministrator)
				{
					var db = new RomanceTourDbContext();
					var matched = await db.Host.SingleOrDefaultAsync(x => x.Id == id);
					if (matched != null)
					{
						db.Host.Remove(matched);
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
					Error = e,
					Result = ResultType.SYSTEM_ERROR
				});
			}
		}
	}
}