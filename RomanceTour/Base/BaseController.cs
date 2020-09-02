using System;
using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using RomanceTour.Middlewares;

namespace RomanceTour
{
	public class BaseController : Controller
	{
		protected int? SessionId
		{
			get => HttpContext.Session.GetInt32(Session.Id);
			private set
			{
				if (value == null) throw new InvalidOperationException("세션의 값은 null일 수 없습니다.");
				else HttpContext.Session.SetInt32(Session.Id, (int)value);
			}
		}
		protected bool IsLoggedIn => HttpContext.Session.GetInt32(Session.Id) != null;
		protected string IPAddress => Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
		protected string Name
		{
			get => HttpContext.Session.GetString(Session.Name);
			private set => HttpContext.Session.SetString(Session.Name, value);
		}

		protected string Directory => ControllerContext.RouteData.Values["controller"].ToString();
		protected string Action => ControllerContext.RouteData.Values["action"].ToString();
		protected string Back => Request != null ? Request.Headers["Referer"].ToString() : string.Empty;

		protected bool IsAdministrator => XmlConfiguration.Administrator.Id == SessionId;

		protected void AddSession(int id, string name)
		{
			if (!IsLoggedIn)
			{
				SessionId = id;
				Name = name;
			}
		}
		protected void RemoveSession()
		{
			if (IsLoggedIn) HttpContext.Session.Clear();
		}

		public override async void OnActionExecuting(ActionExecutingContext context)
		{
			try
			{
				string ip = IPAddress;
				int? userId = SessionId;
				string controller = Directory;
				string action = Action;
				string parameter = context.HttpContext.Request.QueryString.ToString();

				await LogManager.LogAsync(ip, userId, controller, action, parameter);
				base.OnActionExecuting(context);
			}
			catch (Exception e)
			{
				await LogManager.ErrorAsync(e);
			}
		}
	}
}