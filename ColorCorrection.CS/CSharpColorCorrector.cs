﻿// JEZYKI ASEMBLEROWE - PROJEKT
// Data: 20.01.2022, wersja: 1.0
// Autor: Jerzy Balcer, Informatyka Katowice, rok 3 sem. 5, gr. 1
// Temat: Korekcja kolorow bitmapy przez balans kanalow RGB
// Opis: Algorytm mnozy oryginalne wartoci RGB przez wspolczynnik bedacy procentem oryginalnej wartosci zmieniajac jednoczesnie kolor piksela

namespace ColorCorrection.CS
{
    public static class CSharpColorCorrector
    {
        public static byte[] Correct(byte[] channels, float red, float green, float blue)
        {
            // prepare an output array for corrected RGB values
            byte[] newChannels = new byte[channels.Length];

            // go through all input pixels
            for (int i = 3; i < channels.Length; i += 4)
            {
                // calculate new values (percent of original * original value)
                float newBlue = blue * (float)channels[i - 3];
                float newGreen = green * (float)channels[i - 2];
                float newRed = red * (float)channels[i-1];
                float newBlue2 = blue * (float)channels[i];

                // clamp values to fit in byte
                if (newBlue > 255) newBlue = 255;

                if (newRed > 255) newRed = 255;

                if (newGreen > 255) newGreen = 255;

                if (newBlue2 > 255) newBlue2 = 255;

                // save corrected RGB values to output array
                newChannels[i] = (byte)newBlue2;
                newChannels[i - 1] = (byte)newRed;
                newChannels[i - 2] = (byte)newGreen;
                newChannels[i - 3] = (byte)newBlue;
            }

            return newChannels;
        }
    }
}
