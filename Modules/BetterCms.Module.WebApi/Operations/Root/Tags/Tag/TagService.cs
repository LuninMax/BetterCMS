﻿using System.Linq;

using BetterCms.Api;
using BetterCms.Core;

using ServiceStack.ServiceInterface;

namespace BetterCms.Module.Api.Operations.Root.Tags.Tag
{
    public class TagService : Service, ITagService
    {
        public GetTagResponse Get(GetTagRequest request)
        {
            using (var api = CmsContext.CreateApiContextOf<PagesApiContext>())
            {
                var request1 = new Module.Pages.Api.DataContracts.GetTagsRequest();
                if (request.TagId.HasValue)
                {
                    request1.Filter = t => t.Id == request.TagId;
                }
                else
                {
                    request1.Filter = t => t.Name == request.TagName;
                }

                var tag = api
                    .GetTags(request1)
                    .Items
                    .First();

                return new GetTagResponse
                {
                    Data = new TagModel
                    {
                        Id = tag.Id,
                        Version = tag.Version,
                        CreatedBy = tag.CreatedByUser,
                        CreatedOn = tag.CreatedOn,
                        LastModifiedBy = tag.ModifiedByUser,
                        LastModifiedOn = tag.ModifiedOn,
                        Name = tag.Name,
                    }
                };
            }

        }

        public PostTagResponse Put(PostTagRequest request)
        {
            return new PostTagResponse
            {
                Data = request.Id,
            };
        }

        PostTagResponse ITagService.Update(PostTagRequest request)
        {
            return Put(request);
        }

        // TODO: add delete action
    }
}