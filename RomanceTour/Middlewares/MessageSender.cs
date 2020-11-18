using Core.Network.NCloud;
using System;
using System.Threading.Tasks;

namespace RomanceTour.Middlewares
{
	public static class MessageSender
	{
		private static readonly Sens sens;

		private static readonly string appointmentSubject = XmlConfiguration.AppointmentSubject;
		private static readonly string appointmentTemplate = XmlConfiguration.AppointmentTemplate;

		private static readonly string cancelSubject = XmlConfiguration.CancelSubject;
		private static readonly string cancelTemplate = XmlConfiguration.CancelTemplate;

		static MessageSender()
		{
			var api = XmlConfiguration.NCloudAPI;
			sens = new Sens(api.URL, api.URI, api.ServiceId, api.AccessKey, api.SecretKey, api.From);
		}

		public static async Task<bool> SendAppointmentMessage(string phone, string name, string productName, DateTime date, string link)
		{
			return await sens.SendMessage(MessageType.LMS, appointmentSubject, phone, string.Format(appointmentTemplate, name, productName, date.ToShortDateString(), link));
		}
		public static async Task<bool> SendCancelAppointmentMessage(string[] phone, string productName, DateTime date, string message)
		{
			return await sens.SendMessage(MessageType.LMS, cancelSubject, string.Format(cancelTemplate, productName, date.ToShortDateString(), message), phone);
		}
		public static async Task<bool> SendCustomMessage(string[] phone, string subject, string message)
		{
			return await sens.SendMessage(MessageType.LMS, $"[낭만투어] {subject}", message, phone);
		}
	}
}