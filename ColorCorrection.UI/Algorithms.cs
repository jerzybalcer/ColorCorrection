using ColorCorrection.CS;
using System;
using System.Runtime.InteropServices;

namespace ColorCorrection.UI
{
    public static class Algorithms
    {
        public static byte[] RunCSharpAlgorithm(byte[] portion, float red, float green, float blue)
        {
            return CSharpColorCorrector.Correct(portion, red, green, blue);
        }
        public static byte[] RunAsmAlgorithm(byte[] portion, float red, float green, float blue)
        {
            IntPtr inputAddress = Marshal.AllocHGlobal(portion.Length);
            
            Marshal.Copy(portion, 0, inputAddress, portion.Length);

            IntPtr outputAddress = Marshal.AllocHGlobal(portion.Length);

            Correct(inputAddress, outputAddress, (int)red, (int)green, (int)blue);

            byte[] outputArray = new byte[portion.Length];

            Marshal.Copy(outputAddress, outputArray, 0, portion.Length);

            Marshal.FreeHGlobal(inputAddress);
            Marshal.FreeHGlobal(outputAddress);

            return outputArray;
        }

        //[DllImport(@"C:\Users\Jerzy\source\repos\ColorCorrection\x64\Debug\ColorCorrection.ASM.dll")]
        [DllImport(@"C:\Users\jurek\Source\Repos\ColorCorrection\x64\Debug\ColorCorrection.ASM.dll")]
        private static extern void Correct(IntPtr inputArrayAddress, IntPtr outputArrayAddress, int red, int green, int blue);
    }
}
