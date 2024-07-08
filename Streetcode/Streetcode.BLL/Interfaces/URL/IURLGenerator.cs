﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Streetcode.BLL.Interfaces.URL
{
    public interface IURLGenerator
    {
        public string Url(
            string action,
            string controller,
            object values,
            HttpContext httpContext);
    }
}