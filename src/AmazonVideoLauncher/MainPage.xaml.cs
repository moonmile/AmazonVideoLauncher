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
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace AmazonVideoLauncher
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this.vm; // = new MainViewModel();
        }
        MainViewModel vm;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.vm = new MainViewModel();
            vm.Items = e.Parameter as List<Video>;
            base.OnNavigatedTo(e);

            // [戻る]ボタンを表示するかどうかを設定する
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                Frame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;

            //［戻る］ボタンが押されたときのイベントを結び付ける
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView()
              .BackRequested += MainPage_BackRequested;
        }

        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
                e.Handled = true;
            }
        }

        // ページを離れる場合の処理
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            // ［戻る］ボタンのイベントハンドラーを解除
            SystemNavigationManager.GetForCurrentView()
              .BackRequested -= MainPage_BackRequested;
        }

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as Video;
            // ファイルの場合
            var url = item.Url;
            bool success = await Launcher.LaunchUriAsync( new Uri(url),
                new LauncherOptions { DesiredRemainingView = ViewSizePreference.Default });
            item.IsCheck = true;
        }

        private void AppBarClear_Click(object sender, RoutedEventArgs e)
        {
            foreach ( var v in vm.Items )
            {
                v.IsCheck = false;
            }
        }

    }
}
