using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Streetcode.BLL.Interfaces.URL;

namespace Streetcode.BLL.Services.URL
{
    public class URLGenerator : IURLGenerator
    {
        private LinkGenerator _linkGenerator;
        public URLGenerator(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        public string Url(string action, string controller, object values, HttpContext httpContext)
        {
            string url = _linkGenerator.GetPathByAction(action, controller, values) !;
            return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{url}";
        }

        public string Url(string route, Dictionary<string, object> queryValues, HttpContext httpContext)
        {
            StringBuilder url = new StringBuilder();
            url.Append(route);
            if (queryValues.Count > 0)
            {
                int i = 0;
                foreach (var k_v in queryValues)
                {
                    if (i == 0)
                    {
                        url.Append($"?{k_v.Key}={k_v.Value}");
                    }
                    else 
                    {
                        url.Append($"&{k_v.Key}={k_v.Value}");
                    }

                    ++i;
                }
            }

            return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{url}";
        }
    }
}
