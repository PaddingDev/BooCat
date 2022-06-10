namespace KevinZonda.Bookie.Library.Provider.Factory
{
    internal class HttpClientFatory
    {
        public static HttpClient Produce()
        {
            return new HttpClient();
        }
    }
}
