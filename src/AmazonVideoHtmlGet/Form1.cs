using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AmazonVideoHtmlGet
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            wb.NewWindow += Wb_NewWindow;
        }

        private void Wb_NewWindow(object sender, CancelEventArgs e)
        {
            wb.Navigate(wb.StatusText);
            e.Cancel = true;
        }


        /// <summary>
        /// Amazon接続
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            var url = "https://www.amazon.co.jp/Prime-Video/b/ref=sv_jp_2?ie=UTF8&node=3535604051";
            wb.ScriptErrorsSuppressed = true;
            wb.Navigate(new Uri(url));

        }
        // HTML取得
        private void button2_Click(object sender, EventArgs e)
        {
            var htmlText = wb.DocumentText;

            var path = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\amazonvideo.html";
            var fs = new System.IO.StreamWriter(path);
            fs.Write(htmlText);
            fs.Close();
        }

        /// <summary>
        /// 密林ヘビーロードに送信
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            var htmlText = wb.DocumentText;
            Clipboard.Clear();
            Clipboard.SetText(htmlText);
            Process.Start("amazon-video-launcher://api/HTML");
        }
    }
}
