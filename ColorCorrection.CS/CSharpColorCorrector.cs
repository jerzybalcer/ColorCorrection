namespace ColorCorrection.CS
{
    public static class CSharpColorCorrector
    {
        public static byte[] Correct(byte[] channels, float red, float green, float blue)
        {
            // prepare an output array for corrected RGB values
            byte[] newChannels = new byte[channels.Length];

            // go through all input pixels
            for (int i = 2; i < channels.Length; i += 3)
            {
                // bug: when color parameter is 255 the new pixel value is not changed (divide by 0 -> infinity)
                // calculate corrected RGB values
                float newRed = 255.0f / (255.0f - red) * (float)channels[i];
                float newGreen = 255.0f / (255.0f - green) * (float)channels[i - 1];
                float newBlue = 255.0f / (255.0f - blue) * (float)channels[i - 2];

                // clamp result values to 0-255 range and handle infinities
                if (float.IsInfinity(newRed)) newRed = channels[i];
                else if (newRed > 255) newRed = 255;
                else if (newRed < 0) newRed = 0;

                if (float.IsInfinity(newGreen)) newGreen = channels[i - 1];
                else if (newGreen > 255) newGreen = 255;
                else if (newGreen < 0) newGreen = 0;

                if (float.IsInfinity(newBlue)) newBlue = channels[i - 2];
                else if (newBlue > 255) newBlue = 255;
                else if (newBlue < 0) newBlue = 0;

                // save corrected RGB values to output array
                newChannels[i] = (byte)newRed;
                newChannels[i - 1] = (byte)newGreen;
                newChannels[i - 2] = (byte)newBlue;
            }

            return newChannels;
        }
    }
}
