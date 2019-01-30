using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YoutubePlaylist.Wpf
{
    public static class HttpHelper
    {
        public static async Task<string> Post(string url, NameValueCollection parameters)
        {
            using (WebClient client = new WebClient())
            {
                var response = await client.UploadValuesTaskAsync(url, parameters);
                return Encoding.UTF8.GetString(response);
            }
        }

        public static async Task<WebResponse> Get(string url)
        {
            WebRequest request = WebRequest.Create(url);
            request.Timeout = 30000;
            request.Method = "GET";

            return await request.GetResponseAsync();
        }

        public static async Task<string> GetResponseContent(WebRequest request)
        {
            using (WebResponse response = await request.GetResponseAsync())
            {
                return await GetResponseContent(response);
            }
        }

        public static async Task<string> GetResponseContent(WebResponse response)
        {
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
