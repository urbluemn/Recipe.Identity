using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recipe.Identity.Models.Logout
{
    public class LoggedOutViewModel
    {
        public string PostLogoutRedirectUri { get; set; }
        public string ClientName { get; set; }
        public string SignOutIframeUrl { get; set; }

        public bool AutomaticRedirectAfterSignOut { get; set; }

        public string LogoutId { get; set; }
    }
}