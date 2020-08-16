using System.Drawing;
using QRCoder;

namespace RomanceTour.Middlewares
{
	public static class QRGenerator
	{
		private static readonly QRCodeGenerator generator = new QRCodeGenerator();
		public static Image GenerateImage(string data)
		{
			var code = generator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
			using var qrCode = new QRCode(code);
			using var image = qrCode.GetGraphic(20);
			return image;
		}
	}
}