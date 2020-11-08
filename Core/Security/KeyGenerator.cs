using System;
using System.Security.Cryptography;

namespace Core.Security
{
	/// <summary>
	/// 키 생성기입니다.
	/// </summary>
	public class KeyGenerator
	{
		private static readonly RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider(BitConverter.GetBytes(DateTime.Now.ToBinary()));

		/// <summary>
		/// 랜덤한 문자열을 생성합니다.
		/// </summary>
		/// <param name="length">생성할 키의 길이입니다.</param>
		/// <returns>생성된 문자열입니다.</returns>
		public static string GenerateString(int length)
		{
			byte[] array = new byte[length];
			provider.GetBytes(array);
			return Base64.Encode(array);
		}
		/// <summary>
		/// 랜덤한 바이트 시퀀스를 생성합니다.
		/// </summary>
		/// <param name="length">생성할 시퀀스의 길이입니다.</param>
		/// <returns>생성된 시퀀스입니다.</returns>
		public static byte[] GenerateBytes(int length)
		{
			byte[] array = new byte[length];
			provider.GetBytes(array);
			return array;
		}
		/// <summary>
		/// 랜덤한 키를 생성합니다.
		/// </summary>
		/// <param name="length">생성할 키의 길이입니다.</param>
		/// <returns>생성된 키입니다.</returns>
		public static Key Generate(int length)
		{
			byte[] array = new byte[length];
			provider.GetBytes(array);
			return array;
		}
	}
}