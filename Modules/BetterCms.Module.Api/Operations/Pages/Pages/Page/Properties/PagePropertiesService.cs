﻿using System;
using System.Collections.Generic;
using System.Linq;

using BetterCms.Core.DataAccess;
using BetterCms.Core.DataAccess.DataContext;
using BetterCms.Core.DataAccess.DataContext.Fetching;
using BetterCms.Core.DataContracts.Enums;
using BetterCms.Core.Exceptions.DataTier;
using BetterCms.Core.Security;
using BetterCms.Module.Api.Helpers;
using BetterCms.Module.Api.Operations.Root;
using BetterCms.Module.MediaManager.Models;
using BetterCms.Module.MediaManager.Services;
using BetterCms.Module.Pages.Models;
using BetterCms.Module.Pages.Services;
using BetterCms.Module.Root.Models;
using BetterCms.Module.Root.Models.Extensions;
using BetterCms.Module.Root.Mvc;
using BetterCms.Module.Root.Mvc.Helpers;
using BetterCms.Module.Root.Services;

using ServiceStack.ServiceInterface;

using AccessLevel = BetterCms.Module.Api.Operations.Root.AccessLevel;
using ITagService = BetterCms.Module.Pages.Services.ITagService;

namespace BetterCms.Module.Api.Operations.Pages.Pages.Page.Properties
{
    /// <summary>
    /// Default page properties CRUD service.
    /// </summary>
    public class PagePropertiesService : Service, IPagePropertiesService
    {
        /// <summary>
        /// The repository.
        /// </summary>
        private readonly IRepository repository;

        /// <summary>
        /// The unit of work.
        /// </summary>
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// The URL service.
        /// </summary>
        private readonly IUrlService urlService;

        /// <summary>
        /// The option service.
        /// </summary>
        private readonly IOptionService optionService;

        /// <summary>
        /// The file URL resolver.
        /// </summary>
        private readonly IMediaFileUrlResolver fileUrlResolver;

        /// <summary>
        /// The tag service.
        /// </summary>
        private readonly ITagService tagService;

        /// <summary>
        /// The access control service.
        /// </summary>
        private readonly IAccessControlService accessControlService;

        /// <summary>
        /// The sitemap service.
        /// </summary>
        private readonly ISitemapService sitemapService;

        /// <summary>
        /// The master page service.
        /// </summary>
        private readonly IMasterPageService masterPageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PagePropertiesService" /> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="urlService">The URL service.</param>
        /// <param name="optionService">The option service.</param>
        /// <param name="fileUrlResolver">The file URL resolver.</param>
        /// <param name="tagService">The tag service.</param>
        /// <param name="accessControlService">The access control service.</param>
        /// <param name="sitemapService">The sitemap service.</param>
        /// <param name="masterPageService">The master page service.</param>
        public PagePropertiesService(
            IRepository repository,
            IUnitOfWork unitOfWork,
            IUrlService urlService,
            IOptionService optionService,
            IMediaFileUrlResolver fileUrlResolver,
            ITagService tagService,
            IAccessControlService accessControlService,
            ISitemapService sitemapService,
            IMasterPageService masterPageService)
        {
            this.repository = repository;
            this.unitOfWork = unitOfWork;
            this.urlService = urlService;
            this.optionService = optionService;
            this.fileUrlResolver = fileUrlResolver;
            this.tagService = tagService;
            this.accessControlService = accessControlService;
            this.sitemapService = sitemapService;
            this.masterPageService = masterPageService;
        }

