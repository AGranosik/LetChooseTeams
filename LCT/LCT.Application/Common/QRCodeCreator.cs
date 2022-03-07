using QRCoder;
using System.Drawing;

namespace LCT.Application.Common
{
    internal static class QRCodeCreator
    {
        public static string Generate(string url)
        {
            QRCodeGenerator QrGenerator = new QRCodeGenerator();
            QRCodeData QrCodeInfo = QrGenerator.CreateQrCode("hehe", QRCodeGenerator.ECCLevel.Q);
            var QrCode = new QRCode(QrCodeInfo);
            Bitmap QrBitmap = QrCode.GetGraphic(60);
            byte[] BitmapArray = QrBitmap.BitmapToByteArray();
            return Convert.ToBase64String(BitmapArray);
        }
    }
}
