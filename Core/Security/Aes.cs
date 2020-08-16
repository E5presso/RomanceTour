using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Core.Security
{
	/// <summary>
	/// AES 암호화 기능을 제공하는 클래스입니다.
	/// </summary>
	public class Aes : IDisposable
	{
		/// <summary>
		/// 대칭키를 가져옵니다.
		/// </summary>
		public byte[] Key { get; private set; }
		/// <summary>
		/// 초기벡터를 가져옵니다.
		/// </summary>
		public byte[] IV { get; private set; }

		/// <summary>
		/// AES 클래스를 초기화합니다.
		/// </summary>
		public Aes()
		{
			Key = Security.Key.GenerateBytes(32);
			IV = new byte[16];
			Buffer.BlockCopy(Hash.SHA256(Key), 0, IV, 0, 16);
		}
		/// <summary>
		/// AES 클래스를 초기화합니다.
		/// </summary>
		/// <param name="key">암호화에 사용할 키를 지정합니다.</param>
		public Aes(byte[] key)
		{
			if (key.Length > 32) throw new ArgumentException("키의 길이가 32바이트를 넘을 수 없습니다.");
			Key = key;
			IV = new byte[16];
			Buffer.BlockCopy(Hash.SHA256(Key), 0, IV, 0, 16);
		}
		/// <summary>
		/// AES 클래스를 초기화합니다.
		/// </summary>
		/// <param name="key">암호화에 사용할 키를 지정합니다.</param>
		public Aes(string key)
		{
			byte[] _key = Encoding.UTF8.GetBytes(key);
			if (_key.Length > 32) throw new ArgumentException("키의 길이가 32바이트를 넘을 수 없습니다.");
			Key = _key;
			IV = new byte[16];
			Buffer.BlockCopy(Hash.SHA256(Key), 0, IV, 0, 16);
		}

		/// <summary>
		/// 새로운 키를 지정합니다.
		/// </summary>
		/// <param name="key">지정할 키입니다.</param>
		public void SetKey(byte[] key)
		{
			if (key.Length > 32) throw new ArgumentException("키의 길이가 32바이트를 넘을 수 없습니다.");
			Key = key;
			Buffer.BlockCopy(Hash.SHA256(Key), 0, IV, 0, 16);
		}
		/// <summary>
		/// 새로운 키를 지정합니다.
		/// </summary>
		/// <param name="key">지정할 키입니다.</param>
		public void SetKey(string key)
		{
			byte[] array = Encoding.UTF8.GetBytes(key);
			if (array.Length > 32) throw new ArgumentException("키의 길이가 32바이트를 넘을 수 없습니다.");
			Key = array;
			Buffer.BlockCopy(Hash.SHA256(Key), 0, IV, 0, 16);
		}

		/// <summary>
		/// 데이터를 암호화합니다.
		/// </summary>
		/// <param name="data">암호화할 데이터입니다.</param>
		/// <param name="encrypted">암호화된 데이터입니다.</param>
		/// <returns>암호화 결과입니다.</returns>
		public bool Encrypt(byte[] data, out byte[] encrypted)
		{
			try
			{
				using var aes = new RijndaelManaged
				{
					KeySize = 256,
					BlockSize = 128,
					Mode = CipherMode.CBC,
					Padding = PaddingMode.PKCS7,
					Key = Key,
					IV = IV
				};
				using var memory = new MemoryStream();
				using (var crypto = new CryptoStream(memory, aes.CreateEncryptor(aes.Key, aes.IV), CryptoStreamMode.Write))
				{
					crypto.Write(data, 0, data.Length);
				}
				encrypted = memory.ToArray();
				return true;
			}
			catch
			{
				encrypted = default;
				return false;
			}
		}
		/// <summary>
		/// 데이터를 복호화합니다.
		/// </summary>
		/// <param name="data">복호화할 데이터입니다.</param>
		/// <param name="decrypted">복호화된 데이터입니다.</param>
		/// <returns>복호화 결과입니다.</returns>
		public bool Decrypt(byte[] data, out byte[] decrypted)
		{
			try
			{
				using var aes = new RijndaelManaged
				{
					KeySize = 256,
					BlockSize = 128,
					Mode = CipherMode.CBC,
					Padding = PaddingMode.PKCS7,
					Key = Key,
					IV = IV
				};
				using var memory = new MemoryStream();
				using (var crypto = new CryptoStream(memory, aes.CreateDecryptor(aes.Key, aes.IV), CryptoStreamMode.Write))
				{
					crypto.Write(data, 0, data.Length);
				}
				decrypted = memory.ToArray();
				return true;
			}
			catch
			{
				decrypted = default;
				return false;
			}
		}
		/// <summary>
		/// 데이터를 암호화합니다.
		/// </summary>
		/// <param name="data">암호화할 데이터입니다.</param>
		/// <param name="encrypted">암호화된 데이터입니다.</param>
		/// <returns>암호화 결과입니다.</returns>
		public bool Encrypt(string data, out string encrypted)
		{
			bool result = Encrypt(Encoding.UTF8.GetBytes(data), out byte[] cipher);
			encrypted = Base64.GetString(cipher);
			return result;
		}
		/// <summary>
		/// 데이터를 복호화합니다.
		/// </summary>
		/// <param name="data">복호화할 데이터입니다.</param>
		/// <param name="decrypted">복호화된 데이터입니다.</param>
		/// <returns>복호화 결과입니다.</returns>
		public bool Decrypt(string data, out string decrypted)
		{
			bool result = Decrypt(Base64.GetBytes(data), out byte[] plain);
			decrypted = Encoding.UTF8.GetString(plain);
			return result;
		}
		#region IDisposable Support
		private bool disposedValue = false;
		/// <summary>
		/// AES 클래스를 제거합니다.
		/// </summary>
		~Aes()
		{
			Dispose(false);
		}
		/// <summary>
		/// IDisposable 패턴을 구현합니다.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
				}
				Key = null;
				IV = null;
				disposedValue = true;
			}
		}
		/// <summary>
		/// 클래스를 제거하고 리소스를 반환합니다.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}