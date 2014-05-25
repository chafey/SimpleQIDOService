using System.Web.Http;
using SimpleQIDOService.Lib;

namespace SimpleQIDOService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.EnableCors();
            
            // TODO: Manage singletons through DI Container
            DICOMTagOrKeywordLookup.Instance();
            StudyDatabase.Instance();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
