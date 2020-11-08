using Core.Collections;

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Core.Security
{
	/// <summary>
	/// Gcm 암호화 알고리즘을 구현한 클래스입니다.
	/// </summary>
	public class Gcm : IEncryptionProvider
	{
		private const int KEY_SIZE = 32;
		private const int TAG_SIZE = 12;
		private const int NONCE_SIZE = 12;

		/// <summary>
		/// 대칭키를 가져옵니다.
		/// </summary>
		public byte[] Key { get; private set; }

		/// <summary>
		/// Gcm 클래스를 초기화합니다.
		/// </summary>
		public Gcm()
		{
			Key = KeyGenerator.GenerateBytes(KEY_SIZE);
		}
		/// <summary>
		/// Gcm 클래스를 초기화합니다.
		/// </summary>
		/// <param name="key">암호화에 사용할 키를 지정합니다.</param>
		public Gcm(byte[] key)
		{
			byte[] _key = key;
			if (_key.Length > KEY_SIZE) throw new ArgumentException($"키의 길이가 {KEY_SIZE}바이트를 넘을 수 없습니다.");
			Key = _key;
		}
		/// <summary>
		/// Gcm 클래스를 초기화합니다.
		/// </summary>
		/// <param name="key">암호화에 사용할 키를 지정합니다.</param>
		public Gcm(string key)
		{
			byte[] _key = Base64.Decode(key);
			if (_key.Length > KEY_SIZE) throw new ArgumentException($"키의 길이가 {KEY_SIZE}바이트를 넘을 수 없습니다.");
			Key = _key;
		}
		/// <summary>
		/// Gcm 클래스를 초기화합니다.
		/// </summary>
		/// <param name="key">암호화에 사용할 키를 지정합니다.</param>
		public Gcm(Key key)
		{
			byte[] _key = key;
			if (_key.Length > KEY_SIZE) throw new ArgumentException($"키의 길이가 {KEY_SIZE}바이트를 넘을 수 없습니다.");
			Key = _key;
		}

		/// <summary>
		/// 새로운 키를 지정합니다.
		/// </summary>
		/// <param name="key">지정할 키입니다.</param>
		public void SetKey(byte[] key)
		{
			if (key.Length > KEY_SIZE) throw new ArgumentException($"키의 길이가 {KEY_SIZE}바이트를 넘을 수 없습니다.");
			Key = key;
		}
		/// <summary>
		/// 새로운 키를 지정합니다.
		/// </summary>
		/// <param name="key">지정할 키입니다.</param>
		public void SetKey(string key)
		{
			byte[] array = Encoding.UTF8.GetBytes(key);
			if (array.Length > KEY_SIZE) throw new ArgumentException($"키의 길이가 {KEY_SIZE}바이트를 넘을 수 없습니다.");
			Key = array;
		}
		/// <summary>
		/// 새로운 키를 지정합니다.
		/// </summary>
		/// <param name="key">지정할 키입니다.</param>
		public void SetKey(Key key)
		{
			byte[] array = key;
			if (array.Length > KEY_SIZE) throw new ArgumentException($"키의 길이가 {KEY_SIZE}바이트를 넘을 수 없습니다.");
			Key = array;
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
				var buffer = new RingBuffer();
				var tag = new byte[TAG_SIZE];
				var nonce = Hash.Compute(data, HashAlgorithm.SHA512).Take(NONCE_SIZE).ToArray();
				var cipher = new byte[data.Length];

				using var gcm = new AesGcm(Key);
				gcm.Encrypt(nonce, data, cipher, tag);
				buffer.Write(tag);
				buffer.Write(nonce);
				buffer.Write(cipher);

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
				RingBuffer buffer = new RingBuffer(data);
				byte[] tag = buffer.Read(TAG_SIZE);
				byte[] nonce = buffer.Read(NONCE_SIZE);
				byte[] cipher = buffer.Flush();
				byte[] plain = new byte[cipher.Length];

				using var gcm = new AesGcm(Key);
				gcm.Decrypt(nonce, cipher, tag, plain);
				decrypted = plain;
				return true;
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
	}
}