using QRCoder;

namespace RJ_VC_Bypass;

/// <summary>
/// QR Code generation helper
/// </summary>
public static class QrHelper
{
    /// <summary>
    /// Generate QR code as Bitmap
    /// </summary>
    public static Bitmap GenerateQr(string text, int size = 300)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new QRCode(qrCodeData);

        var bitmap = qrCode.GetGraphic(size);
        return bitmap;
    }

    /// <summary>
    /// Generate QR code and save to file
    /// </summary>
    public static void SaveQrToFile(string text, string filePath, int size = 300)
    {
        using var bitmap = GenerateQr(text, size);
        bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
    }
}
