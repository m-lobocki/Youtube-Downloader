using System.Threading.Tasks;

namespace YoutubePlaylist.Wpf.DownloadUrlObtainers
{
    public interface IObtainer
    {
        Task<string> GetDownloadUrl(string youtubeUrl);
    }
}
