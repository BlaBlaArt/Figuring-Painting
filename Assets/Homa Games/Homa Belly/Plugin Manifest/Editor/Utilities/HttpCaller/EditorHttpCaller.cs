using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace HomaGames.HomaBelly.Utilities
{
#if UNITY_EDITOR
    public class EditorHttpCaller<T>
    {
        #region IHttpCaller implementation

        public async Task<T> Get(string uri, IModelDeserializer<T> deserializer)
        {
            using (HttpClient client = GetHttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(uri).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    string resultString = await response.Content.ReadAsStringAsync();
                    return deserializer.Deserialize(resultString);
                }
            }

            return default;
        }

        public async Task<string> DownloadFile(string uri, string outputFilePath)
        {
            using (HttpClient client = GetHttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                    {
                        using (Stream streamToWriteTo = File.Open(outputFilePath, FileMode.Create))
                        {
                            await streamToReadFrom.CopyToAsync(streamToWriteTo);
                        }
                    }

                    HomaBellyEditorLog.Debug($"Done");
                    return outputFilePath;
                }
                else
                {
                    throw new FileNotFoundException(response.ReasonPhrase);
                }
            }
        }
        #endregion

        #region Private helpers

        private HttpClient GetHttpClient()
        {
            HttpClient client = new HttpClient();
            return client;
        }

        #endregion
    }
#endif
}