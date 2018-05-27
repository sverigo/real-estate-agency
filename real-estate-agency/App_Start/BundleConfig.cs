using System.Web.Optimization;

namespace real_estate_agency
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/style.css"));

            bundles.Add(new StyleBundle("~/Content/fancybox").Include(
                "~/Content/jquery.fancybox.css"));

            bundles.Add(new StyleBundle("~/Content/pagedlist").Include(
                "~/Content/PagedList.css"));

            bundles.Add(new StyleBundle("~/Content/slick").Include(
                "~/Scripts/slick/slick.css",
                "~/Scripts/slick/slick-theme.css"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-3.3.1.slim.js",
                "~/Scripts/jquery-3.3.1.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                "~/Scripts/knockout-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/unobtrusive-ajax").Include(
                "~/Scripts/jquery.unobtrusive-ajax.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js",
                "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/fancybox").Include(
                "~/Scripts/jquery.fancybox.pack.js"));

            bundles.Add(new ScriptBundle("~/bundles/fancybox-gallery-single").Include(
                "~/Scripts/gallery-single.js"));

            bundles.Add(new ScriptBundle("~/bundles/fancybox-gallery-group").Include(
                "~/Scripts/jquery.mousewheel-3.0.6.pack.js",
                "~/Scripts/gallery-group.js"));

            bundles.Add(new ScriptBundle("~/bundles/additional-fields").Include(
                "~/Scripts/additional_fields.js"));

            bundles.Add(new ScriptBundle("~/bundles/slick").Include(
                "~/Scripts/slick/slick.js",
                "~/Scripts/slick/slick-slider.js"));
        }
    }
}
