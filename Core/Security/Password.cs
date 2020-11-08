using System.Security.Cryptography;
using System.Text;

namespace Core.Security
{
	/// <summary>
	/// 비밀번호의 단방향 암호화를 위한 PBKDF2 알고리즘을 제공합니다.
	/// </summary>
	public static class Password
	{
		private const int HASH_SIZE = 32;
		private const int ITERATION = 131072;

		/// <summary>
		/// 비밀번호에 대해 단방향 암호화를 수행합니다.
		/// </summary>
		/// <param name="password">암호화할 비밀번호를 지정합니다.</param>
		/// <param name="salt">암호화에 사용할 솔트를 지정합니다.</param>
		/// <param name="size">해쉬 길이를 지정합니다.</param>
		/// <param name="iteration">해쉬 적용횟수를 지정합니다.</param>
		/// <param name="algorithm">적용할 해쉬함수를 지정합니다.</param>
		/// <returns>암호화된 비밀번호를 반환합니다.</returns>
		public static string Hash(string password, Key salt, int size = HASH_SIZE, int iteration = ITERATION, HashAlgorithm algorithm = HashAlgorithm.SHA256)
		{
			Rfc2898DeriveBytes pbkdf2 = algorithm switch
			{
				HashAlgorithm.MD5 => new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password), salt, ITERATION, HashAlgorithmName.MD5),
				HashAlgorithm.SHA1 => new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password), salt, ITERATION, HashAlgorithmName.SHA1),
				HashAlgorithm.SHA256 => new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password), salt, ITERATION, HashAlgorithmName.SHA256),
				HashAlgorithm.SHA384 => new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password), salt, ITERATION, HashAlgorithmName.SHA384),
				HashAlgorithm.SHA512 => new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password), salt, ITERATION, HashAlgorithmName.SHA512),
				_ => new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password), salt, iteration, HashAlgorithmName.SHA256)
			};
			return Base64.Encode(pbkdf2.GetBytes(size));
		}
	}
}