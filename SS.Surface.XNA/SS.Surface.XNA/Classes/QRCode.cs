using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using com.google.zxing.qrcode.decoder;

namespace SS.Surface.XNA.Classes
{
    public static class QRCode
    {
        private static Dictionary<string, Texture2D> _cache = new Dictionary<string, Texture2D>();

        public static Texture2D CreateQRCode(GraphicsDevice graphics, int width, int height, string toEncode)
        {
            if (_cache.ContainsKey(toEncode))
                return _cache[toEncode];

            const int magnify = 2;

            var code = new com.google.zxing.qrcode.encoder.QRCode();
            com.google.zxing.qrcode.encoder.Encoder.encode(toEncode, ErrorCorrectionLevel.M, code);

            sbyte[,] img = code.Matrix.Array;
            var bmp = new Bitmap(code.Matrix.Width * magnify, code.Matrix.Height * magnify);
            var g = Graphics.FromImage(bmp);
            g.Clear(Color.White);

            for (int i = 0; i <= img.Length - 1; i++)
            {
                var row = i / code.Matrix.Width;
                var col = i % code.Matrix.Width;
                if (img[row, col] == 1)
                {
                    g.FillRectangle(Brushes.Black, row * magnify, col * magnify, 1 * magnify, 1 * magnify);
                }
            }

            bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);

            Texture2D texture;
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Jpeg);
                ms.Position = 0;
                texture = Texture2D.FromStream(graphics, ms, width, height, false);
            }
            return texture;
        }
    }
}
