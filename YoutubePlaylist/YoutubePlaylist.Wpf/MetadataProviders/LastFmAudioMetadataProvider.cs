using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using YoutubePlaylist.Wpf.Properties;
using File = TagLib.File;

namespace YoutubePlaylist.Wpf.MetadataProviders
{
    public class LastFmAudioMetadataProvider : IMetadataProvider
    {
        public async Task DownloadDataTo(string filepath)
        {
            File targetFile = File.Create(filepath);
            if (!string.IsNullOrEmpty(targetFile.Tag.Title)) return;

            if (EligibleFileName(targetFile.Name))
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(targetFile.Name);
                var titleParts = fileNameWithoutExtension?.Split('-').ToList();
                if (titleParts == null) return;
                WebRequest request = WebRequest.Create($"http://ws.audioscrobbler.com/2.0/?method=track.getInfo&api_key={Settings.Default.LastFmApiKey}&artist={titleParts[0].Trim()}&track={titleParts[1].Trim()}&autocorrect=1");

                string response = await HttpHelper.GetResponseContent(request);
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(response);

                try
                {
                    UpdateMetadata(targetFile, xml);
                }
                catch
                {
                    //it happens...
                }
            }
        }

        private void UpdateMetadata(File file, XmlDocument xml)
        {
            string author = xml.SelectSingleNode("//artist/name")?.InnerText;
            string title = xml.SelectSingleNode("//name")?.InnerText;
            XmlNode albumNode = xml.SelectSingleNode("//album");

            string albumName = albumNode?.SelectSingleNode("//title")?.InnerText;
            var genres = GetGenres(xml);

            file.Tag.Title = title;
            file.Tag.Album = albumName;
            file.Tag.AlbumArtists = new[] { author };
            file.Tag.Genres = genres;
            file.Save();
        }

        private string[] GetGenres(XmlDocument xml)
        {
            var genres = new List<string>();

            foreach (XmlNode node in xml.SelectSingleNode("//toptags").SelectNodes("tag"))
            {
                string genre = node.SelectSingleNode("name")?.InnerText;
                if (!string.IsNullOrEmpty(genre)) genres.Add(genre);
            }

            return genres.ToArray();
        }

        private bool EligibleFileName(string fileName)
        {
            return fileName.Contains("-");
        }
    }
}
