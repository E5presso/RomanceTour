using System;
using System.Linq;
using System.Threading.Tasks;

using Core.Security;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using RomanceTour.Middlewares;
using RomanceTour.Models;
using RomanceTour.ViewModels;

namespace RomanceTour.Controllers
{
    public class AppointmentController : BaseController
    {
        public async Task<IActionResult> CheckAppointment()
        {
            try
            {
                if (!IsLoggedIn)
                {
                    ViewBag.Back = Back;
                    return View();
                }
                else return RedirectToAction("ListAppointment", "Appointment");
            }
            catch (Exception e)
            {
                await LogManager.ErrorAsync(e);
                return RedirectToAction("Error", "Home");
            }
        }
        public async Task<IActionResult> LookupAppointment(string name, string phone)
        {
            try
            {
                if (!IsLoggedIn)
                {
                    using var db = new RomanceTourDbContext();
                    var matched = await db.Appointment
                        .Include(x => x.DateSession)
                            .ThenInclude(x => x.Product)
                        .Where(x => x.Name == name && x.Phone == phone)
                        .ToListAsync();

                    ViewBag.Back = Back;
                    ViewBag.Appointments = matched;
                    return View("ListAppointment");
                }
                else return RedirectToAction("ListAppointment", "Appointment");
            }
            catch (Exception e)
            {
                await LogManager.ErrorAsync(e);
                return RedirectToAction("Error", "Home");
            }
        }
        public async Task<IActionResult> ListAppointment()
        {
            try
            {
                if (IsLoggedIn)
                {
                    if (IsAdministrator) return RedirectToAction("AccessDenied", "Home");
                    else
                    {
                        using var db = new RomanceTourDbContext();
                        var matched = await db.Appointment
                            .Include(x => x.User)
                            .Include(x => x.DateSession)
                                .ThenInclude(x => x.Product)
                            .Where(x => x.UserId == SessionId)
                            .ToListAsync();

                        ViewBag.Back = Back;
                        ViewBag.Appointments = matched;
                        return View();
                    }
                }
                else return RedirectToAction("CheckAppointment", "Appointment");
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
                if (IsLoggedIn)
                {
                    if (IsAdministrator) return RedirectToAction("AccessDenied", "Home");
                    else
                    {
                        using var db = new RomanceTourDbContext();
                        var matched = await db.Appointment
                            .Include(x => x.User)
                            .Include(x => x.Person)
                                .ThenInclude(x => x.Departure)
                            .Include(x => x.Person)
                                .ThenInclude(x => x.Option)
                                    .ThenInclude(x => x.PriceRule)
                            .Include(x => x.DateSession)
                                .ThenInclude(x => x.Product)
                                    .ThenInclude(x => x.Category)
                            .Include(x => x.DateSession)
                                .ThenInclude(x => x.Product)
                                    .ThenInclude(x => x.ProductBilling)
                                        .ThenInclude(x => x.Billing)
                            .SingleOrDefaultAsync(x => x.UserId == SessionId && x.Id == id);

                        if (matched != null)
                        {
                            ViewBag.Back = Back;
                            ViewBag.Appointment = matched;
                            return View();
                        }
                        else return RedirectToAction("PageNotFound", "Home");
                    }
                }
                else return RedirectToAction("TryGetAppointment", "Appointment", new { id });
            }
            catch (Exception e)
            {
                await LogManager.ErrorAsync(e);
                return RedirectToAction("Error", "Home");
            }
        }
        public async Task<IActionResult> TryGetAppointment(int id, string password)
        {
            try
            {
                if (!IsLoggedIn)
                {
                    using var db = new RomanceTourDbContext();
                    var matched = await db.Appointment
                        .SingleOrDefaultAsync(x => !x.IsUserAppointment && x.Id == id);

                    if (matched != null)
                    {
                        if (password != null)
                        {
                            if (matched.Password == Hash.SHA256(password, matched.HashSalt))
                            {
                                matched = await db.Appointment
                                    .Include(x => x.Person)
                                        .ThenInclude(x => x.Departure)
                                    .Include(x => x.Person)
                                        .ThenInclude(x => x.Option)
                                            .ThenInclude(x => x.PriceRule)
                                    .Include(x => x.DateSession)
                                        .ThenInclude(x => x.Product)
                                            .ThenInclude(x => x.Category)
                                    .Include(x => x.DateSession)
                                        .ThenInclude(x => x.Product)
                                            .ThenInclude(x => x.ProductBilling)
                                                .ThenInclude(x => x.Billing)
                                    .SingleOrDefaultAsync(x => x.UserId == SessionId && x.Id == id);

                                ViewBag.Back = Back;
                                ViewBag.Appointment = matched;
                                return View("GetAppointment");
                            }
                            else return RedirectToAction("AccessDenied", "Home");
                        }
                        else
                        {
                            ViewBag.Id = id;
                            return View();
                        }
                    }
                    else return RedirectToAction("PageNotFound", "Home");
                }
                else return RedirectToAction("GetAppointment", "Appointment", new { id });
            }
            catch (Exception e)
            {
                await LogManager.ErrorAsync(e);
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> AddUserAppointment(int id)
        {
            try
            {
                if (IsLoggedIn)
                {
                    if (!IsAdministrator)
                    {
                        using var db = new RomanceTourDbContext();
                        var product = await db.Product
                            .Include(x => x.ProductPriceRule)
                                .ThenInclude(x => x.PriceRule)
                            .Include(x => x.ProductDeparture)
                                .ThenInclude(x => x.Departure)
                            .Include(x => x.ProductBilling)
                                .ThenInclude(x => x.Billing)
                            .Include(x => x.DateSession)
                            .SingleOrDefaultAsync(x => x.Id == id);
                        var user = await db.User.SingleOrDefaultAsync(x => x.Id == SessionId);
                        ViewBag.Id = id;
                        ViewBag.User = user;
                        ViewBag.Product = product;
                        ViewBag.Back = Back;
                        return View();
                    }
                    else return RedirectToAction("AccessDenied", "Home");
                }
                else return Redirect($"/Appointment/AddNormalAppointment?id={id}");
            }
            catch (Exception e)
            {
                await LogManager.ErrorAsync(e);
                return RedirectToAction("Error", "Home");
            }
        }
        public async Task<IActionResult> AddNormalAppointment(int id)
        {
            try
            {
                if (!IsLoggedIn)
                {
                    using var db = new RomanceTourDbContext();
                    var product = await db.Product
                        .Include(x => x.ProductPriceRule)
                            .ThenInclude(x => x.PriceRule)
                        .Include(x => x.ProductDeparture)
                            .ThenInclude(x => x.Departure)
                        .Include(x => x.ProductBilling)
                            .ThenInclude(x => x.Billing)
                        .Include(x => x.DateSession)
                        .SingleOrDefaultAsync(x => x.Id == id);
                    var user = await db.User.SingleOrDefaultAsync(x => x.Id == SessionId);
                    ViewBag.Id = id;
                    ViewBag.User = user;
                    ViewBag.Product = product;
                    ViewBag.Back = Back;
                    return View();
                }
                else return Redirect($"/Appointment/AddUserAppointment?id={id}");
            }
            catch (Exception e)
            {
                await LogManager.ErrorAsync(e);
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> SearchAppointment(int id)
        {
            try
            {
                using var db = new RomanceTourDbContext();
                var matched = await db.Appointment
                    .Include(x => x.DateSession)
                        .ThenInclude(x => x.Product)
                    .Include(x => x.Person)
                        .ThenInclude(x => x.Option)
                            .ThenInclude(x => x.PriceRule)
                    .Include(x => x.Person)
                        .ThenInclude(x => x.Departure)
                    .SingleOrDefaultAsync(x => x.Id == id);
                if (matched != null)
                {
                    return Json(new Response
                    {
                        Result = ResultType.SUCCESS,
                        Model = new
                        {
                            Found = true,
                            Result = matched
                        }
                    });
                }
                else return Json(new Response
                {
                    Result = ResultType.SUCCESS,
                    Model = new
                    {
                        Found = false
                    }
                });
            }
            catch (Exception e)
            {
                await LogManager.ErrorAsync(e);
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> GetAvailable(int id)
        {
            try
            {
                using var db = new RomanceTourDbContext();
                var matched = await db.DateSession.Where(x => x.ProductId == id).ToArrayAsync();
                return Json(new Response
                {
                    Result = ResultType.SUCCESS,
                    Model = matched
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
        public async Task<IActionResult> AddAppointment(AppointmentVM appointment)
        {
            try
            {
                if (appointment.IsUserAppointment)
                {
                    if (IsLoggedIn)
                    {
                        using var db = new RomanceTourDbContext();
                        var user = await db.User.SingleOrDefaultAsync(x => x.Id == SessionId);
                        var product = await db.Product.Include(x => x.DateSession).SingleOrDefaultAsync(x => x.Id == appointment.Id);
                        if (product != null)
                        {
                            var session = product.DateSession.SingleOrDefault(x => x.Date == appointment.Date);
                            if (session != null)
                            {
                                var item = new Appointment
                                {
                                    DateSessionId = session.Id,
                                    IsUserAppointment = true,
                                    TimeStamp = DateTime.Now,
                                    Status = AppointmentStatus.READY_TO_PAY,
                                    User = user,
                                    BillingName = appointment.BillingName,
                                    BillingBank = appointment.BillingBank,
                                    BillingNumber = appointment.BillingNumber,
                                    Ammount = appointment.People.Sum(x => x.Ammount)
                                };
                                foreach (var x in appointment.People)
                                {
                                    var person = new Person
                                    {
                                        DepartureId = x.Departure,
                                        Ammount = x.Ammount
                                    };
                                    person.Option.Add(new Option
                                    {
                                        PriceRuleId = x.Price
                                    });
                                    if (x.Options != null)
                                    {
                                        foreach (var y in x.Options)
                                        {
                                            person.Option.Add(new Option
                                            {
                                                PriceRuleId = y
                                            });
                                        }
                                    }
                                    item.Person.Add(person);
                                }
                                db.Appointment.Add(item);
                                await db.SaveChangesAsync();
                                item = await db.Appointment
                                    .Include(x => x.Person)
                                        .ThenInclude(x => x.Option)
                                            .ThenInclude(x => x.PriceRule)
                                    .SingleOrDefaultAsync(x => x.Id == item.Id);
                                item.Price = item.Person.Sum
                                (
                                    x => x.Option.Sum
                                    (
                                        x => x.PriceRule.RuleType switch
                                        {
                                            PriceRuleType.PERCENT_AS => product.Price / 100 * x.PriceRule.Price,
                                            PriceRuleType.PERCENT_PLUS => product.Price / 100 * x.PriceRule.Price,
                                            PriceRuleType.PERCENT_MINUS => -1 * product.Price / 100 * x.PriceRule.Price,
                                            PriceRuleType.STATIC_PLUS => x.PriceRule.Price,
                                            PriceRuleType.STATIC_MINUS => -1 * x.PriceRule.Price,
                                            _ => 0
                                        }
                                    ) * x.Ammount
                                );
                                db.Appointment.Update(item);

                                user.BillingName = appointment.BillingName;
                                user.BillingBank = appointment.BillingBank;
                                user.BillingNumber = appointment.BillingNumber;
                                db.User.Update(user);

                                session.Reserved += item.Ammount;
                                db.DateSession.Update(session);

                                await db.SaveChangesAsync();
                                return Json(new Response
                                {
                                    Result = ResultType.SUCCESS,
                                    Model = new
                                    {
                                        Result = true,
                                        Message = "예약이 완료되었습니다."
                                    }
                                });
                            }
                            else return Json(new Response
                            {
                                Result = ResultType.SUCCESS,
                                Model = new
                                {
                                    Result = false,
                                    Message = "예약할 수 없는 날짜입니다."
                                }
                            });
                        }
                        else return Json(new Response
                        {
                            Result = ResultType.SUCCESS,
                            Model = new
                            {
                                Result = false,
                                Message = "존재하지 않는 상품입니다."
                            }
                        });
                    }
                    else return Json(new Response
                    {
                        Result = ResultType.LOGIN_REQUIRED
                    });
                }
                else
                {
                    using var db = new RomanceTourDbContext();
                    var product = await db.Product.Include(x => x.DateSession).SingleOrDefaultAsync(x => x.Id == appointment.Id);
                    if (product != null)
                    {
                        var session = product.DateSession.SingleOrDefault(x => x.Date == appointment.Date);
                        if (session != null)
                        {
                            var salt = Key.GenerateString(32);
                            var item = new Appointment
                            {
                                DateSessionId = session.Id,
                                IsUserAppointment = false,
                                TimeStamp = DateTime.Now,
                                Status = AppointmentStatus.READY_TO_PAY,
                                Password = Hash.SHA256(appointment.Password, salt),
                                HashSalt = salt,
                                Name = appointment.Name,
                                Phone = appointment.Phone,
                                Address = appointment.Address,
                                BillingName = appointment.BillingName,
                                BillingBank = appointment.BillingBank,
                                BillingNumber = appointment.BillingNumber,
                                Ammount = appointment.People.Sum(x => x.Ammount)
                            };
                            foreach (var x in appointment.People)
                            {
                                var person = new Person
                                {
                                    DepartureId = x.Departure,
                                    Ammount = x.Ammount
                                };
                                person.Option.Add(new Option
                                {
                                    PriceRuleId = x.Price
                                });
                                if (x.Options != null)
                                {
                                    foreach (var y in x.Options)
                                    {
                                        person.Option.Add(new Option
                                        {
                                            PriceRuleId = y
                                        });
                                    }
                                }
                                item.Person.Add(person);
                            }
                            db.Appointment.Add(item);
                            await db.SaveChangesAsync();
                            item = await db.Appointment
                                .Include(x => x.Person)
                                    .ThenInclude(x => x.Option)
                                        .ThenInclude(x => x.PriceRule)
                                .SingleOrDefaultAsync(x => x.Id == item.Id);
                            item.Price = item.Person.Sum
                            (
                                x => x.Option.Sum
                                (
                                    x => x.PriceRule.RuleType switch
                                    {
                                        PriceRuleType.PERCENT_AS => product.Price / 100 * x.PriceRule.Price,
                                        PriceRuleType.PERCENT_PLUS => product.Price / 100 * x.PriceRule.Price,
                                        PriceRuleType.PERCENT_MINUS => -1 * product.Price / 100 * x.PriceRule.Price,
                                        PriceRuleType.STATIC_PLUS => x.PriceRule.Price,
                                        PriceRuleType.STATIC_MINUS => -1 * x.PriceRule.Price,
                                        _ => 0
                                    }
                                ) * x.Ammount
                            );
                            db.Appointment.Update(item);

                            session.Reserved += item.Ammount;
                            db.DateSession.Update(session);

                            await db.SaveChangesAsync();
                            return Json(new Response
                            {
                                Result = ResultType.SUCCESS,
                                Model = new
                                {
                                    Result = true,
                                    Message = "예약이 완료되었습니다."
                                }
                            });
                        }
                        else return Json(new Response
                        {
                            Result = ResultType.SUCCESS,
                            Model = new
                            {
                                Result = false,
                                Message = "예약할 수 없는 날짜입니다."
                            }
                        });
                    }
                    else return Json(new Response
                    {
                        Result = ResultType.SUCCESS,
                        Model = new
                        {
                            Result = false,
                            Message = "존재하지 않는 상품입니다."
                        }
                    });
                }
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

        public async Task<IActionResult> FilterSession(int id, int status, SessionFilterVM filter)
        {
            try
            {
                if (IsAdministrator)
                {
                    using var db = new RomanceTourDbContext();
                    var matched = db.DateSession.Where(x => x.ProductId == id);
                    if (status != -1) matched = matched.Where(x => x.Status == (DateSessionStatus)status);
                    if (filter.FromAppointment != null && filter.ToAppointment != null) matched = matched.Where(x => x.Reserved >= filter.FromAppointment && x.Reserved <= filter.ToAppointment);
                    if (filter.FromPaid != null && filter.ToPaid != null) matched = matched.Where(x => x.Paid >= filter.FromPaid && x.Paid <= filter.ToPaid);
                    if (filter.FromDate != null && filter.ToDate != null) matched = matched.Where(x => x.Date >= filter.FromDate && x.Date <= filter.ToDate);
                    return Json(new Response
                    {
                        Result = ResultType.SUCCESS,
                        Model = await matched.ToArrayAsync()
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
        public async Task<IActionResult> GetSession(int id)
        {
            try
            {
                if (IsAdministrator)
                {
                    using var db = new RomanceTourDbContext();
                    var matched = await db.DateSession
                        .Include(x => x.Product)
                        .Include(x => x.Appointment)
                            .ThenInclude(x => x.Person)
                                .ThenInclude(x => x.Option)
                                    .ThenInclude(x => x.PriceRule)
                        .SingleOrDefaultAsync(x => x.Id == id);
                    var model = new DateSessionVM
                    {
                        Id = matched.Id,
                        Date = matched.Date,
                        Paid = matched.Paid,
                        Reserved = matched.Reserved,
                        Status = matched.Status,
                        Sales = matched.Sales
                    };
                    return Json(new Response
                    {
                        Result = ResultType.SUCCESS,
                        Model = model
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
        public async Task<IActionResult> UpdateSession(int id, int status)
        {
            try
            {
                if (IsAdministrator)
                {
                    using var db = new RomanceTourDbContext();
                    var session = await db.DateSession.SingleOrDefaultAsync(x => x.Id == id);
                    session.Status = (DateSessionStatus)status;
                    db.DateSession.Update(session);
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
        public async Task<IActionResult> CountDeparture(int id, int departureId)
        {
            try
            {
                if (IsAdministrator)
                {
                    using var db = new RomanceTourDbContext();
                    var appointment = db.Appointment.Include(x => x.Person).Where(x => x.DateSessionId == id);
                    var count = await appointment.SumAsync(x => x.Person.Where(x => x.DepartureId == departureId).Sum(x => x.Ammount));
                    return Json(new Response
                    {
                        Result = ResultType.SUCCESS,
                        Model = count
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
        public async Task<IActionResult> CountPriceRule(int id, int PriceRuleId)
        {
            try
            {
                if (IsAdministrator)
                {
                    using var db = new RomanceTourDbContext();
                    var appointment = db.Appointment
                        .Include(x => x.Person)
                            .ThenInclude(x => x.Option)
                        .Where(x => x.DateSessionId == id);
                    var count = await appointment.SumAsync(x => x.Person.Where(x => x.Option.SingleOrDefault(x => x.PriceRuleId == PriceRuleId) != null).Sum(x => x.Ammount));
                    return Json(new Response
                    {
                        Result = ResultType.SUCCESS,
                        Model = count
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

        public async Task<IActionResult> FilterAppointment(int id, int status, string keyword)
        {
            try
            {
                if (IsAdministrator)
                {
                    using var db = new RomanceTourDbContext();
                    var matched = db.Appointment
                        .Include(x => x.DateSession)
                            .ThenInclude(x => x.Product)
                        .Include(x => x.Person)
                            .ThenInclude(x => x.Option)
                                .ThenInclude(x => x.PriceRule)
                        .Include(x => x.User)
                        .Where(x => x.DateSessionId == id)
                        .Select(x => new ManageAppointmentVM
                        {
                            Id = x.Id,
                            Name = x.IsUserAppointment ? x.User.Name : x.Name,
                            Ammount = x.Ammount,
                            IsUser = x.IsUserAppointment,
                            Status = x.Status,
                            Phone = x.IsUserAppointment ? x.User.Phone : x.Phone,
                            Address = x.IsUserAppointment ? x.User.Address : x.Address,
                            BillingName = x.IsUserAppointment ? x.User.BillingName : x.BillingName,
                            BillingBank = x.IsUserAppointment ? x.User.BillingBank : x.BillingBank,
                            BillingNumber = x.IsUserAppointment ? x.User.BillingNumber : x.BillingNumber,
                            Price = x.Price
                        });
                    if (status != -1) matched = matched.Where(x => x.Status == (AppointmentStatus)status);
                    matched = matched.Where(x =>
                        EF.Functions.Like(x.Name, $"%{keyword}%") ||
                        EF.Functions.Like(x.Phone, $"%{keyword}%") ||
                        EF.Functions.Like(x.Address, $"%{keyword}%") ||
                        EF.Functions.Like(x.BillingName, $"%{keyword}%") ||
                        EF.Functions.Like(x.BillingBank, $"%{keyword}%") ||
                        EF.Functions.Like(x.BillingNumber, $"%{keyword}%")
                    );
                    return Json(new Response
                    {
                        Result = ResultType.SUCCESS,
                        Model = await matched.ToArrayAsync()
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
        public async Task<IActionResult> GetAppointmentStatus(int id)
        {
            try
            {
                if (IsAdministrator)
                {
                    using var db = new RomanceTourDbContext();
                    var matched = await db.Appointment.SingleOrDefaultAsync(x => x.Id == id);
                    return Json(new Response
                    {
                        Result = ResultType.SUCCESS,
                        Model = (int)matched.Status
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
        public async Task<IActionResult> UpdateAppointmentStatus(int id, int status)
        {
            try
            {
                if (IsAdministrator)
                {
                    using var db = new RomanceTourDbContext();
                    var matched = await db.Appointment
                        .Include(x => x.DateSession)
                            .ThenInclude(x => x.Product)
                        .Include(x => x.Person)
                        .SingleOrDefaultAsync(x => x.Id == id);

                    if (matched.Status == AppointmentStatus.CONFIRMED && (AppointmentStatus)status != AppointmentStatus.CONFIRMED)
                    {
                        matched.DateSession.Paid -= matched.Person.Sum(x => x.Ammount);
                        matched.DateSession.Sales -= matched.Price;
                    }
                    else if (matched.Status != AppointmentStatus.CONFIRMED && (AppointmentStatus)status == AppointmentStatus.CONFIRMED)
                    {
                        matched.DateSession.Paid += matched.Person.Sum(x => x.Ammount);
                        matched.DateSession.Sales += matched.Price;
                    }
                    else matched.DateSession.Paid = matched.DateSession.Paid;
                    matched.Status = (AppointmentStatus)status;

                    db.Appointment.Update(matched);
                    db.DateSession.Update(matched.DateSession);

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
    }
}