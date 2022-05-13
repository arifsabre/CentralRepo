using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ManufacturingManagement_V2
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


            // To disable tracing in your application, please comment out or remove the following line of code  
            // For more information, refer to: http://www.asp.net/web-api  
            //config.EnableSystemDiagnosticsTracing();

            // Adding formatter for Json   
            //config.Formatters.JsonFormatter.MediaTypeMappings.Add(
                //new System.Net.Http.Formatting.QueryStringMapping("type", "json", new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")));

            // Adding formatter for XML   
            //config.Formatters.XmlFormatter.MediaTypeMappings.Add(
                //new System.Net.Http.Formatting.QueryStringMapping("type", "xml", new System.Net.Http.Headers.MediaTypeHeaderValue("application/xml")));


        }
    }
}
