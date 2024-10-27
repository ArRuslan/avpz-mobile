namespace UniMobileProject.src.Services.Http
{
    public class HttpServiceFactory : IHttpServiceFactory
    {
        public HttpService Create(string endpoint)
        {
            return new HttpService(endpoint);
        }
    }
}
