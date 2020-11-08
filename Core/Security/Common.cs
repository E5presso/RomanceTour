using System;

namespace Core.Security
{
	/// <summary>
	/// Security 네임스페이스에서 사용 가능한 서명 알고리즘의 종류를 정의합니다.
	/// </summary>
	public enum SignAlgorithm
	{
		/// <summary>
		/// HMAC256 알고리즘입니다.
		/// </summary>
		HS256,
		/// <summary>
		/// HMAC384 알고리즘입니다.
		/// </summary>
		HS384,
		/// <summary>
		/// HMAC512 알고리즘입니다.
		/// </summary>
		HS512,
		/// <summary>
		/// RSA_SHA256 알고리즘입니다.
		/// </summary>
		RS256,
		/// <summary>
		/// RSA_SHA384 알고리즘입니다.
		/// </summary>
		RS384,
		/// <summary>
		/// RSA_SHA512 알고리즘입니다.
		/// </summary>
		RS512
	}
	/// <summary>
	/// Security 네임스페이스에서 사용 가능한 해시 알고리즘의 종류를 정의합니다.
	/// </summary>
	public enum HashAlgorithm
	{
		/// <summary>
		/// MD5 알고리즘입니다.
		/// </summary>
		MD5,
		/// <summary>
		/// SHA1 알고리즘입니다.
		/// </summary>
		SHA1,
		/// <summary>
		/// SHA256 알고리즘입니다.
		/// </summary>
		SHA256,
		/// <summary>
		/// SHA384 알고리즘입니다.
		/// </summary>
		SHA384,
		/// <summary>
		/// SHA512 알고리즘입니다.
		/// </summary>
		SHA512
	}
	/// <summary>
	/// 키의 종류를 지정합니다.
	/// </summary>
	public enum KeyType
	{
		/// <summary>
		/// 대칭키입니다.
		/// </summary>
		SYMMETRIC,
		/// <summary>
		/// 비대칭키입니다.
		/// </summary>
		ASYMMETRIC
	}
	/// <summary>
	/// 암호화에 사용할 키에 대한 정보를 구현합니다.
	/// </summary>
	public struct Key
	{
		private readonly byte[] _key;

		/// <summary>
		/// 키의 종류를 가져옵니다.
		/// </summary>
		public KeyType KeyType { get; private set; }

		/// <summary>
		/// Key 구조체를 초기화합니다.
		/// </summary>
		/// <param name="key">사용할 키를 지정합니다.</param>
		public Key(byte[] key)
		{
			_key = (byte[])key.Clone();
			KeyType = KeyType.SYMMETRIC;
		}
		/// <summary>
		/// Key 구조체를 초기화합니다.
		/// </summary>
		/// <param name="key">사용할 키를 지정합니다.</param>
		public Key(string key)
		{
			_key = Base64.Decode(key);
			KeyType = KeyType.SYMMETRIC;
		}
		/// <summary>
		/// Key 구조체를 초기화합니다.
		/// </summary>
		/// <param name="key">사용할 키를 지정합니다.</param>
		public Key(X509 key)
		{
			_key = key.Serialized;
			KeyType = KeyType.ASYMMETRIC;
		}

		/// <summary>
		/// 키를 표현가능한 Base64 문자열로 변환합니다.
		/// </summary>
		/// <returns>Base64 문자열을 반환합니다.</returns>
		public override string ToString() => Base64.Encode(_key);

		/// <summary>
		/// 대칭키를 문자열로 명시적 변환합니다.
		/// </summary>
		/// <param name="key">변환할 키를 지정합니다.</param>
		/// <exception cref="InvalidCastException">지정한 키가 비대칭키인 경우에 발생합니다.</exception>
		public static implicit operator string(Key key)
		{
			if (key.KeyType == KeyType.SYMMETRIC) return Base64.Encode(key._key);
			else throw new InvalidCastException("비대칭키는 문자열로 변환할 수 없습니다.");
		}
		/// <summary>
		/// 대칭키를 바이트 시퀀스로 명시적 변환합니다.
		/// </summary>
		/// <param name="key">변환할 키를 지정합니다.</param>
		/// <exception cref="InvalidCastException">지정한 키가 비대칭키인 경우에 발생합니다.</exception>
		public static implicit operator byte[](Key key)
		{
			if (key.KeyType == KeyType.SYMMETRIC) return (byte[])key._key.Clone();
			else throw new InvalidCastException("비대칭키는 바이트 시퀀스로 변환할 수 없습니다.");
		}
		/// <summary>
		/// 비대칭키를 X509 인증서로 명시적 변환합니다.
		/// </summary>
		/// <param name="key">변환할 키를 지정합니다.</param>
		/// <exception cref="InvalidCastException">지정한 키가 대칭키인 경우에 발생합니다.</exception>
		public static implicit operator X509(Key key)
		{
			if (key.KeyType == KeyType.ASYMMETRIC) return new X509((byte[])key._key.Clone());
			else throw new InvalidCastException("대칭키는 X509 인증서로 변환할 수 없습니다.");
		}

		/// <summary>
		/// 문자열을 대칭키로 명시적 변환합니다.
		/// </summary>
		/// <param name="key">변환할 키를 지정합니다.</param>
		public static implicit operator Key(string key) => new Key(key);
		/// <summary>
		/// 바이트시퀀스를 대칭키로 명시적 변환합니다.
		/// </summary>
		/// <param name="key">변환할 키를 지정합니다.</param>
		public static implicit operator Key(byte[] key) => new Key(key);
		/// <summary>
		/// X509 인증서를 비대칭키로 명시적 변환합니다.
		/// </summary>
		/// <param name="key">변환할 키를 지정합니다.</param>
		public static implicit operator Key(X509 key) => new Key(key);
	}
}