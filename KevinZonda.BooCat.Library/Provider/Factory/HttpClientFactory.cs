namespace KevinZonda.BooCat.Library.Provider.Factory
{
    internal class HttpClientFatory
    {
        private static HttpClient? _httpClient = null;

        public static HttpClient Produce()
        {
            if (_httpClient == null)
            {
                _httpClient = new HttpClient();
                _httpClient.Timeout = TimeSpan.FromSeconds(7);
                _httpClient.DefaultRequestHeaders
                           .UserAgent
                           .ParseAdd(
                               "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                               "AppleWebKit/537.36 (KHTML, like Gecko) " +
                               "Chrome/102.0.0.0 Safari/537.36");
            }
            return _httpClient;
        }
    }
}
