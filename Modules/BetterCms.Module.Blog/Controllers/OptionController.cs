﻿using System.Web.Mvc;

using BetterCms.Core.Security;
using BetterCms.Module.Blog.Commands.GetTemplatesList;
using BetterCms.Module.Blog.Commands.SaveDefaultTemplate;
using BetterCms.Module.Blog.ViewModels.Blog;
using BetterCms.Module.Root;
using BetterCms.Module.Root.Mvc;

using Microsoft.Web.Mvc;

namespace BetterCms.Module.Blog.Controllers
{
    /// <summary>
    /// Manage blogs options.
    /// </summary>
    [BcmsAuthorize(RootModuleConstants.UserRoles.EditContent)]
    [ActionLinkArea(BlogModuleDescriptor.BlogAreaName)]
    public class OptionController : CmsControllerBase
    {
        /// <summary>
        /// Gets templates list.
        /// </summary>
        /// <returns>Template list.</returns>
        public ActionResult Templates()
        {
            var templates = GetCommand<GetTemplatesCommand>().ExecuteCommand(true);
            var view = RenderView("Templates");
            
            return ComboWireJson(templates != null, view, templates, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Saves the default template.
        /// </summary>
        /// <param name="templateId">The id.</param>
        /// <returns>Json result</returns>
        [HttpPost]
        public ActionResult SaveDefaultTemplate(string templateId, string masterPageId)
        {
            var request = new DefaultTemplateViewModel { TemplateId = templateId.ToGuidOrDefault(), MasterPageId = masterPageId.ToGuidOrDefault() };

            var response = GetCommand<SaveDefaultTemplateCommand>().ExecuteCommand(request);

            return WireJson(response);
        }
    }
}