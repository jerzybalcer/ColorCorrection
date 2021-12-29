using ColorCorrection.CS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
            // load image from file in a way it can be overwritten
            // without using block the file would be locked until the disposal of the bitmap object
            using (var memoryStream = new MemoryStream(File.ReadAllBytes(path)))
                _originalBitmap = new Bitmap(memoryStream);

            CopyPixelsToByteArray();
        }

        private void CopyPixelsToByteArray()
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

            // save extracted pixels
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

        public Stopwatch CorrectColors(float red, float green, float blue, bool isAssemblyChosen, int numThreads)
        {
            // prepare array for corrected image pixels
            byte[] correctedPixels = new byte[_pixels.Length];

            // setup a stopwatch for measuring execution time
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            ThreadPool.SetMaxThreads(numThreads, numThreads);
            ThreadPool.SetMinThreads(numThreads, numThreads);

            var taskList = new List<Task<byte[]>>();

            for (int i = 2; i < _pixels.Length; i += 3)
            {
                byte[] portion = new byte[]{ _pixels[i-2], _pixels[i-1], _pixels[i] };

                Task<byte[]> task = null;

                if (isAssemblyChosen)
                {
                    task = Task.Run(() => RunAsmAlgorithm(portion, red, green, blue));
                }
                else
                {
                    task = Task.Run(() => RunCSharpAlgorithm(portion, red, green, blue));
                }

                taskList.Add(task);
            }

            Task.WaitAll(taskList.ToArray());

            // copy result of all tasks to output array
            for (int i = 2; i < _pixels.Length; i += 3)
            {
                var resultArray = taskList[(i - 2) / 3];
                correctedPixels[i - 2] = resultArray.Result[0]; 
                correctedPixels[i - 1] = resultArray.Result[1]; 
                correctedPixels[i] = resultArray.Result[2]; 
            }

            stopwatch.Stop();

            // copy corrected pixels back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(correctedPixels, 0, firstPixelPtr, correctedPixels.Length);

            return stopwatch;
        }

        private byte[] RunCSharpAlgorithm(byte[] portion, float red, float green, float blue)
        {
            return CSharpColorCorrector.Correct(portion, red, green, blue);
        }
        private byte[] RunAsmAlgorithm(byte[] portion, float red, float green, float blue)
        {
            return new byte[1]; // to do
        }

        public void SaveToFile(string format)
        {
            if (format == "Jpeg")
                _originalBitmap.Save("output." + format, ImageFormat.Jpeg);
            else if (format == "Png")
                _originalBitmap.Save("output." + format, ImageFormat.Png);
            else if (format == "Bmp")
                _originalBitmap.Save("output." + format, ImageFormat.Bmp);
        }
    }
}