        /// <summary>
        /// Gets the specified page properties.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///   <c>GetPagePropertiesResponse</c> with a page properties data.
        /// </returns>
        public GetPagePropertiesResponse Get(GetPagePropertiesRequest request)
        {
            var query = repository.AsQueryable<PageProperties>();

            if (request.PageId.HasValue)
            {
                query = query.Where(page => page.Id == request.PageId.Value);
            }
            else
            {
                var url = urlService.FixUrl(request.PageUrl);
                query = query.Where(page => page.PageUrlHash == url.UrlHash());
            }

            var response = query
                .Select(page => new GetPagePropertiesResponse
                    {
                        Data = new PagePropertiesModel
                            {
                                Id = page.Id,
                                Version = page.Version,
                                CreatedBy = page.CreatedByUser,
                                CreatedOn = page.CreatedOn,
                                LastModifiedBy = page.ModifiedByUser,
                                LastModifiedOn = page.ModifiedOn,

                                PageUrl = page.PageUrl,
                                Title = page.Title,
                                Description = page.Description,
                                IsPublished = page.Status == PageStatus.Published,
                                PublishedOn = page.PublishedOn,
                                LayoutId = page.Layout != null && !page.Layout.IsDeleted ? page.Layout.Id : (Guid?)null,
                                MasterPageId = page.MasterPage != null && !page.MasterPage.IsDeleted ? page.MasterPage.Id : (Guid?)null,
                                CategoryId = page.Category != null && !page.Category.IsDeleted ? page.Category.Id : (Guid?)null,
                                IsArchived = page.IsArchived,
                                IsMasterPage = page.IsMasterPage,
                                LanguageGroupIdentifier = page.LanguageGroupIdentifier,
                                LanguageId = page.Language != null ? page.Language.Id : (Guid?)null,
                                MainImageId = page.Image != null && !page.Image.IsDeleted ? page.Image.Id : (Guid?)null,
                                FeaturedImageId = page.FeaturedImage != null && !page.FeaturedImage.IsDeleted ? page.FeaturedImage.Id : (Guid?)null,
                                SecondaryImageId = page.SecondaryImage != null && !page.SecondaryImage.IsDeleted ? page.SecondaryImage.Id : (Guid?)null,
                                CustomCss = page.CustomCss,
                                CustomJavaScript = page.CustomJS,
                                UseCanonicalUrl = page.UseCanonicalUrl,
                                UseNoFollow = page.UseNoFollow,
                                UseNoIndex = page.UseNoIndex
                            },
                        MetaData = request.Data.IncludeMetaData 
                            ? new MetadataModel
                            {
                                MetaTitle = page.MetaTitle,
                                MetaDescription = page.MetaDescription,
                                MetaKeywords = page.MetaKeywords
                            } 
                            : null,
                        Category = page.Category != null && !page.Category.IsDeleted && request.Data.IncludeCategory 
                            ? new CategoryModel
                            {
                                Id = page.Category.Id,
                                Version = page.Category.Version,
                                CreatedBy = page.Category.CreatedByUser,
                                CreatedOn = page.Category.CreatedOn,
                                LastModifiedBy = page.Category.ModifiedByUser,
                                LastModifiedOn = page.Category.ModifiedOn,
                                Name = page.Category.Name
                            }
                            : null,
                        Layout = request.Data.IncludeLayout && !page.Layout.IsDeleted 
                            ? new LayoutModel
                            {
                                Id = page.Layout.Id,
                                Version = page.Layout.Version,
                                CreatedBy = page.Layout.CreatedByUser,
                                CreatedOn = page.Layout.CreatedOn,
                                LastModifiedBy = page.Layout.ModifiedByUser,
                                LastModifiedOn = page.Layout.ModifiedOn,

                                Name = page.Layout.Name,
                                LayoutPath = page.Layout.LayoutPath,
                                PreviewUrl = page.Layout.PreviewUrl
                            } 
                            : null,
                        MainImage = page.Image != null && !page.Image.IsDeleted && request.Data.IncludeImages 
                            ? new ImageModel
                            {
                                Id = page.Image.Id,
                                Version = page.Image.Version,
                                CreatedBy = page.Image.CreatedByUser,
                                CreatedOn = page.Image.CreatedOn,
                                LastModifiedBy = page.Image.ModifiedByUser,
                                LastModifiedOn = page.Image.ModifiedOn,

                                Title = page.Image.Title,
                                Caption = page.Image.Caption,
                                Url = fileUrlResolver.EnsureFullPathUrl(page.Image.PublicUrl),
                                ThumbnailUrl = fileUrlResolver.EnsureFullPathUrl(page.Image.PublicThumbnailUrl)
                            } 
                            : null,
                        FeaturedImage = page.FeaturedImage != null && !page.FeaturedImage.IsDeleted && request.Data.IncludeImages 
                            ? new ImageModel
                            {
                                Id = page.FeaturedImage.Id,
                                Version = page.FeaturedImage.Version,
                                CreatedBy = page.FeaturedImage.CreatedByUser,
                                CreatedOn = page.FeaturedImage.CreatedOn,
                                LastModifiedBy = page.FeaturedImage.ModifiedByUser,
                                LastModifiedOn = page.FeaturedImage.ModifiedOn,

                                Title = page.FeaturedImage.Title,
                                Caption = page.FeaturedImage.Caption,
                                Url = fileUrlResolver.EnsureFullPathUrl(page.FeaturedImage.PublicUrl),
                                ThumbnailUrl = fileUrlResolver.EnsureFullPathUrl(page.FeaturedImage.PublicThumbnailUrl)
                            } 
                            : null,
                        SecondaryImage = page.SecondaryImage != null && !page.SecondaryImage.IsDeleted && request.Data.IncludeImages 
                            ? new ImageModel
                            {
                                Id = page.SecondaryImage.Id,
                                Version = page.SecondaryImage.Version,
                                CreatedBy = page.SecondaryImage.CreatedByUser,
                                CreatedOn = page.SecondaryImage.CreatedOn,
                                LastModifiedBy = page.SecondaryImage.ModifiedByUser,
                                LastModifiedOn = page.SecondaryImage.ModifiedOn,

                                Title = page.SecondaryImage.Title,
                                Caption = page.SecondaryImage.Caption,
                                Url = fileUrlResolver.EnsureFullPathUrl(page.SecondaryImage.PublicUrl),
                                ThumbnailUrl = fileUrlResolver.EnsureFullPathUrl(page.SecondaryImage.PublicThumbnailUrl)
                            } 
                            : null,
                        Language = page.Language != null && !page.Language.IsDeleted && request.Data.IncludeLanguage
                            ? new LanguageModel
                            {
                                Id = page.Language.Id,
                                Version = page.Language.Version,
                                CreatedBy = page.Language.CreatedByUser,
                                CreatedOn = page.Language.CreatedOn,
                                LastModifiedBy = page.Language.ModifiedByUser,
                                LastModifiedOn = page.Language.ModifiedOn,

                                Name = page.Language.Name,
                                Code = page.Language.Code,
                            }
                            : null,
                    })
                .FirstOne();

            if (request.Data.IncludeTags)
            {
                response.Tags = LoadTags(response.Data.Id);
            }

            if (request.Data.IncludePageContents)
            {
                response.PageContents = LoadPageContents(response.Data.Id);
            }

            if (request.Data.IncludePageOptions)
            {
                // Get layout options, page options and merge them
                response.PageOptions = optionService
                    .GetMergedMasterPagesOptionValues(response.Data.Id, response.Data.MasterPageId, response.Data.LayoutId)
                    .Select(o => new OptionModel
                        {
                            Key = o.OptionKey,
                            Value = o.OptionValue,
                            DefaultValue = o.OptionDefaultValue,
                            Type = (Root.OptionType)(int)o.Type
                        })
                    .ToList();
            }
            
            if (request.Data.IncludeAccessRules)
            {
                // Get layout options, page options and merge them
                response.AccessRules = LoadAccessRules(response.Data.Id);
            }
            
            if (request.Data.IncludePageTranslations 
                && response.Data.LanguageGroupIdentifier.HasValue)
            {
                // Get layout options, page options and merge them
                response.PageTranslations = repository
                    .AsQueryable<PageProperties>()
                    .Where(p => p.LanguageGroupIdentifier == response.Data.LanguageGroupIdentifier)
                    .OrderBy(p => p.Title)
                    .Select(p => new PageTranslationModel
                        {
                            Id = p.Id,
                            Title = p.Title,
                            PageUrl = p.PageUrl,
                            LanguageId = p.Language != null ? p.Language.Id : (Guid?)null,
                            LanguageCode = p.Language != null ? p.Language.Code : null,
                        })
                    .ToList();
            }

            return response;
        }

