using Core.Security;
using Core.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Network.NCloud
{
	/// <summary>
	/// 발송할 메세지 타입을 정의합니다.
	/// </summary>
	public enum MessageType
	{
		/// <summary>
		/// SMS 메세지입니다.
		/// </summary>
		SMS,
		/// <summary>
		/// LMS 메세지입니다.
		/// </summary>
		LMS,
		/// <summary>
		/// MMS 메세지입니다.
		/// </summary>
		MMS
	}
	/// <summary>
	/// 네이버 클라우드 플랫폼(NCloud)의 SENS 서비스를 활용한 SMS 웹 발송 기능을 구현합니다.
	/// </summary>
	public class Sens
	{
		private readonly string uri;

		/// <summary>
		/// SENS 서비스의 API URL을 가져옵니다.
		/// </summary>
		public string URL { get; private set; }
		/// <summary>
		/// SENS 서비스의 Service-ID 를 가져옵니다.
		/// </summary>
		public string ServiceId { get; private set; }
		/// <summary>
		/// SENS 서비스의 Access Key 를 가져옵니다.
		/// </summary>
		public string AccessKey { get; private set; }
		/// <summary>
		/// SENS 서비스의 Secret Key 를 가져옵니다.
		/// </summary>
		public string SecretKey { get; private set; }
		/// <summary>
		/// SMS 발송 시 사용할 전화번호를 가져옵니다.
		/// </summary>
		public string From { get; private set; }

		/// <summary>
		/// Sens 클래스를 초기화합니다.
		/// </summary>
		/// <param name="url">서비스 URL을 지정합니다.</param>
		/// <param name="uri">메세지 전송 URI를 지정합니다.</param>
		/// <param name="serviceId">Service-ID를 지정합니다.</param>
		/// <param name="accessKey">Access Key를 지정합니다.</param>
		/// <param name="secretKey">Secret-Key를 지정합니다.</param>
		/// <param name="from">발송 시 사용할 전화번호를 지정합니다.</param>
		public Sens(string url, string uri, string serviceId, string accessKey, string secretKey, string from)
		{
			URL = url;
			this.uri = uri;
			ServiceId = serviceId;
			AccessKey = accessKey;
			SecretKey = secretKey;
			From = from;
		}

		/// <summary>
		/// SMS 메세지를 발송합니다.
		/// </summary>
		/// <param name="type">발송할 메세지의 타입을 지정합니다.</param>
		/// <param name="subject">발송할 메세지의 제목을 지정합니다.</param>
		/// <param name="to">수신 대상자를 지정합니다.</param>
		/// <param name="message">발송할 메세지를 지정합니다.</param>
		/// <returns>발송 요청에 대한 요청식별자를 반환합니다.</returns>
		public async Task<bool> SendMessage(MessageType type, string subject, string to, string message)
		{
			try
			{
				string method = "POST";
				string uri = string.Format(this.uri, ServiceId);
				long timestamp = GetTimeStamp();
				string signature = GetSignature(timestamp, method, uri);

				HttpWebRequest request = WebRequest.Create($"{URL}{uri}") as HttpWebRequest;
				request.Method = method;
				request.Headers.Add("Content-Type", "application/json; charset=utf-8");
				request.Headers.Add("x-ncp-apigw-timestamp", timestamp.ToString());
				request.Headers.Add("x-ncp-iam-access-key", AccessKey);
				request.Headers.Add("x-ncp-apigw-signature-v2", signature);
				var body = new
				{
					type = type switch
					{
						MessageType.SMS => "SMS",
						MessageType.LMS => "LMS",
						MessageType.MMS => "MMS",
						_ => ""
					},
					contentType = "COMM",
					countryCode = "82",
					from = From,
					subject,
					content = message,
					messages = new[]
					{
						new
						{
							to
						}
					}
				};
				byte[] binary = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(body));

				using var requestStream = await request.GetRequestStreamAsync();
				await requestStream.WriteAsync(binary, 0, binary.Length);
				requestStream.Close();

				HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;
				var statusCode = response.StatusCode;
				using var responseStream = response.GetResponseStream();
				using var reader = new StreamReader(responseStream);
				string responseMessage = await reader.ReadToEndAsync();
				dynamic result = JsonSerializer.Deserialize(responseMessage);
				return statusCode == HttpStatusCode.Accepted;
			}
			catch { return false; }
		}
		/// <summary>
		/// SMS 메세지를 발송합니다.
		/// </summary>
		/// <param name="type">발송할 메세지의 타입을 지정합니다.</param>
		/// <param name="subject">발송할 메세지의 제목을 지정합니다.</param>
		/// <param name="to">수신 대상자 목록을 지정합니다.</param>
		/// <param name="message">발송할 메세지를 지정합니다.</param>
		/// <returns>발송 요청에 대한 요청식별자를 반환합니다.</returns>
		public async Task<bool> SendMessage(MessageType type, string subject, string[] to, string message)
		{
			try
			{
				string method = "POST";
				string uri = string.Format(this.uri, ServiceId);
				long timestamp = GetTimeStamp();
				string signature = GetSignature(timestamp, method, uri);

				HttpWebRequest request = WebRequest.Create($"{URL}{uri}") as HttpWebRequest;
				request.Method = method;
				request.Headers.Add("Content-Type", "application/json; charset=utf-8");
				request.Headers.Add("x-ncp-apigw-timestamp", timestamp.ToString());
				request.Headers.Add("x-ncp-iam-access-key", AccessKey);
				request.Headers.Add("x-ncp-apigw-signature-v2", signature);
				var body = new
				{
					type = type switch
					{
						MessageType.SMS => "SMS",
						MessageType.LMS => "LMS",
						MessageType.MMS => "MMS",
						_ => ""
					},
					contentType = "COMM",
					countryCode = "82",
					from = From,
					subject,
					content = message,
					messages = new List<object>()
				};
				foreach (var x in to)
				{
					body.messages.Add(new
					{
						to = x
					});
				}
				byte[] binary = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(body));

				using var requestStream = await request.GetRequestStreamAsync();
				await requestStream.WriteAsync(binary, 0, binary.Length);
				requestStream.Close();

				HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;
				var statusCode = response.StatusCode;
				using var responseStream = response.GetResponseStream();
				using var reader = new StreamReader(responseStream);
				string responseMessage = await reader.ReadToEndAsync();
				dynamic result = JsonSerializer.Deserialize(responseMessage);
				return statusCode == HttpStatusCode.Accepted;
			}
			catch { return false; }
		}
		/// <summary>
		/// SMS 메세지를 발송합니다.
		/// </summary>
		/// <param name="type">발송할 메세지의 타입을 지정합니다.</param>
		/// <param name="subject">발송할 메세지의 제목을 지정합니다.</param>
		/// <param name="message">발송할 메세지를 지정합니다.</param>
		/// <param name="to">수신 대상자 목록을 지정합니다.</param>
		/// <returns>발송 요청에 대한 요청식별자를 반환합니다.</returns>
		public async Task<bool> SendMessage(MessageType type, string subject, string message, params string[] to)
		{
			try
			{
				string method = "POST";
				string uri = string.Format(this.uri, ServiceId);
				long timestamp = GetTimeStamp();
				string signature = GetSignature(timestamp, method, uri);

				HttpWebRequest request = WebRequest.Create($"{URL}{uri}") as HttpWebRequest;
				request.Method = method;
				request.Headers.Add("Content-Type", "application/json; charset=utf-8");
				request.Headers.Add("x-ncp-apigw-timestamp", timestamp.ToString());
				request.Headers.Add("x-ncp-iam-access-key", AccessKey);
				request.Headers.Add("x-ncp-apigw-signature-v2", signature);
				var body = new
				{
					type = type switch
					{
						MessageType.SMS => "SMS",
						MessageType.LMS => "LMS",
						MessageType.MMS => "MMS",
						_ => ""
					},
					contentType = "COMM",
					countryCode = "82",
					from = From,
					subject,
					content = message,
					messages = new List<object>()
				};
				foreach (var x in to)
				{
					body.messages.Add(new
					{
						to = x
					});
				}
				byte[] binary = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(body));

				using var requestStream = await request.GetRequestStreamAsync();
				await requestStream.WriteAsync(binary, 0, binary.Length);
				requestStream.Close();

				HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;
				var statusCode = response.StatusCode;
				using var responseStream = response.GetResponseStream();
				using var reader = new StreamReader(responseStream);
				string responseMessage = await reader.ReadToEndAsync();
				dynamic result = JsonSerializer.Deserialize(responseMessage);
				return statusCode == HttpStatusCode.Accepted;
			}
			catch { return false; }
		}

		private long GetTimeStamp()
		{
			var delta = DateTime.UtcNow - DateTime.Parse("1970-01-01 00:00:00");
			return Convert.ToInt64(delta.TotalMilliseconds);
		}
		private string GetSignature(long timestamp, string method, string uri)
		{
			string space = " ";
			string newLine = "\n";
			string message = new StringBuilder()
				.Append(method)
				.Append(space)
				.Append(uri)
				.Append(newLine)
				.Append(timestamp)
				.Append(newLine)
				.Append(AccessKey)
				.ToString();
			return HMAC.Compute(message, SecretKey);
		}
	}
}