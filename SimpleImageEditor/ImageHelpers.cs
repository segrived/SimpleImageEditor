using System;
using System.Drawing;

namespace SimpleImageEditor
{
    static class ImageHelpers
    {
        public static Rectangle RectangleFromCoords(Point p1, Point p2)
        {
            var xMin = Math.Min(p1.X, p2.X);
            var xMax = Math.Max(p1.X, p2.X);
            var yMin = Math.Min(p1.Y, p2.Y);
            var yMax = Math.Max(p1.Y, p2.Y);
            return new Rectangle(xMin, yMin, xMax - xMin, yMax - yMin);
        }

        public static Bitmap InvertBitmap(Bitmap input)
        {
            var newImage = new Bitmap(input.Width, input.Height);
            for (int i = 0; i < input.Width; i++) {
                for (int j = 0; j < input.Height; j++) {
                    var current = input.GetPixel(i, j);
                    // ну или (byte)~current.R
                    var r = 255 - current.R;
                    var g = 255 - current.G;
                    var b = 255 - current.B;
                    newImage.SetPixel(i, j, Color.FromArgb(r, g, b));
                }
            }
            return newImage;
        }
    }
}