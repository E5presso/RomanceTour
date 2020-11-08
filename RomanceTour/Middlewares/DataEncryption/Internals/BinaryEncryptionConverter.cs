using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace RomanceTour.Middlewares.DataEncryption.Internals
{
	internal sealed class BinaryEncryptionConverter : ValueConverter<byte[], byte[]>
	{
		public BinaryEncryptionConverter(IDataEncryptionProvider provider, ConverterMappingHints hints = default) : 
			base(x => provider.Encrypt(x), x => provider.Decrypt(x), hints) { }
	}
}