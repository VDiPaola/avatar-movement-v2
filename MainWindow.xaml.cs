using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace avatar_movent_v2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
             

            //audio
            var waveIn = new WaveInEvent()
            {
                DeviceNumber = 0,
                WaveFormat = new WaveFormat(rate: 44100, bits: 16, channels: 1),
                BufferMilliseconds = 50
            };
            waveIn.DataAvailable += OnDataAvailable;
            waveIn.StartRecording();
        }

        private void OnDataAvailable(object sender, WaveInEventArgs args)
        {
            float maxValue = 32767/4;
            float ratio = 0f;
            float easedRatio = 0f;
            float ratioThreshold = 0.05f;
            int peakValue = 0;
            int bytesPerSample = 2;
            for (int index = 0; index < args.BytesRecorded; index += bytesPerSample)
            {
                int value = BitConverter.ToInt16(args.Buffer, index);
                peakValue = Math.Max(peakValue, value);

            }

            ratio = peakValue / maxValue;
            if (ratio > 1) ratio = 1;
            if (ratio > ratioThreshold) {
                easedRatio = ratio <= 0.5 ? ratio : easeOut(ratio);
            }
            

            Dispatcher.Invoke(() =>
            {
                //update UI
                var maxRotation = -40;
                ImageTop.Angle = maxRotation * easedRatio;
            });
        }

        private float easeIn(float ratio)
        {
            return 2 * ratio * ratio; // + L
        }
        private float easeOut(float ratio)
        {
            return (2f * ratio * (1f - ratio)) + 0.5f;
        }
    }
}
