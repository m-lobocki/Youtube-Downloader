using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace YoutubePlaylist.Wpf.DownloadUrlObtainers
{
    public class OfflibertyObtainer : IObtainer
    {
        private const string FormUrl = "http://offliberty.com/off03.php";
        private const string VideoUrlParamName = "track";
        private const int SafeDelay = 5000; // prevent from being banned

        public async Task<string> GetDownloadUrl(string youtubeUrl)
        {
            string response = await HttpHelper.Post(FormUrl, new NameValueCollection {{VideoUrlParamName, youtubeUrl}});

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(response);

            if(doc.DocumentNode.InnerText.Contains("Try again later")) throw new InvalidOperationException("Downloading is initialized too ofter, try again later");

            HtmlNode downloadLink = doc.DocumentNode.SelectSingleNode("//a[@class=\"download\"]");
            string downloadUrl = downloadLink.GetAttributeValue("href", "");

            await Task.Delay(SafeDelay);

            return downloadUrl;
        }
    }
}
