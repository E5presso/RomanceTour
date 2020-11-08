using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Core.Security;
using RomanceTour.Models;
using RomanceTour.Middlewares;
using System.Linq;

namespace RomanceTour.Controllers
{
	public class UserController : BaseController
	{
		public async Task<IActionResult> Login()
		{
			try
			{
				if (!IsLoggedIn)
				{
					ViewBag.Back = Back;
					return View();
				}
				else return RedirectToAction("Index", "Home");
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return RedirectToAction("Error", "Home");
			}
		}
		public async Task<IActionResult> Register()
		{
			try
			{
				if (!IsLoggedIn)
				{
					ViewBag.Back = Back;
					return View();
				}
				else return RedirectToAction("Index", "Home");
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return RedirectToAction("Error", "Home");
			}
		}
		public async Task<IActionResult> Logout()
		{
			try
			{
				RemoveSession();
				return Json(new Response
				{
					Result = ResultType.SUCCESS,
					Model = true
				});
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return Json(new Response
				{
					Result = ResultType.SYSTEM_ERROR,
					Error = e,
				});
			}
		}
		public async Task<IActionResult> Mypage()
		{
			try
			{
				if (IsLoggedIn)
				{
					if (IsAdministrator) return RedirectToAction("AccessDenied", "Home");
					else
					{
						using var db = new RomanceTourDbContext();
						var matched = await db.User.SingleOrDefaultAsync(x => x.Id == SessionId);
						ViewBag.Back = Back;
						return View(matched);
					}
				}
				else return RedirectToAction("LoginRequired", "Home");
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return RedirectToAction("Error", "Home");
			}
		}

