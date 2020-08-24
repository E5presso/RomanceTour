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
	/// 전송할 SMS 메세지의 내용을 구성합니다.
	/// </summary>
	public class Message
	{
		/// <summary>
		/// 수신할 대상을 가져오거나 설정합니다.
		/// </summary>
		public string[] To { get; set; }
		/// <summary>
		/// 발송할 메세지의 본문을 가져오거나 설정합니다.
		/// </summary>
		public string Content { get; set; }
	}
	/// <summary>
	/// 네이버 클라우드 플랫폼(NCloud)의 SENS 서비스를 활용한 SMS 웹 발송 기능을 구현합니다.
	/// </summary>
	public class Sens
	{
		/// <summary>
		/// SENS 서비스의 API URL을 가져옵니다.
		/// </summary>
		public string URL { get; private set; }
		/// <summary>
		/// SENS 서비스의 API URI를 가져옵니다.
		/// </summary>
		public string URI { get; private set; }
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
		/// <param name="uri">서비스 URL를 지정합니다.</param>
		/// <param name="serviceId">Service-ID를 지정합니다.</param>
		/// <param name="accessKey">Access Key를 지정합니다.</param>
		/// <param name="secretKey">Secret-Key를 지정합니다.</param>
		/// <param name="from">발송 시 사용할 전화번호를 지정합니다.</param>
		public Sens(string url, string uri, string serviceId, string accessKey, string secretKey, string from)
		{
			URL = url;
			URI = string.Format(uri, serviceId);
			ServiceId = serviceId;
			AccessKey = accessKey;
			SecretKey = secretKey;
			From = from;
		}

		/// <summary>
		/// SMS 메세지를 발송합니다.
		/// </summary>
		/// <param name="message">발송할 메세지를 지정합니다.</param>
		/// <returns>발송 결과를 반환합니다.</returns>
		public async Task<(HttpStatusCode, dynamic)> SendMessage(Message message)
		{
			try
			{
				long timestamp = GetTimeStamp();
				string signature = GetSignature(timestamp);

				HttpWebRequest request = WebRequest.Create($"{URL}{URI}") as HttpWebRequest;
				request.Method = "POST";
				request.Headers.Add("Content-Type", "application/json; charset=utf-8");
				request.Headers.Add("x-ncp-apigw-timestamp", timestamp.ToString());
				request.Headers.Add("x-ncp-iam-access-key", AccessKey);
				request.Headers.Add("x-ncp-apigw-signature-v2", signature);
				var body = new
				{
					type = "SMS",
					contentType = "COMM",
					countryCode = "82",
					from = From,
					content = message.Content,
					messages = new List<object>()
				};
				foreach (var x in message.To)
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
				return (statusCode, JsonSerializer.Deserialize(responseMessage));
			}
			catch { throw; }
		}

		private long GetTimeStamp()
		{
			var delta = DateTime.UtcNow - DateTime.Parse("1970-01-01 00:00:00");
			return Convert.ToInt64(delta.TotalMilliseconds);
		}
		private string GetSignature(long timestamp)
		{
			string space = " ";
			string newLine = "\n";
			string method = "POST";
			string message = new StringBuilder()
				.Append(method)
				.Append(space)
				.Append(URI)
				.Append(newLine)
				.Append(timestamp)
				.Append(newLine)
				.Append(AccessKey)
				.ToString();
			return HMAC.SHA256(SecretKey, message);
		}
	}
}