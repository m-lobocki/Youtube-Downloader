using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using YoutubePlaylist.Wpf.DownloadUrlObtainers;

namespace YoutubePlaylist.Wpf
{
    public class YoutubeEngine
    {
        private const string YoutubeBaseUrl = "https://www.youtube.com";
        private readonly IObtainer obtainer;

        public YoutubeEngine(string downloadDestinationDirectory, Clip.Format format)
        {
            obtainer = new SaveofflineObtainer(format);
            DownloadDestinationDirectory = downloadDestinationDirectory;
            Format = format;
        }

        public delegate void PlaylistDownloadProgressChangedEventHander(
            object sender, PlaylistDownloadProgressChangedEventArgs eventArgs);
        public event PlaylistDownloadProgressChangedEventHander PlaylistDownloadProgressChanged;
        public event EventHandler GeneratingUrl;

        public string DownloadDestinationDirectory { get; set; }
        public Clip.Format Format { get; set; }

        /// <summary>
        /// Download a clip
        /// </summary>
        /// <param name="clip">The clip</param>
        /// <param name="throwIfExists">Throws <seealso cref="DownloadSkippedException"/> if already exists</param>
        /// <returns></returns>
        public async Task Download(Clip clip, bool throwIfExists = true)
        {
            GeneratingUrl?.Invoke(this, EventArgs.Empty);
            if (File.Exists(clip.FilePath) && throwIfExists) throw new DownloadSkippedException();
            string downloadUrl = await obtainer.GetDownloadUrl(clip.Url);

            using (WebClient client = new WebClient())
            {
                client.DownloadProgressChanged += (s, e) =>
                    PlaylistDownloadProgressChanged?.Invoke(this, new PlaylistDownloadProgressChangedEventArgs(e, clip.FilePath));
                await client.DownloadFileTaskAsync(downloadUrl, clip.FilePath);
            }
        }

        public async Task<IEnumerable<Clip>> ParsePlaylist(string playlistUrl)
        {
            HtmlDocument document = new HtmlDocument();
            WebResponse playlistResponse = await HttpHelper.Get(playlistUrl);
            document.Load(playlistResponse.GetResponseStream());
            HtmlNodeCollection rows = document.DocumentNode.SelectNodes("//tbody[@id=\"pl-load-more-destination\"]/tr");

            var clips = rows.Select(row => new Clip(
                title: WebUtility.HtmlDecode(row.GetAttributeValue("data-title", "")),
                url: YoutubeBaseUrl + row.SelectSingleNode("td[@class=\"pl-video-title\"]/a").GetAttributeValue("href", ""),
                fileDirectoryPath: DownloadDestinationDirectory,
                format: Format));

            return clips;
        }
    }
}
