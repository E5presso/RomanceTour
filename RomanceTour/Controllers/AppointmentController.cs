using System;
using System.Linq;
using System.Threading.Tasks;

using Core.Security;
using Core.Utility;
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
                    matched.ForEach(x => x.Status = x.DateSession.Status == DateSessionStatus.CANCELED ? AppointmentStatus.CANCELED : x.Status);
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
                        matched.ForEach(x => x.Status = x.DateSession.Status == DateSessionStatus.CANCELED ? AppointmentStatus.CANCELED : x.Status);
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
                        matched.Status = matched.DateSession.Status == DateSessionStatus.CANCELED ? AppointmentStatus.CANCELED : matched.Status;

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
                            if (matched.Password == Password.Hash(password, matched.HashSalt))
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
                                matched.Status = matched.DateSession.Status == DateSessionStatus.CANCELED ? AppointmentStatus.CANCELED : matched.Status;

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
        public async Task<IActionResult> ViewAppointment(string link)
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
                .SingleOrDefaultAsync(x => x.Link == link);

            if (matched != null)
            {
                matched.Status = matched.DateSession.Status == DateSessionStatus.CANCELED ? AppointmentStatus.CANCELED : matched.Status;

                ViewBag.Back = Back;
                ViewBag.Appointment = matched;
                ViewBag.Type = "View";
                return View("GetAppointment");
            }
            else return RedirectToAction("PageNotFound", "Home");
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
                                string link = Converter.ToHexCode(Guid.NewGuid().ToByteArray());
                                var check = await db.Appointment.SingleOrDefaultAsync(x => x.Link == link);
                                while (check != null)
                                {
                                    link = Converter.ToHexCode(Guid.NewGuid().ToByteArray());
                                    check = await db.Appointment.SingleOrDefaultAsync(x => x.Link == link);
                                }
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
                                    Ammount = appointment.People.Sum(x => x.Ammount),
                                    Link = link
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
                                await MessageSender.SendAppointmentMessage(user.Phone, user.Name, product.Title, session.Date, $@"https://romancetour.ml/Appointment/ViewAppointment?link={link}");
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
                            string link = Converter.ToHexCode(Guid.NewGuid().ToByteArray());
                            var check = await db.Appointment.SingleOrDefaultAsync(x => x.Link == link);
                            while (check != null)
                            {
                                link = Converter.ToHexCode(Guid.NewGuid().ToByteArray());
                                check = await db.Appointment.SingleOrDefaultAsync(x => x.Link == link);
                            }
                            var salt = KeyGenerator.GenerateString(32);
                            var item = new Appointment
                            {
                                DateSessionId = session.Id,
                                IsUserAppointment = false,
                                TimeStamp = DateTime.Now,
                                Status = AppointmentStatus.READY_TO_PAY,
                                Password = Password.Hash(appointment.Password, salt),
                                HashSalt = salt,
                                Name = appointment.Name,
                                Phone = appointment.Phone,
                                Address = appointment.Address,
                                BillingName = appointment.BillingName,
                                BillingBank = appointment.BillingBank,
                                BillingNumber = appointment.BillingNumber,
                                Ammount = appointment.People.Sum(x => x.Ammount),
                                Link = link
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
                            await MessageSender.SendAppointmentMessage(appointment.Phone, appointment.Name, product.Title, session.Date, $@"https://romancetour.ml/Appointment/ViewAppointment?link={link}");
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
                        Model = await matched.OrderBy(x => x.Date).ToArrayAsync()
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
                    var session = await db.DateSession
                        .Include(x => x.Appointment)
                        .SingleOrDefaultAsync(x => x.Id == id);
                    if (session.Status == DateSessionStatus.CANCELED && (DateSessionStatus)status != DateSessionStatus.CANCELED)
					{
                        session.Reserved = session.Appointment.Sum(x => x.Ammount);
                        session.Paid = session.Appointment.Where(x => x.Status == AppointmentStatus.CONFIRMED).Sum(x => x.Ammount);
                        session.Sales = session.Appointment.Where(x => x.Status == AppointmentStatus.CONFIRMED).Sum(x => x.Price);
                    }
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
                    var count = await appointment.Select(x => x.Person.Where(x => x.DepartureId == departureId).Sum(x => x.Ammount)).ToArrayAsync();
                    return Json(new Response
                    {
                        Result = ResultType.SUCCESS,
                        Model = count.Sum()
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
                    var count = await appointment.Select(x => x.Person.Where(x => x.Option.SingleOrDefault(x => x.PriceRuleId == PriceRuleId) != null).Sum(x => x.Ammount)).ToArrayAsync();
                    return Json(new Response
                    {
                        Result = ResultType.SUCCESS,
                        Model = count.Sum()
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
                            Status = x.DateSession.Status == DateSessionStatus.CANCELED ? AppointmentStatus.CANCELED : x.Status,
                            Phone = x.IsUserAppointment ? x.User.Phone : x.Phone,
                            Address = x.IsUserAppointment ? x.User.Address : x.Address,
                            BillingName = x.BillingName,
                            BillingBank = x.BillingBank,
                            BillingNumber = x.BillingNumber,
                            Price = x.Price
                        });
                    if (status != -1) matched = matched.Where(x => x.Status == (AppointmentStatus)status);
                    var converted = await matched.ToArrayAsync();
                    converted = converted.Where(x =>
                        x.Name.Contains($"{keyword}") ||
                        x.Phone.Contains($"{keyword}") ||
                        x.Address.Contains($"{keyword}") ||
                        x.BillingName.Contains($"{keyword}") ||
                        x.BillingBank.Contains($"{keyword}") ||
                        x.BillingNumber.Contains($"{keyword}")
                    ).ToArray();
                    return Json(new Response
                    {
                        Result = ResultType.SUCCESS,
                        Model = converted
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
                    var matched = await db.Appointment
                        .Include(x => x.DateSession)
                        .SingleOrDefaultAsync(x => x.Id == id);
                    if (matched != null)
                    {
                        return Json(new Response
                        {
                            Result = ResultType.SUCCESS,
                            Model = matched.DateSession.Status == DateSessionStatus.CANCELED ? (int)AppointmentStatus.CANCELED : (int)matched.Status
                        });
                    }
                    else
                    {
                        return Json(new Response
                        {
                            Result = ResultType.SUCCESS,
                            Model = false
                        });
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
                    if ((AppointmentStatus)status == AppointmentStatus.REFUNDED)
                    {
                        matched.DateSession.Reserved -= matched.Person.Sum(x => x.Ammount);
                        db.DateSession.Update(matched.DateSession);
                        db.Appointment.Remove(matched);
                        await db.SaveChangesAsync();
                        return Json(new Response
                        {
                            Result = ResultType.SUCCESS,
                            Model = true
                        });
                    }
                    else
                    {
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

        public async Task<IActionResult> EditAppointment(int id)
        {
            try
            {
                if (IsLoggedIn)
                {
                    if (IsAdministrator)
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
                                    .ThenInclude(x => x.ProductPriceRule)
                                        .ThenInclude(x => x.PriceRule)
                            .Include(x => x.DateSession)
                                .ThenInclude(x => x.Product)
                                    .ThenInclude(x => x.ProductDeparture)
                                        .ThenInclude(x => x.Departure)
                            .SingleOrDefaultAsync(x => x.Id == id);
                        matched.Status = matched.DateSession.Status == DateSessionStatus.CANCELED ? AppointmentStatus.CANCELED : matched.Status;

                        if (matched != null)
                        {
                            ViewBag.Appointment = matched;
                            if (matched.IsUserAppointment) return View("EditUserAppointment");
                            else return View("EditNormalAppointment");
                        }
                        else return RedirectToAction("PageNotFound", "Home");
                    }
                    else return RedirectToAction("AccessDenied", "Home");
                }
                else return RedirectToAction("TryGetAppointment", "Appointment", new { id });
            }
            catch (Exception e)
            {
                await LogManager.ErrorAsync(e);
                return RedirectToAction("Error", "Home");
            }
        }
        public async Task<IActionResult> UpdateAppointment(AppointmentVM appointment)
        {
            try
            {
                if (IsLoggedIn)
				{
                    if (IsAdministrator)
                    {
                        using var db = new RomanceTourDbContext();
                        var matched = await db.Appointment
                            .Include(x => x.User)
                            .Include(x => x.DateSession)
                                .ThenInclude(x => x.Product)
                            .Include(x => x.Person)
                                .ThenInclude(x => x.Option)
                                    .ThenInclude(x => x.PriceRule)
                            .SingleOrDefaultAsync(x => x.Id == appointment.Id);
                        if (matched != null)
                        {
                            if (appointment.IsUserAppointment)
                            {
                                if (matched.DateSession.Date == appointment.Date)
								{
                                    var date = await db.DateSession.SingleOrDefaultAsync(x => x.Id == matched.DateSessionId);

                                    matched.TimeStamp = DateTime.Now;
                                    matched.BillingName = appointment.BillingName;
                                    matched.BillingBank = appointment.BillingBank;
                                    matched.BillingNumber = appointment.BillingNumber;
                                    date.Reserved -= matched.Ammount;
                                    if (matched.Status == AppointmentStatus.CONFIRMED)
                                    {
                                        date.Paid -= matched.Ammount;
                                        date.Sales -= matched.Price;
                                    }
                                    matched.Person.Clear();
                                    foreach (var x in appointment.People)
                                    {
                                        var person = new Person
                                        {
                                            DepartureId = x.Departure,
                                            Ammount = x.Ammount
                                        };
                                        var priceId = x.Price;
                                        person.Option.Add(new Option
                                        {
                                            PriceRule = await db.PriceRule.SingleOrDefaultAsync(x => x.Id == priceId)
                                        });
                                        if (x.Options != null)
                                        {
                                            foreach (var y in x.Options)
                                            {
                                                person.Option.Add(new Option
                                                {
                                                    PriceRule = await db.PriceRule.SingleOrDefaultAsync(x => x.Id == y)
                                                });
                                            }
                                        }
                                        matched.Person.Add(person);
                                    }

                                    matched.Ammount = appointment.People.Sum(x => x.Ammount);
                                    matched.Price = matched.Person.Sum
                                    (
                                        x => x.Option.Sum
                                        (
                                            x => x.PriceRule.RuleType switch
                                            {
                                                PriceRuleType.PERCENT_AS => matched.DateSession.Product.Price / 100 * x.PriceRule.Price,
                                                PriceRuleType.PERCENT_PLUS => matched.DateSession.Product.Price / 100 * x.PriceRule.Price,
                                                PriceRuleType.PERCENT_MINUS => -1 * matched.DateSession.Product.Price / 100 * x.PriceRule.Price,
                                                PriceRuleType.STATIC_PLUS => x.PriceRule.Price,
                                                PriceRuleType.STATIC_MINUS => -1 * x.PriceRule.Price,
                                                _ => 0
                                            }
                                        ) * x.Ammount
                                    );

                                    date.Reserved += matched.Ammount;
                                    if (matched.Status == AppointmentStatus.CONFIRMED)
                                    {
                                        date.Paid += matched.Ammount;
                                        date.Sales += matched.Price;
                                    }
                                    db.Appointment.Update(matched);
                                    db.DateSession.Update(date);
                                    await db.SaveChangesAsync();
                                    return Json(new Response
                                    {
                                        Result = ResultType.SUCCESS,
                                        Model = new
										{
                                            Result = true,
                                            Message = "예약이 성공적으로 변경되었습니다."
										}
                                    });
                                }
                                else
								{
                                    var oldDate = await db.DateSession.SingleOrDefaultAsync(x => x.Id == matched.DateSessionId);
                                    var newDate = await db.DateSession.SingleOrDefaultAsync(x => x.ProductId == matched.DateSession.ProductId && x.Date == appointment.Date);

                                    matched.TimeStamp = DateTime.Now;
                                    matched.DateSession = newDate;
                                    matched.BillingName = appointment.BillingName;
                                    matched.BillingBank = appointment.BillingBank;
                                    matched.BillingNumber = appointment.BillingNumber;
                                    oldDate.Reserved -= matched.Ammount;
                                    if (matched.Status == AppointmentStatus.CONFIRMED)
                                    {
                                        oldDate.Paid -= matched.Ammount;
                                        oldDate.Sales -= matched.Price;
                                    }
                                    matched.Person.Clear();
                                    foreach (var x in appointment.People)
                                    {
                                        var person = new Person
                                        {
                                            DepartureId = x.Departure,
                                            Ammount = x.Ammount
                                        };
                                        var priceId = x.Price;
                                        person.Option.Add(new Option
                                        {
                                            PriceRule = await db.PriceRule.SingleOrDefaultAsync(x => x.Id == priceId)
                                        });
                                        if (x.Options != null)
                                        {
                                            foreach (var y in x.Options)
                                            {
                                                person.Option.Add(new Option
                                                {
                                                    PriceRule = await db.PriceRule.SingleOrDefaultAsync(x => x.Id == y)
                                                });
                                            }
                                        }
                                        matched.Person.Add(person);
                                    }

                                    matched.Ammount = appointment.People.Sum(x => x.Ammount);
                                    matched.Price = matched.Person.Sum
                                    (
                                        x => x.Option.Sum
                                        (
                                            x => x.PriceRule.RuleType switch
                                            {
                                                PriceRuleType.PERCENT_AS => matched.DateSession.Product.Price / 100 * x.PriceRule.Price,
                                                PriceRuleType.PERCENT_PLUS => matched.DateSession.Product.Price / 100 * x.PriceRule.Price,
                                                PriceRuleType.PERCENT_MINUS => -1 * matched.DateSession.Product.Price / 100 * x.PriceRule.Price,
                                                PriceRuleType.STATIC_PLUS => x.PriceRule.Price,
                                                PriceRuleType.STATIC_MINUS => -1 * x.PriceRule.Price,
                                                _ => 0
                                            }
                                        ) * x.Ammount
                                    );

                                    newDate.Reserved += matched.Ammount;
                                    if (matched.Status == AppointmentStatus.CONFIRMED)
                                    {
                                        newDate.Paid += matched.Ammount;
                                        newDate.Sales += matched.Price;
                                    }
                                    db.Appointment.Update(matched);
                                    db.DateSession.Update(oldDate);
                                    db.DateSession.Update(newDate);
                                    await db.SaveChangesAsync();
                                    return Json(new Response
                                    {
                                        Result = ResultType.SUCCESS,
                                        Model = new
                                        {
                                            Result = true,
                                            Message = "예약이 성공적으로 변경되었습니다."
                                        }
                                    });
                                }
                            }
                            else
                            {
                                if (matched.DateSession.Date == appointment.Date)
                                {
                                    var date = await db.DateSession.SingleOrDefaultAsync(x => x.Id == matched.DateSessionId);

                                    matched.HashSalt = KeyGenerator.GenerateString(32);
                                    matched.Password = Password.Hash(appointment.Password, matched.HashSalt);
                                    matched.TimeStamp = DateTime.Now;
                                    matched.Name = appointment.Name;
                                    matched.Phone = appointment.Phone;
                                    matched.Address = appointment.Address;
                                    matched.BillingName = appointment.BillingName;
                                    matched.BillingBank = appointment.BillingBank;
                                    matched.BillingNumber = appointment.BillingNumber;
                                    date.Reserved -= matched.Ammount;
                                    if (matched.Status == AppointmentStatus.CONFIRMED)
                                    {
                                        date.Paid -= matched.Ammount;
                                        date.Sales -= matched.Price;
                                    }
                                    matched.Person.Clear();
                                    foreach (var x in appointment.People)
                                    {
                                        var person = new Person
                                        {
                                            DepartureId = x.Departure,
                                            Ammount = x.Ammount
                                        };
                                        var priceId = x.Price;
                                        person.Option.Add(new Option
                                        {
                                            PriceRule = await db.PriceRule.SingleOrDefaultAsync(x => x.Id == priceId)
                                        });
                                        if (x.Options != null)
                                        {
                                            foreach (var y in x.Options)
                                            {
                                                person.Option.Add(new Option
                                                {
                                                    PriceRule = await db.PriceRule.SingleOrDefaultAsync(x => x.Id == y)
                                                });
                                            }
                                        }
                                        matched.Person.Add(person);
                                    }

                                    matched.Ammount = appointment.People.Sum(x => x.Ammount);
                                    matched.Price = matched.Person.Sum
                                    (
                                        x => x.Option.Sum
                                        (
                                            x => x.PriceRule.RuleType switch
                                            {
                                                PriceRuleType.PERCENT_AS => matched.DateSession.Product.Price / 100 * x.PriceRule.Price,
                                                PriceRuleType.PERCENT_PLUS => matched.DateSession.Product.Price / 100 * x.PriceRule.Price,
                                                PriceRuleType.PERCENT_MINUS => -1 * matched.DateSession.Product.Price / 100 * x.PriceRule.Price,
                                                PriceRuleType.STATIC_PLUS => x.PriceRule.Price,
                                                PriceRuleType.STATIC_MINUS => -1 * x.PriceRule.Price,
                                                _ => 0
                                            }
                                        ) * x.Ammount
                                    );

                                    date.Reserved += matched.Ammount;
                                    if (matched.Status == AppointmentStatus.CONFIRMED)
                                    {
                                        date.Paid += matched.Ammount;
                                        date.Sales += matched.Price;
                                    }
                                    db.Appointment.Update(matched);
                                    db.DateSession.Update(date);
                                    await db.SaveChangesAsync();
                                    return Json(new Response
                                    {
                                        Result = ResultType.SUCCESS,
                                        Model = new
                                        {
                                            Result = true,
                                            Message = "예약이 성공적으로 변경되었습니다."
                                        }
                                    });
                                }
                                else
                                {
                                    var oldDate = await db.DateSession.SingleOrDefaultAsync(x => x.Id == matched.DateSessionId);
                                    var newDate = await db.DateSession.SingleOrDefaultAsync(x => x.ProductId == matched.DateSession.ProductId && x.Date == appointment.Date);

                                    matched.HashSalt = KeyGenerator.GenerateString(32);
                                    matched.Password = Password.Hash(appointment.Password, matched.HashSalt);
                                    matched.TimeStamp = DateTime.Now;
                                    matched.Name = appointment.Name;
                                    matched.Phone = appointment.Phone;
                                    matched.Address = appointment.Address;
                                    matched.DateSession = newDate;
                                    matched.BillingName = appointment.BillingName;
                                    matched.BillingBank = appointment.BillingBank;
                                    matched.BillingNumber = appointment.BillingNumber;
                                    oldDate.Reserved -= matched.Ammount;
                                    if (matched.Status == AppointmentStatus.CONFIRMED)
                                    {
                                        oldDate.Paid -= matched.Ammount;
                                        oldDate.Sales -= matched.Price;
                                    }
                                    matched.Person.Clear();
                                    foreach (var x in appointment.People)
                                    {
                                        var person = new Person
                                        {
                                            DepartureId = x.Departure,
                                            Ammount = x.Ammount
                                        };
                                        var priceId = x.Price;
                                        person.Option.Add(new Option
                                        {
                                            PriceRule = await db.PriceRule.SingleOrDefaultAsync(x => x.Id == priceId)
                                        });
                                        if (x.Options != null)
                                        {
                                            foreach (var y in x.Options)
                                            {
                                                person.Option.Add(new Option
                                                {
                                                    PriceRule = await db.PriceRule.SingleOrDefaultAsync(x => x.Id == y)
                                                });
                                            }
                                        }
                                        matched.Person.Add(person);
                                    }

                                    matched.Ammount = appointment.People.Sum(x => x.Ammount);
                                    matched.Price = matched.Person.Sum
                                    (
                                        x => x.Option.Sum
                                        (
                                            x => x.PriceRule.RuleType switch
                                            {
                                                PriceRuleType.PERCENT_AS => matched.DateSession.Product.Price / 100 * x.PriceRule.Price,
                                                PriceRuleType.PERCENT_PLUS => matched.DateSession.Product.Price / 100 * x.PriceRule.Price,
                                                PriceRuleType.PERCENT_MINUS => -1 * matched.DateSession.Product.Price / 100 * x.PriceRule.Price,
                                                PriceRuleType.STATIC_PLUS => x.PriceRule.Price,
                                                PriceRuleType.STATIC_MINUS => -1 * x.PriceRule.Price,
                                                _ => 0
                                            }
                                        ) * x.Ammount
                                    );

                                    newDate.Reserved += matched.Ammount;
                                    if (matched.Status == AppointmentStatus.CONFIRMED)
                                    {
                                        newDate.Paid += matched.Ammount;
                                        newDate.Sales += matched.Price;
                                    }
                                    db.Appointment.Update(matched);
                                    db.DateSession.Update(oldDate);
                                    db.DateSession.Update(newDate);
                                    await db.SaveChangesAsync();
                                    return Json(new Response
                                    {
                                        Result = ResultType.SUCCESS,
                                        Model = new
                                        {
                                            Result = true,
                                            Message = "예약이 성공적으로 변경되었습니다."
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
                                Message = "해당 예약을 찾을 수 없습니다."
							}
                        });
                    }
                    else return Json(new Response { Result = ResultType.ACCESS_DENIED });
				}
                else return Json(new Response { Result = ResultType.LOGIN_REQUIRED });
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

        public async Task<IActionResult> CancelSession(int id, string message)
		{
            try
            {
                if (IsAdministrator)
                {
                    using var db = new RomanceTourDbContext();
                    var session = await db.DateSession
                        .Include(x => x.Product)
                        .Include(x => x.Appointment)
                            .ThenInclude(x => x.User)
                        .SingleOrDefaultAsync(x => x.Id == id);

                    var phone = session.Appointment.Where(x => !x.IsUserAppointment).Select(x => x.Phone).ToList();
                    phone.AddRange(session.Appointment.Where(x => x.IsUserAppointment).Select(x => x.User.Phone));
                    session.Reserved = 0;
                    session.Paid = 0;
                    session.Sales = 0;
                    session.Status = DateSessionStatus.CANCELED;
                    db.DateSession.Update(session);
                    await db.SaveChangesAsync();
                    var result = await MessageSender.SendCustomMessage(phone.ToArray(), session.Product.Title, session.Date, message);
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
                    Result = ResultType.SYSTEM_ERROR,
                    Error = e
                });
            }
        }
   }
}