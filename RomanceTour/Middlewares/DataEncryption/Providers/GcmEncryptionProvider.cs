using Core.Security;

namespace RomanceTour.Middlewares.DataEncryption.Providers
{
	public class GcmEncryptionProvider : IDataEncryptionProvider
	{
		private readonly Gcm gcm;

		public GcmEncryptionProvider(Gcm gcm)
		{
			this.gcm = gcm;
		}

		public string Encrypt(string plain)
		{
			var key = XmlConfiguration.SecretKey;
			if (key != null)
			{
				gcm.SetKey((Key)key);
				var result = gcm.Encrypt(plain, out var cipher);
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
				gcm.SetKey((Key)key);
				var result = gcm.Decrypt(cipher, out var plain);
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

				gcm.SetKey((Key)key);
				var result = gcm.Encrypt(plain, out var cipher);
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
				gcm.SetKey((Key)key);
				var result = gcm.Decrypt(cipher, out var plain);
				if (result) return plain;
				else return cipher;
			}
			else return cipher;
		}
	}
}