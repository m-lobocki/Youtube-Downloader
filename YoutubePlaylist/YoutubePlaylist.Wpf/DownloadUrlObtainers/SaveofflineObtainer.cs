using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YoutubePlaylist.Wpf.DownloadUrlObtainers
{
    public class SaveofflineObtainer : IObtainer
    {
        private readonly Clip.Format format;
        private const int SafeDelay = 500;

        public SaveofflineObtainer(Clip.Format format)
        {
            this.format = format;
        }

        public async Task<string> GetDownloadUrl(string youtubeUrl)
        {
            await Task.Delay(SafeDelay);
            var response =
                await HttpHelper.Get("https://www.saveoffline.com/process/?url=" + youtubeUrl + "&type=text");
            string rawOutput = await HttpHelper.GetResponseContent(response);
            string[] lines = rawOutput.Split(new[] {"<br />"}, StringSplitOptions.None);

            string targetFormat = format.ToString().ToLower();
            string bestMatch = lines.First(line => line.Contains(targetFormat));
            string link = bestMatch.Split(new[] {": "}, StringSplitOptions.None).Last().Trim();

            return link;
        }
    }
}