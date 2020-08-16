using System.Text;
using System.Security.Cryptography;

namespace Core.Security
{
	/// <summary>
	/// 해쉬 기반 메세지 인증 코드의 생성을 구현합니다.
	/// </summary>
	public static class HMAC
	{
		/// <summary>
		/// SHA256 기반의 HMAC을 생성합니다.
		/// </summary>
		/// <param name="key">HMAC의 생성에 사용할 비밀키를 지정합니다.</param>
		/// <param name="message">HMAC을 적용할 메세지 본문을 지정합니다.</param>
		/// <returns>생성된 HMAC을 반환합니다.</returns>
		public static string SHA256(string key, string message)
		{
			var key_array = Encoding.UTF8.GetBytes(key);
			var message_array = Encoding.UTF8.GetBytes(message);
			using HMACSHA256 hmac = new HMACSHA256(key_array);
			hmac.Initialize();
			var hash = hmac.ComputeHash(message_array);
			return Base64.GetString(hash);
		}
	}
}