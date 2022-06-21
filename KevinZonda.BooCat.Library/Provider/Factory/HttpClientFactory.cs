namespace KevinZonda.BooCat.Library.Provider.Factory
{
    internal class HttpClientFatory
    {
        private static HttpClient? _httpClient = null;
        
        public static HttpClient Produce()
        {
            if (_httpClient == null) _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(7);
            return _httpClient;
        }
    }
}
