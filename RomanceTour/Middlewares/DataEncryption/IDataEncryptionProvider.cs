namespace RomanceTour.Middlewares.DataEncryption
{
	public interface IDataEncryptionProvider
	{
		string Encrypt(string plain);
		string Decrypt(string cipher);

		byte[] Encrypt(byte[] plain);
		byte[] Decrypt(byte[] cipher);
	}
}