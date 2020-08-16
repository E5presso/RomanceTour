using System;
using System.Security.Cryptography;
using System.Text;

namespace Core.Security
{
	/// <summary>
	/// 해쉬 타입을 지정합니다.
	/// </summary>
	public enum HashType
	{
		/// <summary>
		/// SHA1 해쉬입니다.
		/// </summary>
		SHA1,
		/// <summary>
		/// SHA256 해쉬입니다.
		/// </summary>
		SHA256,
		/// <summary>
		/// SHA384 해쉬입니다.
		/// </summary>
		SHA384,
		/// <summary>
		/// SHA512 해쉬입니다.
		/// </summary>
		SHA512
	}
	/// <summary>
	/// RSA 암호화 기능을 제공하는 클래스입니다..
	/// </summary>
	public class Rsa : IDisposable
	{
		private string privateKey;

		/// <summary>
		/// RSA 공개키입니다.
		/// </summary>
		public string PublicKey { get; set; }
		/// <summary>
		/// RSA 개인키입니다.
		/// </summary>
		public string PrivateKey
		{
			get => privateKey;
			set
			{
				using var provider = new RSACryptoServiceProvider();
				privateKey = value;
				provider.FromXmlString(value);
				PublicKey = provider.ToXmlString(false);
			}
		}

		/// <summary>
		/// RSA 클래스를 초기화합니다.
		/// </summary>
		public Rsa()
		{
			using var provider = new RSACryptoServiceProvider();
			PrivateKey = provider.ToXmlString(true);
		}
		/// <summary>
		/// RSA 클래스를 초기화합니다.
		/// </summary>
		/// <param name="certificate">X.509 인증서를 지정합니다.</param>
		public Rsa(X509 certificate)
		{
			if (certificate.HasPrivateKey) PrivateKey = certificate.PrivateKey;
			else
			{
				using var provider = new RSACryptoServiceProvider();
				privateKey = provider.ToXmlString(true);
				PublicKey = certificate.PublicKey;
			}
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
				using var provider = new RSACryptoServiceProvider();
				provider.FromXmlString(PublicKey);
				encrypted = provider.Encrypt(data, true);
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
				using var provider = new RSACryptoServiceProvider();
				provider.FromXmlString(privateKey);
				decrypted = provider.Decrypt(data, true);
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

		/// <summary>
		/// 데이터에 서명합니다.
		/// </summary>
		/// <param name="data">서명할 데이터입니다.</param>
		/// <param name="type">서명에 사용할 해쉬함수를 지정합니다.</param>
		/// <returns>서명된 데이터입니다.</returns>
		public byte[] Sign(byte[] data, HashType type = HashType.SHA256)
		{
			try
			{
				using var provider = new RSACryptoServiceProvider();
				provider.FromXmlString(privateKey);
				return type switch
				{
					HashType.SHA1 => provider.SignData(data, new SHA1CryptoServiceProvider()),
					HashType.SHA256 => provider.SignData(data, new SHA256CryptoServiceProvider()),
					HashType.SHA384 => provider.SignData(data, new SHA384CryptoServiceProvider()),
					HashType.SHA512 => provider.SignData(data, new SHA512CryptoServiceProvider()),
					_ => provider.SignData(data, new SHA1CryptoServiceProvider())
				};
			}
			catch { throw; }
		}
		/// <summary>
		/// 서명된 데이터를 검증합니다.
		/// </summary>
		/// <param name="data">서명의 유효성을 검증할 데이터를 지정합니다.</param>
		/// <param name="sign">데이터의 서명값을 지정합니다.</param>
		/// <param name="type">서명에 사용한 해쉬 알고리즘을 지정합니다.</param>
		/// <returns>검증 결과입니다.</returns>
		public bool Verify(byte[] data, byte[] sign, HashType type = HashType.SHA256)
		{
			try
			{
				using var provider = new RSACryptoServiceProvider();
				provider.FromXmlString(PublicKey);
				return type switch
				{
					HashType.SHA1 => provider.VerifyData(data, new SHA1CryptoServiceProvider(), sign),
					HashType.SHA256 => provider.VerifyData(data, new SHA256CryptoServiceProvider(), sign),
					HashType.SHA384 => provider.VerifyData(data, new SHA384CryptoServiceProvider(), sign),
					HashType.SHA512 => provider.VerifyData(data, new SHA512CryptoServiceProvider(), sign),
					_ => provider.VerifyData(data, new SHA1CryptoServiceProvider(), sign)
				};
			}
			catch { throw; }
		}
		/// <summary>
		/// 데이터에 서명합니다.
		/// </summary>
		/// <param name="data">서명할 데이터입니다.</param>
		/// <param name="type">서명에 사용할 해쉬함수를 지정합니다.</param>
		/// <returns>서명된 데이터입니다.</returns>
		public string Sign(string data, HashType type = HashType.SHA256)
		{
			return Base64.GetString(Sign(Encoding.UTF8.GetBytes(data), type));
		}
		/// <summary>
		/// 서명된 데이터를 검증합니다.
		/// </summary>
		/// <param name="data">서명의 유효성을 검증할 데이터를 지정합니다.</param>
		/// <param name="sign">데이터의 서명값을 지정합니다.</param>
		/// <param name="type">서명에 사용한 해쉬 알고리즘을 지정합니다.</param>
		/// <returns>검증 결과입니다.</returns>
		public bool Verify(string data, string sign, HashType type = HashType.SHA256)
		{
			return Verify(Encoding.UTF8.GetBytes(data), Base64.GetBytes(sign), type);
		}
		#region IDisposable Support
		private bool disposedValue = false;
		/// <summary>
		/// RSA 클래스를 제거합니다.
		/// </summary>
		~Rsa()
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
				PublicKey = null;
				privateKey = null;
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