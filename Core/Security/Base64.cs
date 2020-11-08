using System;
using System.Text;

namespace Core.Security
{
	/// <summary>
	/// URL-Safe Base64 변환기입니다.
	/// </summary>
	public static class UrlSafeBase64
	{
		/// <summary>
		/// 바이트 시퀀스를 Base64 문자열로 변환합니다.
		/// </summary>
		/// <param name="binary">바이트 시퀀스입니다.</param>
		/// <returns>Base64 문자열입니다.</returns>
		public static string Encode(byte[] binary)
		{
			return Convert.ToBase64String(binary).Split('=')[0].Replace('+', '-').Replace('/', '_');
		}
		/// <summary>
		/// Base64 문자열을 바이트 시퀀스로 변환합니다.
		/// </summary>
		/// <param name="base64">Base64 문자열입니다.</param>
		/// <returns>바이트 시퀀스입니다.</returns>
		public static byte[] Decode(string base64)
		{
			string temp = base64.Replace('-', '+').Replace('_', '/');
			switch (temp.Length % 4)
			{
				case 0: break;
				case 2:
				{
					temp += "==";
					break;
				}
				case 3:
				{
					temp += "=";
					break;
				}
				default: throw new InvalidOperationException("정상적인 Base64 문자열이 아닙니다.");
			}
			return Convert.FromBase64String(temp);
		}

		/// <summary>
		/// BASE64 문자열을 UTF8 문자열로 변환합니다.
		/// </summary>
		/// <param name="base64">BASE64 문자열을 지정합니다.</param>
		/// <returns>UTF8 문자열을 반환합니다.</returns>
		public static string FromBase64(string base64)
		{
			return Encoding.UTF8.GetString(Decode(base64));
		}
		/// <summary>
		/// UTF8 문자열을 BASE64 문자열로 변환합니다.
		/// </summary>
		/// <param name="utf8">UTF8 문자열을 지정합니다.</param>
		/// <returns>BASE64 문자열을 반환합니다.</returns>
		public static string ToBase64(string utf8)
		{
			return Encode(Encoding.UTF8.GetBytes(utf8));
		}

		/// <summary>
		/// URL-Safe Base64 문자열을 Base64로 변환합니다.
		/// </summary>
		/// <param name="urlSafe">URL-Safe Base64 문자열입니다.</param>
		/// <returns>Base64 문자열을 반환합니다.</returns>
		public static string ToNonSafe(string urlSafe)
		{
			string temp = urlSafe.Replace('-', '+').Replace('_', '/');
			switch (temp.Length % 4)
			{
				case 0: break;
				case 2:
				{
					temp += "==";
					break;
				}
				case 3:
				{
					temp += "=";
					break;
				}
				default: throw new InvalidOperationException("정상적인 Base64 문자열이 아닙니다.");
			}
			return temp;
		}
	}
	/// <summary>
	/// Base64 변환기입니다.
	/// </summary>
	public static class Base64
	{
		/// <summary>
		/// 바이트 시퀀스를 Base64 문자열로 변환합니다.
		/// </summary>
		/// <param name="binary">바이트 시퀀스입니다.</param>
		/// <returns>Base64 문자열입니다.</returns>
		public static string Encode(byte[] binary)
		{
			return Convert.ToBase64String(binary);
		}
		/// <summary>
		/// Base64 문자열을 바이트 시퀀스로 변환합니다.
		/// </summary>
		/// <param name="base64">Base64 문자열입니다.</param>
		/// <returns>바이트 시퀀스입니다.</returns>
		public static byte[] Decode(string base64)
		{
			return Convert.FromBase64String(base64);
		}

		/// <summary>
		/// BASE64 문자열을 UTF8 문자열로 변환합니다.
		/// </summary>
		/// <param name="base64">BASE64 문자열을 지정합니다.</param>
		/// <returns>UTF8 문자열을 반환합니다.</returns>
		public static string FromBase64(string base64)
		{
			return Encoding.UTF8.GetString(Decode(base64));
		}
		/// <summary>
		/// UTF8 문자열을 BASE64 문자열로 변환합니다.
		/// </summary>
		/// <param name="utf8">UTF8 문자열을 지정합니다.</param>
		/// <returns>BASE64 문자열을 반환합니다.</returns>
		public static string ToBase64(string utf8)
		{
			return Encode(Encoding.UTF8.GetBytes(utf8));
		}

		/// <summary>
		/// Base64 문자열을 URL-Safe Base64로 변환합니다.
		/// </summary>
		/// <param name="nonSafe">Base64 문자열입니다.</param>
		/// <returns>URL-Safe Base64 문자열을 반환합니다.</returns>
		public static string ToUrlSafe(string nonSafe)
		{
			return nonSafe.Split('=')[0].Replace('+', '-').Replace('/', '_');
		}
	}
}