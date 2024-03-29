﻿// JEZYKI ASEMBLEROWE - PROJEKT
// Data: 20.01.2022, wersja: 1.0
// Autor: Jerzy Balcer, Informatyka Katowice, rok 3 sem. 5, gr. 1
// Temat: Korekcja kolorow bitmapy przez balans kanalow RGB
// Opis: Algorytm mnozy oryginalne wartoci RGB przez wspolczynnik bedacy procentem oryginalnej wartosci zmieniajac jednoczesnie kolor piksela

using ColorCorrection.CS;
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
            Correct(portion, new float[] { blue, green, red, blue });

            return portion;
        }

        //[DllImport(@"C:\Users\Jerzy\source\repos\ColorCorrection\x64\Debug\ColorCorrection.ASM.dll")]
        [DllImport(@"C:\Users\Jerzy\source\repos\ColorCorrection\x64\Release\ColorCorrection.ASM.dll")]
        //[DllImport(@"C:\Users\jurek\Source\Repos\ColorCorrection\x64\Debug\ColorCorrection.ASM.dll")]
        //private static extern void Correct(byte[] inputArray, float red, float green, float blue); // 3 params mask method
        private static extern void Correct(byte[] inputArray, float[] mask); // mask array method
    }
}
