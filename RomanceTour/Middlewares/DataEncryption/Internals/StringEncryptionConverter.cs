using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace RomanceTour.Middlewares.DataEncryption.Internals
{
	internal sealed class StringEncryptionConverter : ValueConverter<string, string>
	{
		public StringEncryptionConverter(IDataEncryptionProvider provider, ConverterMappingHints hints = default) : 
			base(x => provider.Encrypt(x), x => provider.Decrypt(x), hints) { }
	}
}