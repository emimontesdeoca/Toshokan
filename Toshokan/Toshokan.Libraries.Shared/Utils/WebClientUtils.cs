using System.Net;
using System.Net.Http.Headers;

namespace Toshokan.Libraries.Shared
{
    public class WebClientUtils
    {
        public static string GetString(string url)
        {
            using (var wc = new WebClient())
            {
                wc.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                wc.Headers.Add("referer", "https://mangakakalot.com/");

                return wc.DownloadString(url);
            }
        }

        public static async Task<string> GetStringAsync(string url)
        {
            using (var wc = new WebClient())
            {
                wc.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                wc.Headers.Add("referer", "https://mangakakalot.com/");

                return await wc.DownloadStringTaskAsync(url);
            }
        }

        public static byte[] GetData(string url)
        {
            using (var wc = new WebClient())
            {
                wc.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                wc.Headers.Add("referer", "https://mangakakalot.com/");

                return wc.DownloadData(url);
            }
        }

        public static async Task<byte[]> GetDataAsync(string url)
        {
            using (var wc = new WebClient())
            {
                wc.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                wc.Headers.Add("referer", "https://mangakakalot.com/");

                return await wc.DownloadDataTaskAsync(url);
            }
        }

    }
}