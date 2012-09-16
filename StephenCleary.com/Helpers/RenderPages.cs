using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.WebPages;

namespace StephenCleary.Helpers
{
    public static class RenderPagesExtension
    {
        /// <summary>
        /// Renders all *.cshtml pages under a specified directory.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="path">The directory to search.</param>
        /// <param name="data">Optional data to pass to the pages.</param>
        public static HtmlString RenderPages(this WebPageBase @this, string path, params object[] data)
        {
            var sb = new StringBuilder();
            var pageDirectory = Path.GetDirectoryName(@this.Request.MapPath(@this.VirtualPath));
            var dir = Path.Combine(pageDirectory, path);
            foreach (var page in Directory.EnumerateFiles(dir, "*.cshtml", SearchOption.AllDirectories).OrderBy(x => x))
            {
                var relativePath = MakeRelative(pageDirectory, page);
                sb.Append(@this.RenderPage(relativePath, data).ToString());
            }

            return new HtmlString(sb.ToString());
        }

        private static string MakeRelative(string root, string path)
        {
            string ret = string.Empty;
            while (path != root)
            {
                ret = Path.Combine(Path.GetFileName(path), ret);
                path = Path.GetDirectoryName(path);
            }

            return ret;
        }
    }
}