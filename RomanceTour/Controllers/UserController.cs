using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Core.Security;
using RomanceTour.Models;
using RomanceTour.Middlewares;
using System.Linq;
using RomanceTour.ViewModels;

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

		public async Task<IActionResult> FindMyPassword()
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
							user.LastLogin = DateTime.Now;
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
							Model = new
							{
								Result = true,
								Message = "로그인이 완료되었습니다."
							}
						});
					}
					else return Json(new Response
					{
						Result = ResultType.SUCCESS,
						Model = new
						{
							Result = false,
							Message = "아이디 또는 비밀번호가 잘못되었습니다."
						}
					});
				}
				else
				{
					using var db = new RomanceTourDbContext();
					var matchedByUserName = await db.User.SingleOrDefaultAsync(x => x.UserName == user.UserName);
					var matchedByPhone = await db.User.SingleOrDefaultAsync(x => x.Phone == user.UserName);
					if (matchedByUserName != null)
					{
						if (matchedByUserName.Password == Password.Hash(user.Password, matchedByUserName.HashSalt))
						{
							switch (matchedByUserName.Status)
							{
								case UserStatus.GREEN:
								{
									AddSession(matchedByUserName.Id, matchedByUserName.Name);
									matchedByUserName.LastLogin = DateTime.Now;
									db.User.Update(matchedByUserName);
									await db.SaveChangesAsync();
									return Json(new Response
									{
										Result = ResultType.SUCCESS,
										Model = new
										{
											Result = true,
											Message = "로그인이 완료되었습니다."
										}
									});
								}
								case UserStatus.YELLOW:
								{
									AddSession(matchedByUserName.Id, matchedByUserName.Name);
									matchedByUserName.LastLogin = DateTime.Now;
									db.User.Update(matchedByUserName);
									await db.SaveChangesAsync();
									return Json(new Response
									{
										Result = ResultType.SUCCESS,
										Model = new
										{
											Result = true,
											Message = "의심스러운 사용자 활동으로 인해 1회 경고가 부가됩니다.\n향후 이러한 활동이 지속될 경우 계정이 정지될 수 있습니다.\n자세한 내용은 고객센터에 문의해주세요."
										}
									});
								}
								case UserStatus.RED:
								{
									return Json(new Response
									{
										Result = ResultType.SUCCESS,
										Model = new
										{
											Result = false,
											Message = "이용이 정지된 계정입니다.\n자세한 내용은 고객센터에 문의해주세요."
										}
									});
								}
								case UserStatus.GREY:
								{
									return Json(new Response
									{
										Result = ResultType.SUCCESS,
										Model = new
										{
											Result = false,
											Message = "현재 휴면상태인 계정입니다.\n계정 활성화를 원하시면 고객센터로 문의해주세요."
										}
									});
								}
								default:
								{
									return Json(new Response
									{
										Result = ResultType.SUCCESS,
										Model = new
										{
											Result = false
										}
									});
								}
							}
						}
						else return Json(new Response
						{
							Result = ResultType.SUCCESS,
							Model = new
							{
								Result = false,
								Message = "아이디 또는 비밀번호가 잘못되었습니다."
							}
						});
					}
					else if (matchedByPhone != null)
					{
						if (matchedByPhone.Password == Password.Hash(user.Password, matchedByPhone.HashSalt))
						{
							switch (matchedByPhone.Status)
							{
								case UserStatus.GREEN:
								{
									AddSession(matchedByPhone.Id, matchedByPhone.Name);
									matchedByPhone.LastLogin = DateTime.Now;
									db.User.Update(matchedByPhone);
									await db.SaveChangesAsync();
									return Json(new Response
									{
										Result = ResultType.SUCCESS,
										Model = new
										{
											Result = true,
											Message = "로그인이 완료되었습니다."
										}
									});
								}
								case UserStatus.YELLOW:
								{
									AddSession(matchedByPhone.Id, matchedByPhone.Name);
									matchedByPhone.LastLogin = DateTime.Now;
									db.User.Update(matchedByPhone);
									await db.SaveChangesAsync();
									return Json(new Response
									{
										Result = ResultType.SUCCESS,
										Model = new
										{
											Result = true,
											Message = "의심스러운 사용자 활동으로 인해 1회 경고가 부가됩니다.\n향후 이러한 활동이 지속될 경우 계정이 정지될 수 있습니다.\n자세한 내용은 고객센터에 문의해주세요."
										}
									});
								}
								case UserStatus.RED:
								{
									return Json(new Response
									{
										Result = ResultType.SUCCESS,
										Model = new
										{
											Result = false,
											Message = "이용이 정지된 계정입니다.\n자세한 내용은 고객센터에 문의해주세요."
										}
									});
								}
								case UserStatus.GREY:
								{
									return Json(new Response
									{
										Result = ResultType.SUCCESS,
										Model = new
										{
											Result = false,
											Message = "현재 휴면상태인 계정입니다.\n계정 활성화를 원하시면 고객센터로 문의해주세요."
										}
									});
								}
								default:
								{
									return Json(new Response
									{
										Result = ResultType.SUCCESS,
										Model = new
										{
											Result = false
										}
									});
								}
							}
						}
						else return Json(new Response
						{
							Result = ResultType.SUCCESS,
							Model = new
							{
								Result = false,
								Message = "아이디 또는 비밀번호가 잘못되었습니다."
							}
						});
					}
					else return Json(new Response
					{
						Result = ResultType.SUCCESS,
						Model = new
						{
							Result = false,
							Message = "아이디 또는 비밀번호가 잘못되었습니다."
						}
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
							var matched = await db.User.ToListAsync();
							matched = matched.Where(x => x.Name.Contains(keyword ?? string.Empty)).ToList();
							return Json(new Response
							{
								Result = ResultType.SUCCESS,
								Model = matched
							});
						}
						case 1:
						{
							var matched = await db.User.ToListAsync();
							matched = matched.Where(x => x.UserName.Contains(keyword ?? string.Empty)).ToList();
							return Json(new Response
							{
								Result = ResultType.SUCCESS,
								Model = matched
							});
						}
						case 2:
						{
							var matched = await db.User.ToListAsync();
							matched = matched.Where(x => x.Phone.Contains(keyword ?? string.Empty)).ToList();
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

		public async Task<IActionResult> AdminGetUser(int id)
		{
			try
			{
				if (IsAdministrator)
				{
					using var db = new RomanceTourDbContext();
					var matched = await db.User.SingleOrDefaultAsync(x => x.Id == id);
					if (matched != null) return Json(new Response
					{
						Result = ResultType.SUCCESS,
						Model = new
						{
							Result = true,
							Data = matched
						}
					});
					else return Json(new Response
					{
						Result = ResultType.SUCCESS,
						Model = new
						{
							Result = false,
							Message = "해당 사용자를 조회할 수 없습니다."
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
					Result = ResultType.SYSTEM_ERROR,
					Error = e
				});
			}
		}
		public async Task<IActionResult> AdminGetUserHistory(int id)
		{
			try
			{
				if (IsAdministrator)
				{
					using var db = new RomanceTourDbContext();
					var matched = await db.User.SingleOrDefaultAsync(x => x.Id == id);
					if (matched != null)
					{
						var logs = db.Log.Where(x => x.UserId == id).OrderByDescending(x => x.TimeStamp).Take(500);
						return Json(new Response
						{
							Result = ResultType.SUCCESS,
							Model = new
							{
								Result = true,
								Data = logs.Select(x => new LogVM
								{
									TimeStamp = x.TimeStamp,
									IpAddress = x.IpAddress,
									Controller = ResourceMapper.GetControllerResource(x.Controller),
									Action = ResourceMapper.GetActionResource(x.Action)
								}).ToArray()
							}
						});
					}
					else return Json(new Response
					{
						Result = ResultType.SUCCESS,
						Model = new
						{
							Result = false,
							Message = "해당 사용자를 조회할 수 없습니다."
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
					Result = ResultType.SYSTEM_ERROR,
					Error = e
				});
			}
		}
		public async Task<IActionResult> AdminUpdateUser(int id, User user)
		{
			try
			{
				if (IsAdministrator)
				{
					using var db = new RomanceTourDbContext();
					var matched = await db.User.SingleOrDefaultAsync(x => x.Id == id);
					if (matched != null)
					{
						matched.Name = user.Name;
						matched.Phone = user.Phone;
						matched.Address = user.Address;
						matched.Birthday = user.Birthday;
						matched.BillingName = user.BillingName;
						matched.BillingBank = user.BillingBank;
						matched.BillingNumber = user.BillingNumber;
						db.User.Update(matched);
						await db.SaveChangesAsync();
						return Json(new Response
						{
							Result = ResultType.SUCCESS,
							Model = new
							{
								Result = true,
								Message = "사용자 정보를 성공적으로 변경했습니다."
							}
						});
					}
					else return Json(new Response
					{
						Result = ResultType.SUCCESS,
						Model = new
						{
							Result = false,
							Message = "해당 사용자를 조회할 수 없습니다."
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
					Result = ResultType.SYSTEM_ERROR,
					Error = e
				});
			}
		}
		public async Task<IActionResult> AdminUpdateUserStatus(int id, string status)
		{
			try
			{
				if (IsAdministrator)
				{
					using var db = new RomanceTourDbContext();
					var matched = await db.User.SingleOrDefaultAsync(x => x.Id == id);
					if (matched != null)
					{
						matched.Status = Enum.Parse<UserStatus>(status);
						db.User.Update(matched);
						await db.SaveChangesAsync();
						return Json(new Response
						{
							Result = ResultType.SUCCESS,
							Model = new
							{
								Result = true,
								Message = "사용자 상태를 성공적으로 변경했습니다."
							}
						});
					}
					else return Json(new Response
					{
						Result = ResultType.SUCCESS,
						Model = new
						{
							Result = false,
							Message = "해당 사용자를 조회할 수 없습니다."
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
		public async Task<IActionResult> VerifyPhoneForUser(string phone)
		{
			try
			{
				using var db = new RomanceTourDbContext();
				int count = db.Verification.Count(x => x.TimeStamp.Date == DateTime.Now.Date && x.IpAddress == IPAddress);
				if (count <= XmlConfiguration.Verification.MaxRequest)
				{
					var matched = await db.User.SingleOrDefaultAsync(x => x.Phone == phone);
					if (matched != null)
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
						Model = VerificationResult.USER_NOT_FOUND
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

		public async Task<IActionResult> ResetPassword(string token, string newPassword)
		{
			string phone = PhoneVerifier.Retrieve(token);
			if (phone != null)
			{
				var db = new RomanceTourDbContext();
				var matched = await db.User.SingleOrDefaultAsync(x => x.Phone == phone);
				if (matched != null)
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
		public async Task<IActionResult> ChangeTermsAgreement(bool agreement)
		{
			try
			{
				using var db = new RomanceTourDbContext();
				var matched = await db.User.SingleOrDefaultAsync(x => x.Id == SessionId);
				if (matched != null)
				{
					matched.AllowMarketingPromotions = agreement;
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
				var matched = await db.User
					.Include(x => x.Appointment)
						.ThenInclude(x => x.DateSession)
					.SingleOrDefaultAsync(x => x.Id == SessionId && x.UserName == user.UserName);
				if (matched != null)
				{
					if (matched.Password == Password.Hash(user.Password, matched.HashSalt))
					{
						var appointments = matched.Appointment.Where(x => x.DateSession.Date > DateTime.Now);
						if (appointments.Count() > 0) return Json(new Response
						{
							Result = ResultType.SUCCESS,
							Model = new
							{
								Result = false,
								Message = "아직 진행되지 않은 예약이 남아있어\n회원 탈퇴를 진행할 수 없습니다.\n자세한 내용은 고객센터에 문의해주세요."
							}
						});
						else
						{
							db.User.Remove(matched);
							await db.SaveChangesAsync();
							RemoveSession();
							return Json(new Response
							{
								Result = ResultType.SUCCESS,
								Model = new
								{
									Result = true,
									Message = "정상적으로 탈퇴 처리되었습니다.\n다시 찾아주시길 기다리겠습니다."
								}
							});
						}
					}
					else return Json(new Response
					{
						Result = ResultType.SUCCESS,
						Model = new
						{
							Result = false,
							Message = "아이디 또는 비밀번호가 잘못되었습니다."
						}
					});
				}
				else return Json(new Response
				{
					Result = ResultType.SUCCESS,
					Model = new
					{
						Result = false,
						Message = "아이디 또는 비밀번호가 잘못되었습니다."
					}
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