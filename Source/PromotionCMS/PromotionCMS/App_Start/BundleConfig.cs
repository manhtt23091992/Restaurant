using System.Collections.Generic;
using System.Web;
using System.Web.Optimization;

namespace PromotionCMS
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //var stylebundle = new StyleBundle("~/resources/admin/css").Include(
            //   "~/Resources/admin/plugins/bootstrap/css/bootstrap.min.css",
            //    "~/Resources/admin/css/font-awesome.min.css",
            //    "~/Resources/admin/css/theme.css",
            //    "~/Resources/admin/css/skins.min.css",
            //    "~/Resources/admin/css/rtl.min.css",
            //   "~/Resources/admin/css/scroll.css",
            //   "~/Resources/admin/css/jquery.datetimepicker.css",
            //   "~/Resources/admin/css/select2.css",
            //   "~/Resources/admin/plugins/jstree/themes/default/jstree.min.css",
            //   "~/Resources/admin/css/loader.css",
            //   "~/Resources/admin/plugins/select2/select2.min.css"
            //    );
            //stylebundle.Orderer = new NonOrderingBundleOrderer();
            //bundles.Add(stylebundle);

            ////Main Javascript
            //var scriptbundle = new ScriptBundle("~/resources/admin/js").Include(
            //    "~/Resources/admin/plugins/bootstrap/js/bootstrap.min.js",
            //    "~/Resources/admin/js/jquery-ui.custom.min.js",
            //    "~/Resources/admin/js/jquery.ui.touch-punch.min.js",
            //    "~/Resources/admin/js/moment.min.js",
            //    "~/Resources/admin/js/jquery.easypiechart.min.js",
            //    "~/Resources/admin/js/jquery.sparkline.min.js",
            //    "~/Resources/admin/js/flot/jquery.flot.min.js",
            //    "~/Resources/admin/js/flot/jquery.flot.pie.min.js",
            //    "~/Resources/admin/js/flot/jquery.flot.resize.min.js",
            //    "~/Resources/admin/js/elements.min.js",
            //    "~/Resources/admin/js/sms.min.js",
            //    "~/Resources/admin/js/sms-extra.min.js",
            //    "~/Resources/admin/js/moment-with-locales.js",
            //    "~/Resources/admin/js/jquery.dataTables.min.js",
            //    "~/Resources/admin/js/custom.js",
            //    "~/Resources/admin/js/select2.min.js",
            //    "~/Resources/admin/plugins/jstree/jstree.min.js",
            //    "~/Resources/admin/js/jquery.validate.min.js",
            //    "~/Resources/admin/plugins/select2/select2.min.js"
            //    );
            //scriptbundle.Orderer = new NonOrderingBundleOrderer();

            //bundles.Add(scriptbundle);

            //var angularbudle = new ScriptBundle("~/resources/admin/angularjs").Include(
            //    "~/Resources/admin/plugins/angularjs/angular.min.js",
            //    "~/Resources/admin/plugins/angularjs/angular-sanitize.js",
            //    "~/Resources/admin/plugins/angularjs/angular-route.min.js",
            //    "~/Resources/admin/plugins/angularjs/angular-animate.js",
            //    "~/Resources/admin/plugins/angularjs/ui-bootstrap-tpls-0.12.0.min.js",
            //    "~/Resources/admin/plugins/angularjs/apps.js"
            //    );
            //angularbudle.Orderer = new NonOrderingBundleOrderer();
            //bundles.Add(angularbudle);
            ////BundleTable.EnableOptimizations = true;
          
        }
    }

    internal class NonOrderingBundleOrderer : IBundleOrderer
    {
        public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
        {
            return files;
        }
    }
}
