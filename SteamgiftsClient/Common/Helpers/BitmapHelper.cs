using Avalonia.Media.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SteamgiftsClient.Common.Helpers
{
    public static class BitmapHelper
    {
        public static async Task<Bitmap?> LoadFromURL(string url)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(url))
                    {
                        response.EnsureSuccessStatusCode();

                        using (Stream inputStream = await response.Content.ReadAsStreamAsync())
                        {
                            return new Bitmap(inputStream);
                        }
                    }
                }
            }
            catch { }
            return null;
        }
    }
}
