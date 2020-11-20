using Core.Network.NCloud;

using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace RomanceTour.Middlewares
{
	public enum VerificationResult
	{
		SUCCESS,				// 발송 성공 시
		FAILURE,				// 발송 실패 시
		TOO_MUCH_REQUEST,		// 너무 빠른 요청
		MAX_REQUEST_REACHED,	// 일일 요청한도 도달
		USER_NOT_FOUND			// 존재하지 않는 사용자
	}
	public static class PhoneVerifier
	{
		private static readonly Sens sens;
		private static readonly MemoryCache cache = MemoryCache.Default;
		private static readonly Dictionary<string, string> dict = new Dictionary<string, string>();
		private static readonly string template = XmlConfiguration.Verification.Template;
		private static readonly int codeLength = XmlConfiguration.Verification.CodeLength;

		static PhoneVerifier()
		{
			var api = XmlConfiguration.NCloudAPI;
			sens = new Sens(api.URL, api.URI, api.ServiceId, api.AccessKey, api.SecretKey, api.From);
		}

		public static async Task<VerificationResult> CreateVerification(string phone)
		{
			if (cache.Contains(phone))
			{
				(_, _, var time) = ((string, string, DateTime))cache.Get(phone);
				if (DateTime.Now - time > new TimeSpan(0, 1, 0))
				{
					string code = GenerateVerificationCode();
					bool result = await sens.SendMessage(MessageType.SMS, "[낭만투어] 휴대폰 인증", phone,  string.Format(template, code));
					if (result)
					{
						var now = DateTime.Now;
						var token = Guid.NewGuid().ToString();
						cache.Set(phone, (code, token, now), DateTimeOffset.Now.AddMinutes(3));
						return VerificationResult.SUCCESS;
					}
					else return VerificationResult.FAILURE;
				}
				else return VerificationResult.TOO_MUCH_REQUEST;
			}
			else
			{
				string code = GenerateVerificationCode();
				bool result = await sens.SendMessage(MessageType.SMS, "[낭만투어] 휴대폰 인증", phone, string.Format(template, code));
				if (result)
				{
					var now = DateTime.Now;
					var token = Guid.NewGuid().ToString();
					cache.Set(phone, (code, token, now), DateTimeOffset.Now.AddMinutes(3));
					return VerificationResult.SUCCESS;
				}
				else return VerificationResult.FAILURE;
			}
		}
		public static (bool, string) Challenge(string phone, string code)
		{
			if (cache.Contains(phone))
			{
				(var number, var token, _) = ((string, string, DateTime))cache.Get(phone);
				if (code == number)
				{
					dict.Add(token, phone);
					return (true, token);
				}
				else return (false, null);
			}
			else return (false, null);
		}
		public static string Retrieve(string token)
		{
			if (dict.ContainsKey(token))
			{
				string phone = dict[token];
				dict.Remove(token);
				return phone;
			}
			else return null;
		}

		private static string GenerateVerificationCode()
		{
			Random rng = new Random((int)DateTime.Now.Ticks);
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < codeLength; i++)
				builder.Append(rng.Next(0, 10));
			return builder.ToString();
		}
	}
}