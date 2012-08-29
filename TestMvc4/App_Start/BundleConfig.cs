using System.Configuration;
using System.IO;
using System.Web.Optimization;
using dotless.Core;
using dotless.Core.Input;
using dotless.Core.configuration;

namespace TestMvc4
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {

            var lessBundle = new Bundle("~/bundles/less").IncludeDirectory("~/Content/less/", "bootstrap.less");
            lessBundle.Transforms.Add(new LessTransform());
            //lessBundle.Transforms.Add(new CssMinify());
            bundles.Add(lessBundle);

            var include = new Bundle("~/bundles/jquery", new JsMinify()).Include("~/Scripts/jquery-1.*");
            //include.Transforms.Add(new JsMinify());
            bundles.Add(include);

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));
        }
    }

    public class LessTransform : IBundleTransform
    {
        public static string BundlePath { get; private set; }

        public void Process(BundleContext context, BundleResponse response)
        {
            setBasePath(context);

            var config = new DotlessConfiguration(DotlessConfiguration.GetDefault())
                             {LessSource = typeof (TwitterBootstrapLessMinifyBundleFileReader)};

            response.Content = Less.Parse(Less.Parse(response.Content, config), config);
        }

        private void setBasePath(BundleContext context)
        {
            //var importsFolder = ConfigurationManager.AppSettings["TwitterBootstrapLessImportsFolder"] ?? "imports";
            var importsFolder = "Content/less";
            BundlePath = context.HttpContext.Server.MapPath("~/" + importsFolder);
        }
    }

    public class TwitterBootstrapLessMinifyBundleFileReader : IFileReader
    {
        public IPathResolver PathResolver { get; set; }
        private string basePath;

        public TwitterBootstrapLessMinifyBundleFileReader() : this(new RelativePathResolver())
        {
        }

        public TwitterBootstrapLessMinifyBundleFileReader(IPathResolver pathResolver)
        {
            PathResolver = pathResolver;
            basePath = LessTransform.BundlePath;
        }

        public bool DoesFileExist(string fileName)
        {
            fileName = GetFullPath(fileName);
            return File.Exists(fileName);
        }

        public byte[] GetBinaryFileContents(string fileName)
        {
            fileName = GetFullPath(fileName);
            return File.ReadAllBytes(fileName);
        }

        public string GetFileContents(string fileName)
        {
            fileName = GetFullPath(fileName);
            return File.ReadAllText(fileName);
        }

        private string GetFullPath(string fileName)
        {
            return PathResolver.GetFullPath(basePath + "\\" + fileName);
        }

    }
}