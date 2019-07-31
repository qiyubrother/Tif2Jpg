﻿using System;
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
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            txtDir.Text = Application.StartupPath;
        }
        private void btnDo_Click(object sender, EventArgs e)
        {
            var files = Directory.GetFiles(txtDir.Text, txtSearchPattern.Text, SearchOption.AllDirectories);
            foreach(var inFileName in files)
            {
                var fi = new FileInfo(inFileName);
                var outFileName = Path.Combine(fi.DirectoryName, fi.Name).ToLower().Replace(".tif", "") + ".jpg";
                Convert(inFileName, outFileName, ConvertCallBack);
            }
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
            var status = result ? "OK" : "ERROR";
            Console.WriteLine($"{inFileName} -> {outFileName} ::{status}");
        }
    }
}