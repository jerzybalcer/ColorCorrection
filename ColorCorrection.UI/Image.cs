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
        private byte[] _channels;
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
            int channelsSize = bmpData.Stride * _originalBitmap.Height;
            byte[] channels = new byte[channelsSize];

            // copy rgb values to byte array
            System.Runtime.InteropServices.Marshal.Copy(firstPixelPtr, channels, 0, channelsSize);

            // unlock bits
            _originalBitmap.UnlockBits(bmpData);

            // save extracted pixels
            _channels = channels;
        }

        public bool IsBitmapLoaded()
        {
            if (_originalBitmap == null)
            {
                return false;
            }
            else
            {
                return true;
            }
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
            byte[] correctedChannels = new byte[_channels.Length];

            // limit number of usable threads
            ThreadPool.SetMaxThreads(numThreads, numThreads);
            ThreadPool.SetMinThreads(numThreads, numThreads);

            // prepare array for storing task for each pixel
            var taskList = new List<Task<byte[]>>();

            // setup a stopwatch for measuring execution time
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // go through all pixels
            for (int i = 11; i < _channels.Length; i += 12)
            {
                byte[] portion1, portion2, portion3;

                portion1 = new byte[] { _channels[i - 11], _channels[i - 10], _channels[i - 9], _channels[i - 8] }; // BGRB
                portion2 = new byte[] { _channels[i - 7], _channels[i - 6], _channels[i - 5], _channels[i - 4] }; // GRBG
                portion3 = new byte[] { _channels[i - 3], _channels[i - 2], _channels[i - 1], _channels[i] }; // RBGR

                Task<byte[]> task1 = null, task2 = null, task3 = null;

                // run correction algorithm on pixel
                if (isAssemblyChosen)
                {
                    task1 = Task.Run(() => Algorithms.RunAsmAlgorithm(portion1, red, green, blue));
                    task2 = Task.Run(() => Algorithms.RunAsmAlgorithm(portion2, blue, red, green));
                    task3 = Task.Run(() => Algorithms.RunAsmAlgorithm(portion3, green, blue, red));
                }
                else
                {
                    task1 = Task.Run(() => Algorithms.RunCSharpAlgorithm(portion1, red, green, blue));
                    task2 = Task.Run(() => Algorithms.RunCSharpAlgorithm(portion2, blue, red, green));
                    task3 = Task.Run(() => Algorithms.RunCSharpAlgorithm(portion3, green, blue, red));
                }

                // store that pixel's task
                taskList.Add(task1);
                taskList.Add(task2);
                taskList.Add(task3);
            }

            // wait for all tasks to finish
            Task.WaitAll(taskList.ToArray());

            // stop measuring time
            stopwatch.Stop();

            // copy result of all tasks to output array
            for (int i = 3; i < _channels.Length; i += 4)
            {
                var resultArray = taskList[(i - 3) / 4];

                for (int j = 0; j < resultArray.Result.Length; j++)
                {
                    correctedChannels[i - j] = resultArray.Result[3 - j];
                }
            }

            // copy corrected pixels back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(correctedChannels, 0, firstPixelPtr, correctedChannels.Length);

            return stopwatch;
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
