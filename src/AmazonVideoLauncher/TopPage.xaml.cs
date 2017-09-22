using AmazonVideoHtmlGet;
using AmazonVideoLauncher.Models;
using AmazonVideoLauncher.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace AmazonVideoLauncher
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class TopPage : Page
    {
        public TopPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            this.Loaded += TopPage_Loaded;
        }
        TopViewModel vm;
        static TopViewModel _vm;

        private void TopPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (vm == null)
            {
                // 初回のみファイルからロードする
                // this.DataContext = this.vm = new TopViewModel();
                dataload();
                TopPage._vm = vm;
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }


        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as VideoTitle;
            this.Frame.Navigate(typeof(MainPage), item.Videos);
        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }

        private async void Grid_Drop(object sender, DragEventArgs e)
        {
            var items = await e.DataView.GetStorageItemsAsync();
            if (items.Count > 0)
            {
                var file = items[0] as StorageFile;
                using (var st = await file.OpenStreamForReadAsync())
                {
                    using (var sr = new System.IO.StreamReader(st))
                    {
                        var doc = new HtmlAgilityPack.HtmlDocument();
                        doc.Load(sr);
                        // タイトルを取得
                        VideoTitle vtitle = new VideoTitle();
                        vtitle.Videos = new List<Video>();
                        var el = doc.DocumentNode.SelectSingleNode(@"//h1[@id=""aiv-content-title""]");
                        vtitle.Title = el.InnerText.Trim();
                        var div = doc.DocumentNode.SelectSingleNode(@"//div[@class=""dp-meta-icon-container""]");
                        vtitle.Thum = div.SelectSingleNode("img").Attributes["src"].Value;
                        vm.Items.Add(vtitle);

                        // 各話を取得
                        var lst = vtitle.Videos;
                        var packs = doc.DocumentNode.SelectNodes(@"//div[contains(@class,'dv-episode-container')]");
                        foreach ( var it in packs.ToList() )
                        {
                            try
                            {
                                var video = new Video();
                                video.Url = it.SelectSingleNode(@".//a[contains(@class,'dv-playback-container')]").Attributes["href"].Value;
                                video.Url = "http://amazon.co.jp" + video.Url.Replace("&amp;", "&");
                                video.Thum = it.SelectSingleNode(@".//div[@class=""dv-el-packshot-image""]").Attributes["style"].Value;
                                video.Thum = video.Thum
                                    .Replace("background-image: url(\"", "")
                                    .Replace("background-image: url(", "")
                                    .Replace("\");", "")
                                    .Replace(");", "");
                                video.Title = it.SelectSingleNode(@".//div[@class=""dv-el-title""]").InnerText
                                    .Replace("<!-- Title -->", "")
                                    .Trim();
                                lst.Add(video);
                            }
                            catch { }
                        }
                        // ファイルに保存する
                        datasave();
                    }
                }
            }
        }
        /// <summary>
        /// データを保存
        /// </summary>
        async void datasave()
        {
            var js = new Newtonsoft.Json.JsonSerializer();
            var sw = new System.IO.StringWriter();
            js.Serialize(sw, this.vm);
            var json = sw.ToString();

            var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var dataFile = await localFolder.CreateFileAsync("data.json", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(dataFile, json);
        }
        /// <summary>
        /// データを読み込み
        /// </summary>
        async void dataload()
        {
            try
            {
                var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                StorageFile dataFile = await localFolder.GetFileAsync("data.json");
                String json = await FileIO.ReadTextAsync(dataFile);
                var js = new Newtonsoft.Json.JsonSerializer();
                var sr = new System.IO.StringReader(json);
                var jr = new Newtonsoft.Json.JsonTextReader(sr);
                vm = js.Deserialize<TopViewModel>(jr);
                this.DataContext = vm;
            }
            catch (Exception)
            {
                this.DataContext = this.vm = new TopViewModel();
            }
        }

        /// <summary>
        /// 一覧から削除する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarRemove_Click(object sender, RoutedEventArgs e)
        {
            var lst = new List<VideoTitle>();
            foreach( var v in vm.Items )
            {
                if ( v.IsCheck == true )
                {
                    lst.Add(v);
                }
            }
            foreach ( var v in lst )
            {
                vm.Items.Remove(v);
            }
            datasave();
        }

        /// <summary>
        /// プロトコルを受信
        /// </summary>
        /// <param name="url"></param>
        public async void RecvProtocol( Uri url )
        {
            if ( url.PathAndQuery == "/HTML")
            {
                this.DataContext = this.vm = TopPage._vm;
                var data = Clipboard.GetContent();
                var html = await data.GetTextAsync();
                var sv = new AVLService();
                var box = sv.Analysis(html);
                this.vm.Items.Add(box);
            }
        }
    }
}
