using Core.Security;

using Microsoft.EntityFrameworkCore.DataEncryption;

namespace RomanceTour.Middlewares
{
	public class EntityEncryptor : IEncryptionProvider
	{
		private readonly Aes encryptor;
		public string EncryptionKey { get; set; }
		public string DecryptionKey { get; set; }

		public EntityEncryptor()
		{
			encryptor = new Aes();
		}
		public EntityEncryptor(string encryptionKey, string decryptionKey)
		{
			EncryptionKey = encryptionKey;
			DecryptionKey = decryptionKey;
			encryptor = new Aes();
		}

		public string Encrypt(string plain)
		{
			if (EncryptionKey != string.Empty)
			{
				encryptor.SetKey(EncryptionKey);
				encryptor.Encrypt(plain, out string cipher);
				return cipher;
			}
			else return plain;
		}
		public string Decrypt(string cipher)
		{
			if (DecryptionKey != string.Empty)
			{
				encryptor.SetKey(DecryptionKey);
				encryptor.Decrypt(cipher, out string plain);
				return plain;
			}
			else return cipher;
		}
	}
}