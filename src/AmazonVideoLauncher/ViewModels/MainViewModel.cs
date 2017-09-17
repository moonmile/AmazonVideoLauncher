using AmazonVideoLauncher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonVideoLauncher.ViewModels
{
    class MainViewModel : ObservableObject
    {
        private List<Video> _Items;
        public List<Video> Items
        {
            get { return _Items; }
            set { SetProperty(ref _Items, value); }
        }

        public MainViewModel()
        {
            this._Items = new List<Video>();
        }

        void init()
        {
            Items.Add(new Video() { Title = "1. お前のドリルで天を突け!", Thum = "https://images-na.ssl-images-amazon.com/images/I/41cRf+TZLaL._SX180_.jpg", Url = "http://amazon.co.jp/gp/video/detail/B01MR5ZDY5/ref=atv_dp_pb_core?autoplay=1&t=1" });
            Items.Add(new Video() { Title = "2. 俺が乗るって言ってんだ!!", Thum = "https://images-na.ssl-images-amazon.com/images/I/51B5nx8SUFL._SX180_.jpg", Url = "http://amazon.co.jp/gp/video/detail/B01MUAP4GC/ref=atv_dp_pb_core?autoplay=1&t=1" });
            Items.Add(new Video() { Title = "3. 顔が2つたぁナマイキな!!", Thum = "https://images-na.ssl-images-amazon.com/images/I/41uhk6fxDCL._SX180_.jpg", Url = "http://amazon.co.jp/gp/video/detail/B01MT92QIS/ref=atv_dp_pb_core?autoplay=1&t=1" });
        }
    }
}
