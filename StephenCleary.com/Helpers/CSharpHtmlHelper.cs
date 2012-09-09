using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace StephenCleary.Helpers
{
    public static class CSharpHtmlHelper
    {
        public static HtmlString CSharp(this HtmlHelper @this, string code)
        {
            return CSharpFormatter.CSharp(code);
        }
    }
}