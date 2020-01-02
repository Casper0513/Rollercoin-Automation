using AForge.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Rollercoin.API
{
    public static class Bitmap_CV
    {
        public static Bitmap ConvertToFormat(this System.Drawing.Image image, PixelFormat format)
        {
            Bitmap copy = new Bitmap(image.Width, image.Height, format);
            using (Graphics gr = Graphics.FromImage(copy))
            {
                gr.DrawImage(image, new Rectangle(0, 0, copy.Width, copy.Height));
            }
            return copy;
        }

        public static TemplateMatch[] FindAllNeedles(Bitmap haystack, Bitmap needle, float matchPercentage)
        {
            if (haystack.PixelFormat != PixelFormat.Format24bppRgb)
                haystack = ConvertToFormat(haystack, PixelFormat.Format24bppRgb);
            if (needle.PixelFormat != PixelFormat.Format24bppRgb)
                needle = ConvertToFormat(needle, PixelFormat.Format24bppRgb);
            // create template matching algorithm's instance
            // (set similarity threshold to 90%)
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(matchPercentage);
            // find all matchings with specified above similarity
            TemplateMatch[] matchings = tm.ProcessImage(haystack, needle);
            // highlight found matchings
            BitmapData data = haystack.LockBits(
                new Rectangle(0, 0, haystack.Width, haystack.Height),
                ImageLockMode.ReadWrite, haystack.PixelFormat);
            haystack.UnlockBits(data);
            return matchings;
        }

        public static bool HaystackContainsNeedle(Bitmap haystack, Bitmap needle)
        {
            if (haystack.PixelFormat != PixelFormat.Format24bppRgb)
                haystack = ConvertToFormat(haystack, PixelFormat.Format24bppRgb);
            if (needle.PixelFormat != PixelFormat.Format24bppRgb)
                needle = ConvertToFormat(needle, PixelFormat.Format24bppRgb);
            // create template matching algorithm's instance
            // (set similarity threshold to 92.5%)
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0.85f);
            // find all matchings with specified above similarity
            TemplateMatch[] matchings = tm.ProcessImage(haystack, needle);
            // highlight found matchings
            BitmapData data = haystack.LockBits(
                new Rectangle(0, 0, haystack.Width, haystack.Height),
                ImageLockMode.ReadWrite, haystack.PixelFormat);
            if (matchings.Length > 0) return true;
            haystack.UnlockBits(data);
            return false;
        }
    }
}
