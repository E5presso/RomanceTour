using RomanceTour.Middlewares.DataEncryption;
using RomanceTour.Middlewares.DataEncryption.Attributes;
using RomanceTour.Middlewares.DataEncryption.Internals;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

using System;
using System.Linq;

namespace RomanceTour.Extensions
{
	public static class DataEncryptionExtensions
	{
		public static void UseEncryption(this ModelBuilder builder, IDataEncryptionProvider provider)
		{
			if (provider != null)
			{
				foreach (IMutableEntityType entity in builder.Model.GetEntityTypes())
				{
					foreach (IMutableProperty property in entity.GetProperties())
					{
						if (!IsDiscriminator(property))
						{
							if (property.ClrType == typeof(string))
							{
								var converter = new StringEncryptionConverter(provider);
								object[] attributes = property.PropertyInfo.GetCustomAttributes(typeof(EncryptedAttribute), false);
								if (attributes.Any()) property.SetValueConverter(converter);
							}
							else if (property.ClrType == typeof(string))
							{
								var converter = new BinaryEncryptionConverter(provider);
								object[] attributes = property.PropertyInfo.GetCustomAttributes(typeof(EncryptedAttribute), false);
								if (attributes.Any()) property.SetValueConverter(converter);
							}
						}
					}
				}
			}
			else throw new ArgumentException("암호화 서비스 공급자는 'null' 일 수 없습니다.");
		}
		private static bool IsDiscriminator(IMutableProperty property)
		{
			return property.Name == "Discriminator" && property.PropertyInfo == null;
		}
	}
}