        /// <summary>
        /// Replaces the page properties or if it doesn't exist, creates it.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///   <c>PutPagePropertiesResponse</c> with a page properties id.
        /// </returns>
        public PutPagePropertiesResponse Put(PutPagePropertiesRequest request)
        {
            var pageProperties =
                repository.AsQueryable<PageProperties>(e => e.Id == request.PageId)
                    .FetchMany(p => p.Options)
                    .Fetch(p => p.Layout)
                    .ThenFetchMany(l => l.LayoutOptions)
                    .FetchMany(p => p.MasterPages)
                    .FetchMany(f => f.AccessRules)
                    .ToList()
                    .FirstOrDefault();

            var createPageProperties = pageProperties == null;
            if (createPageProperties)
            {
                pageProperties = new PageProperties { Id = request.Data.Id, AccessRules = new List<AccessRule>() };
            }
            else
            {
                pageProperties.Version = request.Data.Version;
            }

            // Load master pages for updating page's master path and page's children master path
            IList<Guid> newMasterIds;
            IList<Guid> oldMasterIds;
            IList<Guid> childrenPageIds;
            IList<MasterPage> existingChildrenMasterPages;
            masterPageService.PrepareForUpdateChildrenMasterPages(pageProperties, request.Data.MasterPageId, out newMasterIds, out oldMasterIds, out childrenPageIds, out existingChildrenMasterPages);

            unitOfWork.BeginTransaction();

            pageProperties.PageUrl = request.Data.PageUrl;
            pageProperties.PageUrlHash = request.Data.PageUrl.UrlHash();
            pageProperties.Title = request.Data.Title;
            pageProperties.Description = request.Data.Description;
            pageProperties.Status = request.Data.IsMasterPage || request.Data.IsPublished ? PageStatus.Published : PageStatus.Unpublished;
            pageProperties.PublishedOn = request.Data.PublishedOn;

            masterPageService.SetMasterOrLayout(pageProperties, request.Data.MasterPageId, request.Data.LayoutId);

            pageProperties.Category = request.Data.CategoryId.HasValue
                                    ? repository.AsProxy<Category>(request.Data.CategoryId.Value)
                                    : null;
            pageProperties.IsArchived = request.Data.IsArchived;
            pageProperties.IsMasterPage = request.Data.IsMasterPage;
            pageProperties.LanguageGroupIdentifier = request.Data.LanguageGroupIdentifier;
            pageProperties.Language = request.Data.LanguageId.HasValue && !request.Data.LanguageId.Value.HasDefaultValue()
                                    ? repository.AsProxy<Language>(request.Data.LanguageId.Value)
                                    : null;

            pageProperties.Image = request.Data.MainImageId.HasValue
                                    ? repository.AsProxy<MediaImage>(request.Data.MainImageId.Value)
                                    : null;
            pageProperties.FeaturedImage = request.Data.FeaturedImageId.HasValue
                                    ? repository.AsProxy<MediaImage>(request.Data.FeaturedImageId.Value)
                                    : null;
            pageProperties.SecondaryImage = request.Data.SecondaryImageId.HasValue
                                    ? repository.AsProxy<MediaImage>(request.Data.SecondaryImageId.Value)
                                    : null;

            pageProperties.CustomCss = request.Data.CustomCss;
            pageProperties.CustomJS = request.Data.CustomJavaScript;
            pageProperties.UseCanonicalUrl = request.Data.UseCanonicalUrl;
            pageProperties.UseNoFollow = request.Data.UseNoFollow;
            pageProperties.UseNoIndex = request.Data.UseNoIndex;

            if (request.Data.MetaData != null)
            {
                pageProperties.MetaTitle = request.Data.MetaData.MetaTitle;
                pageProperties.MetaDescription = request.Data.MetaData.MetaDescription;
                pageProperties.MetaKeywords = request.Data.MetaData.MetaKeywords;
            }

            IList<Tag> newTags = null;
            if (request.Data.Tags != null)
            {
                var tags = request.Data.Tags.Select(t => t.Name).ToList();
                tagService.SavePageTags(pageProperties, tags, out newTags);
            }

            if (request.Data.AccessRules != null)
            {
                pageProperties.AccessRules.RemoveDuplicateEntities();
                var accessRules =
                    request.Data.AccessRules.Select(
                        r => (IAccessRule)new AccessRule { AccessLevel = (Core.Security.AccessLevel)(int)r.AccessLevel, Identity = r.Identity, IsForRole = r.IsForRole })
                        .ToList();
                accessControlService.UpdateAccessControl(pageProperties, accessRules);
            }

            repository.Save(pageProperties);

            unitOfWork.Commit();

            masterPageService.UpdateChildrenMasterPages(existingChildrenMasterPages, oldMasterIds, newMasterIds, childrenPageIds);

            // Fire events.
            Events.RootEvents.Instance.OnTagCreated(newTags);
            if (createPageProperties)
            {
                Events.PageEvents.Instance.OnPageCreated(pageProperties);
            }
            else
            {
                Events.PageEvents.Instance.OnPagePropertiesChanged(pageProperties);
            }

            return new PutPagePropertiesResponse
            {
                Data = pageProperties.Id
            };
        }

