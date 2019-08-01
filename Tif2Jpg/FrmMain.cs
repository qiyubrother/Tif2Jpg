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
            int pos = 1;
            int total = files.Length;
            foreach(var inFileName in files)
            {
                var fi = new FileInfo(inFileName);
                var outFileName = Path.Combine(fi.DirectoryName, fi.Name).ToLower().Replace(".tif", "") + ".jpg";
                Console.Write($"[{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff")}][{pos++}/{total}]{inFileName} -> {outFileName} ::");
                Convert(inFileName, outFileName, ConvertCallBack);
            }

            Console.WriteLine("-- Finished all --");
        }

        private void Convert(string inFileName, string outFileName, Action<bool, string, string, string> callback)
        {
            try
            {
                using (Stream imageStreamSource = new FileStream(inFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var decoder = new TiffBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
                    var encoder = new JpegBitmapEncoder();
                    encoder.QualityLevel = 100;

                    encoder.Frames.Add(decoder.Frames[0]);
                    using (var stream = new FileStream(outFileName, FileMode.Create))
                    {
                        encoder.Save(stream);
                        callback(true, string.Empty, inFileName, outFileName);
                        Console.WriteLine();
                    }
                }
            }
            catch(Exception ex)
            {
                callback(false, ex.Message, inFileName, outFileName);
                try
                {
                    // 删除outFileName
                    try
                    {
                        File.Delete(outFileName);
                    }
                    catch { }
                    // 保存为 Bitmap 文件
                    var fi = new FileInfo(inFileName);
                    outFileName = Path.Combine(fi.DirectoryName, fi.Name).ToLower().Replace(".tif", "") + ".bmp";
                    Console.Write($" Retry to {outFileName} ::");
                    using (Stream imageStreamSource = new FileStream(inFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var decoder = new TiffBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
                        var encoder = new BmpBitmapEncoder();

                        encoder.Frames.Add(decoder.Frames[0]);
                        using (var stream = new FileStream(outFileName, FileMode.Create))
                        {
                            encoder.Save(stream);
                            callback(true, string.Empty, inFileName, outFileName);
                        }
                    }
                }
                catch(Exception ex2)
                {
                    callback(false, ex.Message, inFileName, outFileName);
                }
                finally
                {
                    Console.WriteLine();
                }
            }
        }

        private void ConvertCallBack(bool result, string errorMessage, string inFileName, string outFileName)
        {
            var status = result ? "OK" : "ERROR";
            if (!result)
            {
                var fcolor = Console.ForegroundColor;
                var bcolor = Console.BackgroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.Write($"{status}");
                Console.ForegroundColor = fcolor;
                Console.BackgroundColor = bcolor;
            }
            else
            {
                Console.Write($"{status}");
            }
        }
    }
}
