using Core.Collections;

using System;
using System.Security.Cryptography;
using System.Text;

namespace Core.Security
{
	/// <summary>
	/// RSA 암호화 기능을 제공하는 클래스입니다..
	/// </summary>
	public class Rsa : IEncryptionProvider
	{
		private string privateKey;

		/// <summary>
		/// 암호문에 포함될 RSA서명의 길이를 가져옵니다.
		/// </summary>
		public static int SIGNATURE_SIZE = 384;

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
		/// Rsa 클래스를 초기화합니다.
		/// </summary>
		public Rsa()
		{
			using var provider = new RSACryptoServiceProvider();
			PrivateKey = provider.ToXmlString(true);
		}
		/// <summary>
		/// Rsa 클래스를 초기화합니다.
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
		/// Rsa 클래스를 초기화합니다.
		/// </summary>
		/// <param name="key">키 데이터를 지정합니다.</param>
		public Rsa(Key key)
		{
			var cert = (X509)key;
			if (cert.HasPrivateKey) PrivateKey = cert.PrivateKey;
			else
			{
				using var provider = new RSACryptoServiceProvider();
				privateKey = provider.ToXmlString(true);
				PublicKey = cert.PublicKey;
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
				if (data.Length > 256) throw new ArgumentException("암호화하려는 데이터는 키 길이를 초과할 수 없습니다.");
				var buffer = new RingBuffer();
				using var provider = new RSACryptoServiceProvider();
				provider.FromXmlString(PublicKey);
				buffer.Write(Sign(data));
				buffer.Write(provider.Encrypt(data, true));
				encrypted = buffer.ToArray();
				return true;
			}
			catch
			{
				encrypted = data;
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
				var buffer = new RingBuffer(data);
				byte[] signature = buffer.Read(SIGNATURE_SIZE);
				byte[] cipher = buffer.Flush();
				using var provider = new RSACryptoServiceProvider();
				provider.FromXmlString(privateKey);
				decrypted = provider.Decrypt(cipher, true);
				if (Verify(decrypted, signature)) return true;
				else
				{
					decrypted = data;
					return false;
				}
			}
			catch
			{
				decrypted = data;
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
			try
			{
				bool result = Encrypt(Encoding.UTF8.GetBytes(data), out byte[] cipher);
				if (result) encrypted = Base64.Encode(cipher);
				else encrypted = data;
				return result;
			}
			catch
			{
				encrypted = data;
				return false;
			}
		}
		/// <summary>
		/// 데이터를 복호화합니다.
		/// </summary>
		/// <param name="data">복호화할 데이터입니다.</param>
		/// <param name="decrypted">복호화된 데이터입니다.</param>
		/// <returns>복호화 결과입니다.</returns>
		public bool Decrypt(string data, out string decrypted)
		{
			try
			{
				bool result = Decrypt(Base64.Decode(data), out byte[] plain);
				if (result) decrypted = Encoding.UTF8.GetString(plain);
				else decrypted = data;
				return result;
			}
			catch
			{
				decrypted = data;
				return false;
			}
		}

		/// <summary>
		/// 데이터에 서명합니다.
		/// </summary>
		/// <param name="data">서명할 데이터입니다.</param>
		/// <param name="algorithm">서명에 사용할 해쉬함수를 지정합니다.</param>
		/// <returns>서명된 데이터입니다.</returns>
		public byte[] Sign(byte[] data, SignAlgorithm algorithm = SignAlgorithm.RS256)
		{
			try
			{
				using var provider = new RSACryptoServiceProvider();
				provider.FromXmlString(privateKey);
				return algorithm switch
				{
					SignAlgorithm.RS256 => provider.SignData(data, new SHA256CryptoServiceProvider()),
					SignAlgorithm.RS384 => provider.SignData(data, new SHA384CryptoServiceProvider()),
					SignAlgorithm.RS512 => provider.SignData(data, new SHA512CryptoServiceProvider()),
					_ => throw new ArgumentException("유효하지 않은 알고리즘입니다."),
				};
			}
			catch { throw; }
		}
		/// <summary>
		/// 서명된 데이터를 검증합니다.
		/// </summary>
		/// <param name="data">서명의 유효성을 검증할 데이터를 지정합니다.</param>
		/// <param name="sign">데이터의 서명값을 지정합니다.</param>
		/// <param name="algorithm">서명에 사용한 해쉬 알고리즘을 지정합니다.</param>
		/// <returns>검증 결과입니다.</returns>
		public bool Verify(byte[] data, byte[] sign, SignAlgorithm algorithm = SignAlgorithm.RS256)
		{
			try
			{
				using var provider = new RSACryptoServiceProvider();
				provider.FromXmlString(PublicKey);
				return algorithm switch
				{
					SignAlgorithm.RS256 => provider.VerifyData(data, new SHA256CryptoServiceProvider(), sign),
					SignAlgorithm.RS384 => provider.VerifyData(data, new SHA384CryptoServiceProvider(), sign),
					SignAlgorithm.RS512 => provider.VerifyData(data, new SHA512CryptoServiceProvider(), sign),
					_ => throw new ArgumentException("유효하지 않은 알고리즘입니다."),
				};
			}
			catch { return false; }
		}
		/// <summary>
		/// 데이터에 서명합니다.
		/// </summary>
		/// <param name="data">서명할 데이터입니다.</param>
		/// <param name="algorithm">서명에 사용할 해쉬함수를 지정합니다.</param>
		/// <returns>서명된 데이터입니다.</returns>
		public string Sign(string data, SignAlgorithm algorithm = SignAlgorithm.RS256)
		{
			return Base64.Encode(Sign(Encoding.UTF8.GetBytes(data), algorithm));
		}
		/// <summary>
		/// 서명된 데이터를 검증합니다.
		/// </summary>
		/// <param name="data">서명의 유효성을 검증할 데이터를 지정합니다.</param>
		/// <param name="sign">데이터의 서명값을 지정합니다.</param>
		/// <param name="algorithm">서명에 사용한 해쉬 알고리즘을 지정합니다.</param>
		/// <returns>검증 결과입니다.</returns>
		public bool Verify(string data, string sign, SignAlgorithm algorithm = SignAlgorithm.RS256)
		{
			return Verify(Encoding.UTF8.GetBytes(data), Base64.Decode(sign), algorithm);
		}
	}
}