        /// <summary>
        /// Deletes the specified page properties.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///   <c>DeletePagePropertiesResponse</c> with success status.
        /// </returns>
        public DeletePagePropertiesResponse Delete(DeletePagePropertiesRequest request)
        {
            if (request.Data == null || request.Data.Id.HasDefaultValue())
            {
                return new DeletePagePropertiesResponse { Data = false };
            }

            var page = repository.First<PageProperties>(request.Data.Id);
            if (page.Version != request.Data.Version)
            {
                throw new ConcurrentDataException(page);
            }

            var sitemaps = new Dictionary<Module.Pages.Models.Sitemap, bool>();
            var sitemapNodes = sitemapService.GetNodesByPage(page);

            unitOfWork.BeginTransaction();

            IList<SitemapNode> updatedNodes = new List<SitemapNode>();
            IList<SitemapNode> deletedNodes = new List<SitemapNode>();
            if (sitemapNodes != null)
            {
                // Archive sitemaps before update.
                sitemaps.Select(pair => pair.Key).ToList().ForEach(sitemap => sitemapService.ArchiveSitemap(sitemap.Id));
                foreach (var node in sitemapNodes)
                {
                    if (!node.IsDeleted)
                    {
                        // Unlink sitemap node.
                        if (node.Page != null && node.Page.Id == page.Id)
                        {
                            node.Page = null;
                            node.Title = node.UsePageTitleAsNodeTitle ? page.Title : node.Title;
                            node.Url = page.PageUrl;
                            node.UrlHash = page.PageUrlHash;
                            repository.Save(node);
                            updatedNodes.Add(node);
                        }
                    }
                }
            }

            // Delete child entities.            
            if (page.PageTags != null)
            {
                foreach (var pageTag in page.PageTags)
                {
                    repository.Delete(pageTag);
                }
            }

            if (page.PageContents != null)
            {
                foreach (var pageContent in page.PageContents)
                {
                    repository.Delete(pageContent);
                }
            }

            if (page.Options != null)
            {
                foreach (var option in page.Options)
                {
                    repository.Delete(option);
                }
            }

            if (page.AccessRules != null)
            {
                var rules = page.AccessRules.ToList();
                rules.ForEach(page.RemoveRule);
            }

            if (page.MasterPages != null)
            {
                foreach (var master in page.MasterPages)
                {
                    repository.Delete(master);
                }
            }

            // Delete page
            repository.Delete<Module.Root.Models.Page>(request.Data.Id, request.Data.Version);

            unitOfWork.Commit();

            var updatedSitemaps = new List<Module.Pages.Models.Sitemap>();
            foreach (var node in updatedNodes)
            {
                Events.SitemapEvents.Instance.OnSitemapNodeUpdated(node);
                if (!updatedSitemaps.Contains(node.Sitemap))
                {
                    updatedSitemaps.Add(node.Sitemap);
                }
            }

            foreach (var node in deletedNodes)
            {
                Events.SitemapEvents.Instance.OnSitemapNodeDeleted(node);
                if (!updatedSitemaps.Contains(node.Sitemap))
                {
                    updatedSitemaps.Add(node.Sitemap);
                }
            }

            foreach (var updatedSitemap in updatedSitemaps)
            {
                Events.SitemapEvents.Instance.OnSitemapUpdated(updatedSitemap);
            }

            Events.PageEvents.Instance.OnPageDeleted(page);

            return new DeletePagePropertiesResponse { Data = true };
        }

