using Core.Security;

namespace RomanceTour.Middlewares.DataEncryption.Providers
{
	public class RsaEncryptionProvider : IDataEncryptionProvider
	{
		private readonly Rsa aes;

		public RsaEncryptionProvider(Rsa aes)
		{
			this.aes = aes;
		}

		public string Encrypt(string plain)
		{
			var result = aes.Encrypt(plain, out var cipher);
			if (result) return cipher;
			else return plain;
		}
		public string Decrypt(string cipher)
		{
			var result = aes.Decrypt(cipher, out var plain);
			if (result) return plain;
			else return cipher;
		}
		public byte[] Encrypt(byte[] plain)
		{
			var result = aes.Encrypt(plain, out var cipher);
			if (result) return cipher;
			else return plain;
		}
		public byte[] Decrypt(byte[] cipher)
		{
			var result = aes.Decrypt(cipher, out var plain);
			if (result) return plain;
			else return cipher;
		}
	}
}