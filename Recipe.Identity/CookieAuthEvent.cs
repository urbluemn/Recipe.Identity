using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Recipe.Identity
{
    public class CookieAuthEvent : CookieAuthenticationEvents
    {
        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            context.Request.HttpContext.Items.Add("ExpiresUTC", context.Properties.ExpiresUtc);
        }
    }
}