        /// <summary>
        /// Loads the access rules.
        /// </summary>
        /// <param name="pageId">The page identifier.</param>
        /// <returns>Page access rules collection.</returns>
        private List<AccessRuleModel> LoadAccessRules(Guid pageId)
        {
            return (from page in repository.AsQueryable<Module.Root.Models.Page>()
                    from accessRule in page.AccessRules
                    where page.Id == pageId
                    orderby accessRule.IsForRole, accessRule.Identity
                    select new AccessRuleModel
                    {
                        AccessLevel = (AccessLevel)(int)accessRule.AccessLevel,
                        Identity = accessRule.Identity,
                        IsForRole = accessRule.IsForRole
                    })
                    .ToList();
        }

        /// <summary>
        /// Loads the tags.
        /// </summary>
        /// <param name="pageId">The page identifier.</param>
        /// <returns>Page tags collection.</returns>
        private List<TagModel> LoadTags(Guid pageId)
        {
            return repository
                .AsQueryable<PageTag>(pageTag => pageTag.Page.Id == pageId && !pageTag.Tag.IsDeleted)
                .Select(media => new TagModel
                {
                    Id = media.Tag.Id,
                    Version = media.Tag.Version,
                    CreatedBy = media.Tag.CreatedByUser,
                    CreatedOn = media.Tag.CreatedOn,
                    LastModifiedBy = media.Tag.ModifiedByUser,
                    LastModifiedOn = media.Tag.ModifiedOn,

                    Name = media.Tag.Name
                }).ToList();
        }

