using System.Text;
using System.Security.Cryptography;
using System;

namespace Core.Security
{
	/// <summary>
	/// 해쉬 기반 메세지 인증 코드의 생성을 구현합니다.
	/// </summary>
	public static class HMAC
	{
		/// <summary>
		/// HMAC을 생성합니다.
		/// </summary>
		/// <param name="message">HMAC을 적용할 메세지 본문을 지정합니다.</param>
		/// <param name="key">HMAC의 생성에 사용할 비밀키를 지정합니다.</param>
		/// <param name="algorithm">적용할 알고리즘을 지정합니다.</param>
		/// <returns>생성된 HMAC을 반환합니다.</returns>
		public static string Compute(string message, string key, SignAlgorithm algorithm = SignAlgorithm.HS256)
		{
			var key_array = Encoding.UTF8.GetBytes(key);
			var message_array = Encoding.UTF8.GetBytes(message);
			using dynamic hmac = algorithm switch
			{
				SignAlgorithm.HS256 => new HMACSHA256(key_array),
				SignAlgorithm.HS384 => new HMACSHA384(key_array),
				SignAlgorithm.HS512 => new HMACSHA512(key_array),
				_ => throw new ArgumentException("유효하지 않은 알고리즘입니다.")
			};
			hmac.Initialize();
			var hash = hmac.ComputeHash(message_array);
			return Base64.Encode(hash);
		}
		/// <summary>
		/// HMAC을 생성합니다.
		/// </summary>
		/// <param name="message">HMAC을 적용할 메세지 본문을 지정합니다.</param>
		/// <param name="key">HMAC의 생성에 사용할 비밀키를 지정합니다.</param>
		/// <param name="algorithm">적용할 알고리즘을 지정합니다.</param>
		/// <returns>생성된 HMAC을 반환합니다.</returns>
		public static string Compute(string message, Key key, SignAlgorithm algorithm = SignAlgorithm.HS256)
		{
			var key_array = key;
			var message_array = Encoding.UTF8.GetBytes(message);
			using dynamic hmac = algorithm switch
			{
				SignAlgorithm.HS256 => new HMACSHA256(key_array),
				SignAlgorithm.HS384 => new HMACSHA384(key_array),
				SignAlgorithm.HS512 => new HMACSHA512(key_array),
				_ => throw new ArgumentException("유효하지 않은 알고리즘입니다.")
			};
			hmac.Initialize();
			var hash = hmac.ComputeHash(message_array);
			return Base64.Encode(hash);
		}
		/// <summary>
		/// HMAC을 생성합니다.
		/// </summary>
		/// <param name="message">HMAC을 적용할 메세지 본문을 지정합니다.</param>
		/// <param name="key">HMAC의 생성에 사용할 비밀키를 지정합니다.</param>
		/// <param name="algorithm">적용할 알고리즘을 지정합니다.</param>
		/// <returns>생성된 HMAC을 반환합니다.</returns>
		public static byte[] Compute(byte[] message, byte[] key, SignAlgorithm algorithm = SignAlgorithm.HS256)
		{
			using dynamic hmac = algorithm switch
			{
				SignAlgorithm.HS256 => new HMACSHA256(key),
				SignAlgorithm.HS384 => new HMACSHA384(key),
				SignAlgorithm.HS512 => new HMACSHA512(key),
				_ => throw new ArgumentException("유효하지 않은 알고리즘입니다.")
			};
			hmac.Initialize();
			return hmac.ComputeHash(message);
		}
		/// <summary>
		/// HMAC을 생성합니다.
		/// </summary>
		/// <param name="message">HMAC을 적용할 메세지 본문을 지정합니다.</param>
		/// <param name="key">HMAC의 생성에 사용할 비밀키를 지정합니다.</param>
		/// <param name="algorithm">적용할 알고리즘을 지정합니다.</param>
		/// <returns>생성된 HMAC을 반환합니다.</returns>
		public static byte[] Compute(byte[] message, Key key, SignAlgorithm algorithm = SignAlgorithm.HS256)
		{
			var key_array = key;
			using dynamic hmac = algorithm switch
			{
				SignAlgorithm.HS256 => new HMACSHA256(key_array),
				SignAlgorithm.HS384 => new HMACSHA384(key_array),
				SignAlgorithm.HS512 => new HMACSHA512(key_array),
				_ => throw new ArgumentException("유효하지 않은 알고리즘입니다.")
			};
			hmac.Initialize();
			return hmac.ComputeHash(message);
		}
	}
}