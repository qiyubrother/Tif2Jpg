using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.IO;

namespace Tif2Jpg
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnDo_Click(object sender, EventArgs e)
        {

            Convert("", "", ConvertCallBack);
        }

        private void Convert(string inFileName, string outFileName, Action<bool, string, string> callback)
        {
            using (Stream imageStreamSource = new FileStream(inFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var decoder = new TiffBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(decoder.Frames[0]);

                using (var stream = new FileStream(outFileName, FileMode.Create))
                {
                    encoder.Save(stream);
                    callback(true, inFileName, outFileName);
                }
            }
        }

        private void ConvertCallBack(bool result, string inFileName, string outFileName)
        {

        }
    }
}
