namespace UniMobileProject.src.Services.Http
{
    public class HttpService
    {
        private Uri baseAddress;
        private readonly HttpClient _client;

        public HttpService(string endpoint)
        {
            baseAddress = new Uri($"https://hhb-testing.ruslan.page/{endpoint}/");

            _client = new HttpClient()
            {
                BaseAddress = baseAddress
            };
        }

        public HttpClient GetClient()
        {
            return _client;
        }
    }
}
