using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubePlaylist.Wpf.MetadataProviders
{
    public interface IMetadataProvider
    {
        Task DownloadDataTo(string filepath);
    }
}