		public async Task<IActionResult> CheckDuplication(User user)
		{
			try
			{
				if (XmlConfiguration.Administrator.UserName == user.UserName) return Json(new Response
				{
					Result = ResultType.SUCCESS,
					Model = false
				});
				else
				{
					using var db = new RomanceTourDbContext();
					var selected = await db.User.SingleOrDefaultAsync(x => x.UserName == user.UserName);
					if (selected != null) return Json(new Response
					{
						Result = ResultType.SUCCESS,
						Model = false
					});
					else return Json(new Response
					{
						Result = ResultType.SUCCESS,
						Model = true
					});
				}
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return Json(new Response
				{
					Result = ResultType.SYSTEM_ERROR,
					Error = e,
				});
			}
		}
		public async Task<IActionResult> CreateAccount(string token, User user)
		{
			try
			{
				if (XmlConfiguration.Administrator.UserName == user.UserName) return Json(new Response
				{
					Result = ResultType.SUCCESS,
					Model = false
				});
				else
				{
					using var db = new RomanceTourDbContext();
					var matched = await db.User.SingleOrDefaultAsync(x => x.UserName == user.UserName);
					if (matched == null)
					{
						string phone = PhoneVerifier.Retrieve(token);
						if (phone != null && phone == user.Phone)
						{
							user.HashSalt = KeyGenerator.GenerateString(32);
							user.Password = Password.Hash(user.Password, user.HashSalt);
							db.User.Add(user);
							await db.SaveChangesAsync();
							var registered = await db.User.SingleOrDefaultAsync(x => x.UserName == user.UserName);
							AddSession(registered.Id, registered.Name);
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
						Result = ResultType.SUCCESS,
						Model = false
					});
				}
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return Json(new Response
				{
					Result = ResultType.SYSTEM_ERROR,
					Error = e,
				});
			}
		}
		public async Task<IActionResult> CheckAccount(User user)
		{
			try
			{
				var admin = XmlConfiguration.Administrator;
				if (admin.UserName == user.UserName)
				{
					if (admin.Password == Password.Hash(user.Password, admin.HashSalt))
					{
						AddSession(admin.Id, admin.Name);
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
				else
				{
					using var db = new RomanceTourDbContext();
					var matched = await db.User.SingleOrDefaultAsync(x => x.UserName == user.UserName);
					if (matched != null && matched.Password == Password.Hash(user.Password, matched.HashSalt))
					{
						AddSession(matched.Id, matched.Name);
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
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return Json(new Response
				{
					Result = ResultType.SYSTEM_ERROR,
					Error = e,
				});
			}
		}

		public async Task<IActionResult> ListUser()
		{
			try
			{
				if (IsAdministrator)
				{
					using var db = new RomanceTourDbContext();
					var array = await db.User.Where(x => true).ToArrayAsync();
					return Json(new Response
					{
						Result = ResultType.SUCCESS,
						Model = array
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
		public async Task<IActionResult> SearchUser(int option, string keyword)
		{
			try
			{
				if (IsAdministrator)
				{
					using var db = new RomanceTourDbContext();
					switch (option)
					{
						case 0:
						{
							var matched = await db.User.Where(x => EF.Functions.Like(x.Name, $"%{keyword}%")).ToArrayAsync();
							return Json(new Response
							{
								Result = ResultType.SUCCESS,
								Model = matched
							});
						}
						case 1:
						{
							var matched = await db.User.Where(x => EF.Functions.Like(x.UserName, $"%{keyword}%")).ToArrayAsync();
							return Json(new Response
							{
								Result = ResultType.SUCCESS,
								Model = matched
							});
						}
						case 2:
						{
							var matched = await db.User.Where(x => EF.Functions.Like(x.Phone, $"%{keyword}%")).ToArrayAsync();
							return Json(new Response
							{
								Result = ResultType.SUCCESS,
								Model = matched
							});
						}
						default:
						{
							var matched = await db.User.Where(x => true).ToArrayAsync();
							return Json(new Response
							{
								Result = ResultType.SUCCESS,
								Model = matched
							});
						}
					}
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
		public async Task<IActionResult> RemoveUser(int id)
		{
			try
			{
				if (IsAdministrator)
				{
					using var db = new RomanceTourDbContext();
					var matched = await db.User.SingleOrDefaultAsync(x => x.Id == id);
					if (matched != null)
					{
						db.User.Remove(matched);
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

		public async Task<IActionResult> VerifyPhone(string phone)
		{
			try
			{
				using var db = new RomanceTourDbContext();
				int count = db.Verification.Count(x => x.TimeStamp.Date == DateTime.Now.Date && x.IpAddress == IPAddress);
				if (count <= XmlConfiguration.Verification.MaxRequest)
				{
					var result = await PhoneVerifier.CreateVerification(phone);
					if (result == VerificationResult.SUCCESS)
					{
						db.Verification.Add(new Models.Verification
						{
							IpAddress = IPAddress,
							TimeStamp = DateTime.Now
						});
						await db.SaveChangesAsync();
					}
					return Json(new Response
					{
						Result = ResultType.SUCCESS,
						Model = result
					});
				}
				else return Json(new Response
				{
					Result = ResultType.SUCCESS,
					Model = VerificationResult.MAX_REQUEST_REACHED
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
		public async Task<IActionResult> Challenge(string phone, string code)
		{
			try
			{
				(var Result, var Token) = PhoneVerifier.Challenge(phone, code);
				return Json(new Response
				{
					Result = ResultType.SUCCESS,
					Model = new
					{
						Result,
						Token
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

		public async Task<IActionResult> ChangePersonalInfo(User user)
		{
			try
			{
				using var db = new RomanceTourDbContext();
				var matched = await db.User.SingleOrDefaultAsync(x => x.Id == SessionId);
				if (matched != null)
				{
					matched.Name = user.Name;
					matched.Address = user.Address;
					matched.Birthday = user.Birthday;
					db.User.Update(matched);
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
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return Json(new Response
				{
					Result = ResultType.SYSTEM_ERROR,
					Error = e,
				});
			}
		}
		public async Task<IActionResult> ChangePhoneNumber(string token)
		{
			try
			{
				using var db = new RomanceTourDbContext();
				var matched = await db.User.SingleOrDefaultAsync(x => x.Id == SessionId);
				if (matched != null)
				{
					string phone = PhoneVerifier.Retrieve(token);
					var duplicated = await db.User.SingleOrDefaultAsync(x => x.Phone == phone);
					if (phone != null && duplicated == null)
					{
						matched.Phone = phone;
						db.User.Update(matched);
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
					Result = ResultType.SUCCESS,
					Model = false
				});
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return Json(new Response
				{
					Result = ResultType.SYSTEM_ERROR,
					Error = e,
				});
			}
		}
		public async Task<IActionResult> ChangePaymentInfo(User user)
		{
			try
			{
				using var db = new RomanceTourDbContext();
				var matched = await db.User.SingleOrDefaultAsync(x => x.Id == SessionId);
				if (matched != null)
				{
					matched.BillingName = user.BillingName;
					matched.BillingBank = user.BillingBank;
					matched.BillingNumber = user.BillingNumber;
					db.User.Update(matched);
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
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return Json(new Response
				{
					Result = ResultType.SYSTEM_ERROR,
					Error = e,
				});
			}
		}
		public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
		{
			try
			{
				using var db = new RomanceTourDbContext();
				var matched = await db.User.SingleOrDefaultAsync(x => x.Id == SessionId);
				if (matched != null)
				{
					if (matched.Password == Password.Hash(oldPassword, matched.HashSalt))
					{
						matched.HashSalt = KeyGenerator.GenerateString(32);
						matched.Password = Password.Hash(newPassword, matched.HashSalt);
						db.User.Update(matched);
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
					Result = ResultType.SUCCESS,
					Model = false
				});
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return Json(new Response
				{
					Result = ResultType.SYSTEM_ERROR,
					Error = e,
				});
			}
		}
		public async Task<IActionResult> Unregister(User user)
		{
			try
			{
				using var db = new RomanceTourDbContext();
				var matched = await db.User.SingleOrDefaultAsync(x => x.Id == SessionId);
				if (matched != null)
				{
					if (matched.Password == Password.Hash(user.Password, matched.HashSalt))
					{
						db.User.Remove(matched);
						await db.SaveChangesAsync();
						RemoveSession();
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
					Result = ResultType.SUCCESS,
					Model = false
				});
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
				return Json(new Response
				{
					Result = ResultType.SYSTEM_ERROR,
					Error = e,
				});
			}
		}
	}
}