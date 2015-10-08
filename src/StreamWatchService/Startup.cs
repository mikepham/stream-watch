namespace StreamWatchService
{
    using System.Web.Http;

    using Owin;

    using SimpleInjector.Integration.WebApi;

    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();
            config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(Program.Container);
            config.Filters.Add(new AuthorizeAttribute());
            config.MapHttpAttributeRoutes();

            appBuilder.UseWebApi(config);
        }
    }
}