using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Authorization
{
    public class PerfReviewAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session["loginuserid"] != null)
            {
                string level = httpContext.Session["level"].ToString();
                if (level == "PMO" || level == "Team Leader" || level == "Head of PMO" || level == "Head of Delivery" || level == "Management") return true;
                else return false;
            }
            else return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("/Task/Index");
        }
    }
}