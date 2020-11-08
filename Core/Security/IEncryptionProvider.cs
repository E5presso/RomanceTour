using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Security
{
	interface IEncryptionProvider
	{
		bool Encrypt(string data, out string encrypted);
		bool Decrypt(string data, out string decrypted);
		bool Encrypt(byte[] data, out byte[] encrypted);
		bool Decrypt(byte[] data, out byte[] decrypted);
	}
}