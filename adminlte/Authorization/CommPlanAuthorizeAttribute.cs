using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Authorization
{
    public class CommPlanAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session["loginuserid"] != null)
            {
                string level = httpContext.Session["level"].ToString();
                if (level != "Admin") return true;
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