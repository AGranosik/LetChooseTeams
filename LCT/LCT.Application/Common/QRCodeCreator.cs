using QRCoder;

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
            var qrCode = new Base64QRCode(QrCodeInfo);
            return qrCode.GetGraphic(20);
        }
    }
}
