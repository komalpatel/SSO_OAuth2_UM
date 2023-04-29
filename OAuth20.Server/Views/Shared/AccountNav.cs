using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OAuth20.Server.Views.Shared
{
    public static class AccountNav
    {
        public static string Index => "Index";

        public static string Email => "Email";

        public static string CustomerController => "OAuth20.Server.Controllers.CustomerController";

        public static string ConnectMobileController => "OAuth20.Server.Controllers.ConnectMobileController";

        public static string StripeController => "OAuth20.Server.Controllers.StripeController";

        public static string DeletePersonalData => "DeletePersonalData";

        public static string ExternalLogins => "ExternalLogins";

        public static string PersonalData => "PersonalData";

        public static string TwoFactorAuthentication => "TwoFactorAuthentication";

        public static string CustomerNavClass(ViewContext viewContext) => PageNavClass(viewContext, CustomerController);

        public static string ConnectMobileNavClass(ViewContext viewContext) => PageNavClass(viewContext, ConnectMobileController);
    
        public static string StripeNavClass(ViewContext viewContext) => PageNavClass(viewContext, StripeController);

        public static string DeletePersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, DeletePersonalData);

        public static string ExternalLoginsNavClass(ViewContext viewContext) => PageNavClass(viewContext, ExternalLogins);

        public static string PersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, PersonalData);

        public static string TwoFactorAuthenticationNavClass(ViewContext viewContext) => PageNavClass(viewContext, TwoFactorAuthentication);

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