        /// <summary>
        /// Loads the page contents.
        /// </summary>
        /// <param name="pageId">The page identifier.</param>
        /// <returns>Page contents collection.</returns>
        private List<PageContentModel> LoadPageContents(Guid pageId)
        {
            var results = repository
                 .AsQueryable<PageContent>(pageContent => pageContent.Page.Id == pageId && !pageContent.Content.IsDeleted)
                 .OrderBy(pageContent => pageContent.Order)
                 .Select(pageContent => new
                    {
                        Type = pageContent.Content.GetType(),
                        Model = new PageContentModel
                            {
                                Id = pageContent.Id,
                                Version = pageContent.Version,
                                CreatedBy = pageContent.CreatedByUser,
                                CreatedOn = pageContent.CreatedOn,
                                LastModifiedBy = pageContent.ModifiedByUser,
                                LastModifiedOn = pageContent.ModifiedOn,

                                ContentId = pageContent.Content.Id,
                                Name = pageContent.Content.Name,
                                RegionId = pageContent.Region.Id,
                                RegionIdentifier = pageContent.Region.RegionIdentifier,
                                Order = pageContent.Order,
                                IsPublished = pageContent.Content.Status == ContentStatus.Published
                            }
                    }).ToList();

            // Set content types
            results.ToList().ForEach(item => item.Model.ContentType = item.Type.ToContentTypeString());

            return results.Select(item => item.Model).ToList();
        }
    }
}