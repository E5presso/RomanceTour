using System.Security.Cryptography;
using System.Text;

namespace Core.Security
{
	/// <summary>
	/// 해쉬함수 모음입니다.
	/// </summary>
	public class Hash
	{
		private static readonly MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
		private static readonly SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
		private static readonly SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
		private static readonly SHA384CryptoServiceProvider sha384 = new SHA384CryptoServiceProvider();
		private static readonly SHA512CryptoServiceProvider sha512 = new SHA512CryptoServiceProvider();

		/// <summary>
		/// 해쉬코드를 생성합니다.
		/// </summary>
		/// <param name="message">해쉬코드 생성에 사용할 문자열입니다.</param>
		/// <param name="algorithm">적용할 해쉬알고리즘을 지정합니다.</param>
		/// <returns>생성된 해쉬코드입니다.</returns>
		public static string Compute(string message, HashAlgorithm algorithm = HashAlgorithm.SHA256)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(message);
			byte[] hash = algorithm switch
			{
				HashAlgorithm.MD5 => md5.ComputeHash(bytes),
				HashAlgorithm.SHA1 => sha1.ComputeHash(bytes),
				HashAlgorithm.SHA256 => sha256.ComputeHash(bytes),
				HashAlgorithm.SHA384 => sha384.ComputeHash(bytes),
				HashAlgorithm.SHA512 => sha512.ComputeHash(bytes),
				_ => sha256.ComputeHash(bytes)
			};
			return Base64.Encode(hash);
		}
		/// <summary>
		/// 해쉬코드를 생성합니다.
		/// </summary>
		/// <param name="binary">해쉬코드 생성에 사용할 바이트시퀀스입니다.</param>
		/// <param name="algorithm">적용할 해쉬알고리즘을 지정합니다.</param>
		/// <returns>생성된 해쉬코드입니다.</returns>
		public static byte[] Compute(byte[] binary, HashAlgorithm algorithm = HashAlgorithm.SHA256)
		{
			return algorithm switch
			{
				HashAlgorithm.MD5 => md5.ComputeHash(binary),
				HashAlgorithm.SHA1 => sha1.ComputeHash(binary),
				HashAlgorithm.SHA256 => sha256.ComputeHash(binary),
				HashAlgorithm.SHA384 => sha384.ComputeHash(binary),
				HashAlgorithm.SHA512 => sha512.ComputeHash(binary),
				_ => sha256.ComputeHash(binary)
			};
		}
	}
}