using System.Net;

namespace YoutubePlaylist.Wpf
{
    public class PlaylistDownloadProgressChangedEventArgs
    {
        public PlaylistDownloadProgressChangedEventArgs(DownloadProgressChangedEventArgs downloadProgressChangedEventArgs, string fileName)
        {
            DownloadProgressEventArgs = downloadProgressChangedEventArgs;
            FileName = fileName;
        }

        public DownloadProgressChangedEventArgs DownloadProgressEventArgs { get; }
        public string FileName { get; }
    }
}
