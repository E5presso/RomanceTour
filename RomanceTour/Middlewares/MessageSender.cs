using Core.Network.NCloud;
using System;
using System.Threading.Tasks;

namespace RomanceTour.Middlewares
{
	public static class MessageSender
	{
		private static readonly Sens sens;
		private static readonly string subject = XmlConfiguration.AppointmentSubject;
		private static readonly string template = XmlConfiguration.AppointmentTemplate;

		static MessageSender()
		{
			var api = XmlConfiguration.NCloudAPI;
			sens = new Sens(api.URL, api.URI, api.ServiceId, api.AccessKey, api.SecretKey, api.From);
		}

		public static async Task<bool> SendAppointmentMessage(string phone, string name, string productName, DateTime date, string link)
		{
			return await sens.SendMessage(MessageType.LMS, subject, phone, string.Format(template, name, productName, date.ToShortDateString(), link));
		}
	}
}