namespace ColorCorrection.CS
{
    public static class CSharpColorCorrector
    {
        public static byte[] Correct(byte[] pixels, float red, float green, float blue)
        {
            byte[] newPixels = new byte[pixels.Length];

            for (int i = 2; i < pixels.Length; i += 3)
            {
                float newRed = 255.0f / (255.0f - red) * (float)pixels[i];
                float newGreen = 255.0f / (255.0f - green) * (float)pixels[i - 1];
                float newBlue = 255.0f / (255.0f - blue) * (float)pixels[i - 2];

                if (float.IsInfinity(newRed)) newRed = pixels[i];
                else if (newRed > 255) newRed = 255;
                else if (newRed < 0) newRed = 0;

                if (float.IsInfinity(newGreen)) newGreen = pixels[i - 1];
                else if (newGreen > 255) newGreen = 255;
                else if (newGreen < 0) newGreen = 0;

                if (float.IsInfinity(newBlue)) newBlue = pixels[i - 2];
                else if (newBlue > 255) newBlue = 255;
                else if (newBlue < 0) newBlue = 0;

                newPixels[i] = (byte)newRed;
                newPixels[i - 1] = (byte)newGreen;
                newPixels[i - 2] = (byte)newBlue;
            }

            return newPixels;
        }
    }
}
