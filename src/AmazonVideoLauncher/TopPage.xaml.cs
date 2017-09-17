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

        private void TopPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (vm == null)
            {
                // 初回のみファイルからロードする
                // this.DataContext = this.vm = new TopViewModel();
                dataload();
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

                        // タイトルを取得
                        VideoTitle vtitle = new VideoTitle();
                        vtitle.Videos = new List<Video>();
                        while (!sr.EndOfStream)
                        {
                            var line = sr.ReadLine();
                            if ( line.IndexOf("aiv-content-title") > 0 )
                            {
                                line = sr.ReadLine();
                                var t = line.Trim();
                                vtitle.Title = System.Net.WebUtility.HtmlDecode(t);
                            }
                            if (line.IndexOf("https://images-na.ssl-images-amazon.com/images/I") > 0)
                            {
                                var r = new Regex(@"src=""([^""]*)""");
                                var m = r.Match(line);
                                vtitle.Thum = m.Groups[1].Value;
                                break;
                            }
                        }
                        vm.Items.Add(vtitle);

                        // 各話を取得
                        Video video = new Video();
                        var lst = vtitle.Videos;
                        while (!sr.EndOfStream)
                        {
                            var line = sr.ReadLine();
                            if (line.IndexOf("dv-playback-container") > 0)
                            {
                                video = new Video();
                                var r = new Regex(@"href=""([^""]*)""");
                                var m = r.Match(line);
                                video.Url = "http://amazon.co.jp" + m.Groups[1].Value.Replace("&amp;", "&");
                                lst.Add(video);
                                //video = null;
                            }
                            if (line.IndexOf("dv-el-packshot-image") > 0)
                            {
                                var r = new Regex("url\\((.*)\\);");
                                var m = r.Match(line);
                                video.Thum = m.Groups[1].Value;
                            }
                            if (line.IndexOf(@"class=""dv-el-title""") > 0)
                            {
                                line = sr.ReadLine();
                                video.Title = line.Trim();
                            }
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
    }
}
