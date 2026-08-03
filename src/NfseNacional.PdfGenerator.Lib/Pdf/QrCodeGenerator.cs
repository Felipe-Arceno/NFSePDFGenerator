using System;
using System.Collections;
using System.IO;
using PdfSharp.Drawing;
using QRCoder;

namespace NfseNacional.PdfGenerator.Lib.Pdf
{
    public static class QrCodeGenerator
    {
        public static void RenderQrCodeVector(XGraphics gfx, string chaveAcesso, float x, float y, float size)
        {
            if (string.IsNullOrWhiteSpace(chaveAcesso) || gfx == null)
                return;

            string url = DanfseLayout.QrCodeBaseUrl + chaveAcesso.Trim();

            using (var qrGenerator = new QRCodeGenerator())
            using (var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.M))
            {
                var matrix = qrCodeData.ModuleMatrix;
                if (matrix == null || matrix.Count == 0) return;

                int count = matrix.Count;
                float moduleSize = size / count;

                for (int r = 0; r < count; r++)
                {
                    var row = matrix[r];
                    for (int c = 0; c < row.Length; c++)
                    {
                        if (row[c])
                        {
                            gfx.DrawRectangle(XBrushes.Black, x + c * moduleSize, y + r * moduleSize, moduleSize + 0.1f, moduleSize + 0.1f);
                        }
                    }
                }
            }
        }

        public static byte[] GeneratePngBytes(string chaveAcesso)
        {
            if (string.IsNullOrWhiteSpace(chaveAcesso))
                return null;

            string url = DanfseLayout.QrCodeBaseUrl + chaveAcesso.Trim();

            using (var qrGenerator = new QRCodeGenerator())
            using (var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.M))
            using (var qrCode = new PngByteQRCode(qrCodeData))
            {
                return qrCode.GetGraphic(4);
            }
        }
    }
}
