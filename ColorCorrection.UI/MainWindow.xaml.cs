using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ColorCorrection.UI
{
    public partial class MainWindow : Window
    {
        private Image imgToCorrect;

        public MainWindow()
        {
            InitializeComponent();
            imgToCorrect = new Image();
            ThreadsSlider.Value = Environment.ProcessorCount;
        }

        private void BrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            // create a suitable dialog to choose the file
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Images (*.jpeg, *.jpg, *.bmp)|*.jpeg; *.jpg; *.bmp";

            // when the file is chosen load it
            if (fileDialog.ShowDialog() == true)
                imgToCorrect.Load(fileDialog.FileName);

            // process the image only if there was a valid file chosen
            if (fileDialog.FileName.Length > 0)
            {
                DisplayedImg.Source = imgToCorrect.ToBitmapSource();
                Time.Text = "Correction time: not yet measured";
            }
        }

        private void RunBtn_Click(object sender, RoutedEventArgs e)
        {
            // stop if there's no image loaded
            if (imgToCorrect.IsBitmapLoaded() == false) return;

            // correct the colors then display the new image
            Stopwatch executionTime = imgToCorrect.CorrectColors(
                (float)RedSlider.Value, (float)GreenSlider.Value, (float)BlueSlider.Value, (bool)AsmBtn.IsChecked, (int)ThreadsSlider.Value);

            //display execution time
            Time.Text = "Correction time: " + executionTime.ElapsedMilliseconds + " ms | "+ executionTime.ElapsedTicks +" ticks";

            // display corrected image
            DisplayedImg.Source = imgToCorrect.ToBitmapSource();

            // determine output file format
            foreach(RadioButton btn in FormatSelect.Children)
            {
                if ((bool)btn.IsChecked)
                {
                    imgToCorrect.SaveToFile(btn.Name);
                    break;
                }
            }
        }
    }
}
