using AmazonVideoLauncher.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonVideoLauncher.Models
{
    /// <summary>
    /// 個別の Video クラス
    /// </summary>
    class Video : ObservableObject
    {
        public string Title { get; set; }
        public string Thum { get; set; }
        public string Url { get; set; }
        bool _IsCheck;
        public bool IsCheck
        {
            get { return _IsCheck; }
            set { this.SetProperty(ref _IsCheck, value); }
        }
    }

    /// <summary>
    /// ビデオタイトルクラス
    /// </summary>
    class VideoTitle : Video
    {
        public List<Video> Videos { get; set; }
    }
}
