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
        public static QRCodeGenerator _qrGenerator = new QRCodeGenerator();
        public string Generate(string url)
        {
            if(string.IsNullOrEmpty(url))
                throw new ArgumentNullException("qrcode url");

            QRCodeData QrCodeInfo = _qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            var QrCode = new QRCode(QrCodeInfo);
            Bitmap QrBitmap = QrCode.GetGraphic(60);
            byte[] BitmapArray = QrBitmap.BitmapToByteArray();
            return Convert.ToBase64String(BitmapArray);
        }
    }
}
