﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BetterCms.Module.Installation.Views.Shared
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    
    #line 2 "..\..\Views\Shared\WideLayout.cshtml"
    using BetterCms.Module.Installation;
    
    #line default
    #line hidden
    
    #line 1 "..\..\Views\Shared\WideLayout.cshtml"
    using BetterCms.Module.Root.Mvc.Helpers;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Shared/WideLayout.cshtml")]
    public partial class WideLayout : System.Web.Mvc.WebViewPage<BetterCms.Core.DataContracts.IPage>
    {
        public WideLayout()
        {
        }
        public override void Execute()
        {
            
            #line 4 "..\..\Views\Shared\WideLayout.cshtml"
  
    Layout = "~/Areas/bcms-Root/Views/Shared/BaseLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Styles", () => {

WriteLiteral("    \r\n");

WriteLiteral("    ");

            
            #line 9 "..\..\Views\Shared\WideLayout.cshtml"
Write(RenderSection("Styles", false));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 10 "..\..\Views\Shared\WideLayout.cshtml"
Write(Html.RenderStyleSheets<InstallationModuleDescriptor>());

            
            #line default
            #line hidden
WriteLiteral("\r\n");

});

WriteLiteral("\r\n");

DefineSection("HeadScripts", () => {

WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 14 "..\..\Views\Shared\WideLayout.cshtml"
Write(RenderSection("HeadScripts", false));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

});

WriteLiteral("\r\n<div");

WriteLiteral(" class=\"page\"");

WriteLiteral(">\r\n\r\n    <header");

WriteLiteral(" class=\"page-header\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"page-frame clearfix\"");

WriteLiteral(">\r\n            <div>\r\n");

WriteLiteral("                ");

            
            #line 22 "..\..\Views\Shared\WideLayout.cshtml"
           Write(RenderSection("CMSHeader", false));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n        </div>\r\n    </header>\r\n\r\n    <div");

WriteLiteral(" class=\"page-block\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"page-frame clearfix\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 29 "..\..\Views\Shared\WideLayout.cshtml"
       Write(RenderSection("CMSMainContent", false));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 30 "..\..\Views\Shared\WideLayout.cshtml"
       Write(RenderBody());

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n\r\n</div>\r\n\r\n<footer");

WriteLiteral(" class=\"page-footer\"");

WriteLiteral(">\r\n    <div");

WriteLiteral(" class=\"page-frame clearfix\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 38 "..\..\Views\Shared\WideLayout.cshtml"
   Write(RenderSection("CMSFooter", false));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n</footer>\r\n");

        }
    }
}
#pragma warning restore 1591
