using Rollercoin.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageRecognition.Test
{
    static class Program
    {
        static void Main()
        {
            Bitmap haystack = (Bitmap)Bitmap.FromFile("haystack.png");
            Bitmap needle = (Bitmap)Bitmap.FromFile("needle.png");
            List<Point> points = Bitmap_CV.GetSubPositions(haystack, needle);

            Stopwatch s = new Stopwatch();
            s.Start();
            for(int y = 0; y<haystack.Height; y++)
            {
                for(int x = 0; x<haystack.Width; x++)
                {
                    if(haystack.GetPixel(x, y) != needle.GetPixel(x, y))
                    {
                        Console.WriteLine("pixel mismatch");
                    }
                }
            }
            s.Stop();
            Console.WriteLine($"Time: {s.ElapsedMilliseconds}");
            Console.ReadLine();
        }
    }
}
