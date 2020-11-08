using System;

namespace RomanceTour.Middlewares.DataEncryption.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class IndexedAttribute : Attribute { }
}