using System;
using System.Collections.Generic;
using System.IO;
using PdfSharp.Fonts;

namespace NfseNacional.PdfGenerator.Lib.Pdf
{
    /// <summary>
    /// Resolvedor de fontes nativo do Windows para PDFsharp 6.x.
    /// Garante que o PDFsharp localize e utilize as fontes instaladas (ex: Segoe UI, Arial) no .NET Standard/.NET 8.
    /// </summary>
    public class WindowsFontResolver : IFontResolver
    {
        private static readonly string FontsDir = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
        private static readonly Dictionary<string, string> FontFilesCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        static WindowsFontResolver()
        {
            if (Directory.Exists(FontsDir))
            {
                foreach (var file in Directory.GetFiles(FontsDir))
                {
                    string ext = Path.GetExtension(file);
                    if (string.Equals(ext, ".ttf", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(ext, ".otf", StringComparison.OrdinalIgnoreCase))
                    {
                        string name = Path.GetFileNameWithoutExtension(file);
                        FontFilesCache[name] = file;
                    }
                }
            }
        }

        public byte[] GetFont(string faceName)
        {
            if (FontFilesCache.TryGetValue(faceName, out string filePath) && File.Exists(filePath))
            {
                return File.ReadAllBytes(filePath);
            }

            // Fallback para arial.ttf se não encontrar a fonte específica
            if (FontFilesCache.TryGetValue("arial", out string arialPath) && File.Exists(arialPath))
            {
                return File.ReadAllBytes(arialPath);
            }

            return null;
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            string cleanFamily = familyName.ToLowerInvariant().Replace(" ", "");
            string faceName = cleanFamily;

            if (isBold && isItalic)
            {
                if (FontFilesCache.ContainsKey(cleanFamily + "bi")) faceName = cleanFamily + "bi";
                else if (FontFilesCache.ContainsKey(cleanFamily + "z")) faceName = cleanFamily + "z";
                else if (FontFilesCache.ContainsKey(cleanFamily + "-bolditalic")) faceName = cleanFamily + "-bolditalic";
            }
            else if (isBold)
            {
                if (FontFilesCache.ContainsKey(cleanFamily + "b")) faceName = cleanFamily + "b";
                else if (FontFilesCache.ContainsKey(cleanFamily + "bd")) faceName = cleanFamily + "bd";
                else if (FontFilesCache.ContainsKey(cleanFamily + "-bold")) faceName = cleanFamily + "-bold";
                else if (FontFilesCache.ContainsKey("segoeuib")) faceName = "segoeuib";
            }
            else if (isItalic)
            {
                if (FontFilesCache.ContainsKey(cleanFamily + "i")) faceName = cleanFamily + "i";
                else if (FontFilesCache.ContainsKey(cleanFamily + "-italic")) faceName = cleanFamily + "-italic";
            }

            if (!FontFilesCache.ContainsKey(faceName))
            {
                if (FontFilesCache.ContainsKey(cleanFamily)) faceName = cleanFamily;
                else if (FontFilesCache.ContainsKey("segoeui")) faceName = "segoeui";
                else faceName = "arial";
            }

            return new FontResolverInfo(faceName);
        }

        public static void EnsureInitialized()
        {
            if (GlobalFontSettings.FontResolver == null)
            {
                GlobalFontSettings.FontResolver = new WindowsFontResolver();
            }
        }
    }
}
