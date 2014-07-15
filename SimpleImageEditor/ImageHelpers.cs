using System;
using System.Drawing;

namespace SimpleImageEditor
{
    static class ImageHelpers
    {
        public static float GetDistance(Point p1, Point p2)
        {
            double a = p1.X - p2.X;
            double b = p1.Y - p2.Y;
            return (float)Math.Sqrt(a * a + b * b);
        }


        public static Rectangle RectangleFromCoords(Point p1, Point p2)
        {
            var xMin = Math.Min(p1.X, p2.X);
            var xMax = Math.Max(p1.X, p2.X);
            var yMin = Math.Min(p1.Y, p2.Y);
            var yMax = Math.Max(p1.Y, p2.Y);
            return new Rectangle(xMin, yMin, xMax - xMin, yMax - yMin);
        }

        /// <summary>
        /// Инвертирует цвет
        /// </summary>
        /// <param name="input">Исходный цвет</param>
        /// <returns>Инвертированный цвет</returns>
        public static Color InvertPixel(Color input)
        {
            // ну или (byte)~current.R
            var r = 255 - input.R;
            var g = 255 - input.G;
            var b = 255 - input.B;
            return Color.FromArgb(r, g, b);
        }

    }
}