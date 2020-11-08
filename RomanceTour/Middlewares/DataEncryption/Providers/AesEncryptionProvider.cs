using Core.Security;
using RomanceTour.Middlewares;
using RomanceTour.Middlewares.DataEncryption;

namespace ApiServer.Services.DataEncryption.Providers
{
	public class AesEncryptionProvider : IDataEncryptionProvider
	{
		private readonly Aes aes;

		public AesEncryptionProvider(Aes aes)
		{
			this.aes = aes;
		}

		public string Encrypt(string plain)
		{
			var key = XmlConfiguration.SecretKey;
			if (key != null)
			{
				aes.SetKey((Key)key);
				var result = aes.Encrypt(plain, out var cipher);
				if (result) return cipher;
				else return plain;
			}
			else return plain;
		}
		public string Decrypt(string cipher)
		{
			var key = XmlConfiguration.SecretKey;
			if (key != null)
			{
				aes.SetKey((Key)key);
				var result = aes.Decrypt(cipher, out var plain);
				if (result) return plain;
				else return cipher;
			}
			else return cipher;
		}
		public byte[] Encrypt(byte[] plain)
		{
			var key = XmlConfiguration.SecretKey;
			if (key != null)
			{
				aes.SetKey((Key)key);
				var result = aes.Encrypt(plain, out var cipher);
				if (result) return cipher;
				else return plain;
			}
			else return plain;
		}
		public byte[] Decrypt(byte[] cipher)
		{
			var key = XmlConfiguration.SecretKey;
			if (key != null)
			{
				aes.SetKey((Key)key);
				var result = aes.Decrypt(cipher, out var plain);
				if (result) return plain;
				else return cipher;
			}
			else return cipher;
		}
	}
}