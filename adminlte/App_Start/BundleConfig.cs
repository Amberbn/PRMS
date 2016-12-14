using System.Web;
using System.Web.Optimization;

namespace adminlte
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.min.js",
                        "~/Scripts/jquery-ui-1.10.3.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                       "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                       "~/Scripts/bootstrap3-wysihtml5.js"));

            bundles.Add(new ScriptBundle("~/bundles/additional").Include(
                       "~/Scripts/app.js",
                       "~/Scripts/dashboard.js",
                       "~/Scripts/demo.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css",
            //          "~/Content/adminlte.css",
            //          "~/Content/css/font-awesome.css",
            //          "~/Content/ionicons.css",
            //          "~/Content/css/iCheck/all.css"));
            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/adminlte.css",
                        "~/Content/bootstrap.css",
                        "~/Content/css/font-awesome.css",
                        "~/Content/ionicons.css",
                        "~/Content/css/iCheck/all.css"));
        }
    }
}
