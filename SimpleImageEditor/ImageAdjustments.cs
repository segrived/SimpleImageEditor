using System;
using System.Drawing;
using System.Threading.Tasks;

namespace SimpleImageEditor
{
    public static class ImageAdjustments
    {
        public static Task<Bitmap> Invert(Bitmap input, IProgress<int> progress)
        {
            return Task.Run(() => {
                var pixels = input.Height * input.Width;
                var height = input.Height;
                for (int i = 0; i < input.Width; i++) {
                    for (int j = 0; j < input.Height; j++) {
                        var current = input.GetPixel(i, j);
                        input.SetPixel(i, j, ImageHelpers.InvertPixel(current));
                        var p = (i * height + j) / (float)pixels * 100;
                        progress.Report((int)p);
                    }
                }
                return input;
            });
        }
    }
}
