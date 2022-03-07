using QRCoder;
using System.Drawing;

namespace LCT.Application.Common
{
    public interface IQRCodeCreator
    {
        string Generate(string url);
    }
    public class QRCodeCreator: IQRCodeCreator
    {
        public string Generate(string url)
        {
            if(string.IsNullOrEmpty(url))
                throw new ArgumentNullException("qrcode url");

            QRCodeGenerator QrGenerator = new QRCodeGenerator();
            QRCodeData QrCodeInfo = QrGenerator.CreateQrCode("hehe", QRCodeGenerator.ECCLevel.Q);
            var QrCode = new QRCode(QrCodeInfo);
            Bitmap QrBitmap = QrCode.GetGraphic(60);
            byte[] BitmapArray = QrBitmap.BitmapToByteArray();
            return Convert.ToBase64String(BitmapArray);
        }
    }
}
