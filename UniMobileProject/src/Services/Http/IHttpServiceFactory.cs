namespace UniMobileProject.src.Services.Http
{
    public interface IHttpServiceFactory
    {
        HttpService Create(string endpoint);
    }
}
