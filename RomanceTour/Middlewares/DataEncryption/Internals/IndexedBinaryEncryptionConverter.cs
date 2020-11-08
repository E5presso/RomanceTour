using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace RomanceTour.Middlewares.DataEncryption.Internals
{
	internal sealed class IndexedBinaryEncryptionConverter : ValueConverter<byte[], byte[]>
	{
		public IndexedBinaryEncryptionConverter(IDataEncryptionProvider provider, ConverterMappingHints hints = default) : 
			base(x => provider.Encrypt(x), x => provider.Decrypt(x), hints) { }
	}
}