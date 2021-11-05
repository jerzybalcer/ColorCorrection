using ColorCorrection.CS;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ColorCorrection.UI
{
    public class Image
    {
        private Bitmap _originalBitmap;
        private byte[] _pixels;
        IntPtr firstPixelPtr;

        public void Load(string path)
        {
            _originalBitmap = new Bitmap(path); // load bitmap from file
            ToPixelsByteArray();
        }

        private void ToPixelsByteArray()
        {
            // lock bitmap's bits  
            BitmapData bmpData =
                _originalBitmap.LockBits(
                    new Rectangle(0, 0, _originalBitmap.Width, _originalBitmap.Height),
                    ImageLockMode.ReadWrite,
                    _originalBitmap.PixelFormat);

            // get first pixel's pointer
            firstPixelPtr = bmpData.Scan0;

            // create an array for storing all of the image bytes
            int pixelsSize = bmpData.Stride * _originalBitmap.Height;
            byte[] pixels = new byte[pixelsSize];

            // copy rgb values to byte array
            System.Runtime.InteropServices.Marshal.Copy(firstPixelPtr, pixels, 0, pixelsSize);

            // unlock bits
            _originalBitmap.UnlockBits(bmpData);

            _pixels = pixels;
        }

        public BitmapSource ToBitmapSource()
        {
            return Imaging.CreateBitmapSourceFromHBitmap(
                           _originalBitmap.GetHbitmap(),
                           IntPtr.Zero,
                           Int32Rect.Empty,
                           BitmapSizeOptions.FromEmptyOptions());
        }

        public Stopwatch CorrectColors(float red, float green, float blue, bool isAssemblyChosen)
        {
            // convert loaded bitmap to byte array of all pixels
            byte[] correctedPixels = new byte[_pixels.Length];

            // setup a stopwatch for measuring execution time
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // correct the colors 
            if (isAssemblyChosen)
            {
                // asm corrector to do
            }
            else
            {
                correctedPixels = CSharpColorCorrector.Correct(_pixels, red, green, blue);
            }

            stopwatch.Stop();

            // copy corrected pixels back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(correctedPixels, 0, firstPixelPtr, correctedPixels.Length);

            return stopwatch;
        }

        public void SaveToFile(string format)
        {
            if(format == "Jpeg")
                _originalBitmap.Save("output." + format, ImageFormat.Jpeg);
            else if(format == "Png")
                _originalBitmap.Save("output." + format, ImageFormat.Png);
            else if(format == "Bmp")
                _originalBitmap.Save("output." + format, ImageFormat.Bmp);
        }
    }
}
