using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace RomanceTour.Middlewares.DataEncryption.Internals
{
	internal sealed class IndexedStringEncryptionConverter : ValueConverter<string, string>
	{
		public IndexedStringEncryptionConverter(IDataEncryptionProvider provider, ConverterMappingHints hints = default) : 
			base(x => provider.Encrypt(x), x => provider.Decrypt(x), hints) { }
	}
}