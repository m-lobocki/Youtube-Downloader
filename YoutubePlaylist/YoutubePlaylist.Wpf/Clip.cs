using System.IO;
using System.Text.RegularExpressions;

namespace YoutubePlaylist.Wpf
{
    public class Clip
    {
        public enum Format
        {
            Audio,
            Video
        }

        public Clip(string title, string url, string fileDirectoryPath, Format format)
        {
            Title = title;
            Url = url;

            string extension = format == Format.Audio ? ".mp3" : ".mp4";
            string fileName = FixFileName(title + extension);
            FilePath = Path.Combine(fileDirectoryPath, fileName);
        }

        public string Title { get; }
        public string Url { get; }
        public string FilePath { get; }
        public bool IsChecked { get; set; } = true;

        public override string ToString()
        {
            return Title;
        }       

        private string FixFileName(string fileName)
        {
            string fixedTitle = fileName.Replace("/", "").Replace("\\", "").Replace(":", " -").Replace("\"", "").Replace("|", "");
            string fixedFileName = Regex.Replace(fixedTitle, @"\s*[\(\[]+(.*)[\)\]]+\s*", "", RegexOptions.IgnoreCase);

            return fixedFileName;
        }
    }
}
