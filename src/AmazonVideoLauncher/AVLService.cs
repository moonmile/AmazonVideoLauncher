using AmazonVideoLauncher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonVideoHtmlGet
{
    public class AVLService
    {
        public VideoTitle Analysis( string html )
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var box = new VideoTitle();
            // タイトルを取得
            var el = doc.DocumentNode.SelectSingleNode(@"//h1[@data-automation-id=""title""]");
            box.Title = el.InnerText.Trim();
            var div = doc.DocumentNode.SelectSingleNode(@"//div[@class=""av-fallback-packshot av-season-packshot""]");
            box.Thum = div.SelectSingleNode("img").Attributes["src"].Value;
            // vtitle.Videos.Add(vtitle);

            // 各話を取得
            var lst = box.Videos;
            var packs = doc.DocumentNode.SelectNodes(@"//div[contains(@class,'dv-episode-container')]");
            foreach (var it in packs.ToList())
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
            return box;
        }

        /// <summary>
        /// データを保存
        /// </summary>
        public string SaveTo(VideoTitle box)
        {
            var js = new Newtonsoft.Json.JsonSerializer();
            var sw = new System.IO.StringWriter();
            js.Serialize(sw, box);
            var json = sw.ToString();
            return json;
        }
        /// <summary>
        /// データをロード
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public VideoTitle LoadFrom( string json )
        {
            VideoTitle box = null;
            try
            {
                var js = new Newtonsoft.Json.JsonSerializer();
                var sr = new System.IO.StringReader(json);
                var jr = new Newtonsoft.Json.JsonTextReader(sr);
                box = js.Deserialize<VideoTitle>(jr);
            }
            catch (Exception) { }
            return box;
        }
    }
}
