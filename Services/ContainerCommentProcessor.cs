using System;
using Hazza.Blogging.Models;
using Orchard.Comments.Models;
using Orchard.Comments.Services;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace Hazza.Blogging.Services {
    [OrchardFeature("Hazza.Blogging.ContainerCommentCount")]
    public class ContainerCommentProcessor : ICommentsCountProcessor {
        private readonly IContentManager _contentManager;
        private readonly ISessionLocator _sessionLocator;

        public ContainerCommentProcessor(IContentManager contentManager, ISessionLocator sessionLocator) {
            _contentManager = contentManager;
            _sessionLocator = sessionLocator;
        }

        public void Process(int commentsPartId) {
            var commentsPart = _contentManager.Get<CommentsPart>(commentsPartId);
            if (commentsPart == null)
                return;

            var common = commentsPart.As<CommonPart>();
            if (common == null)
                return;

            var container = common.Container;
            if (container == null)
                return;

            var containerComments = container.As<ContainerCommentsCountPart>();
            if (containerComments == null)
                return;

            var session = _sessionLocator.For(typeof(object));

            var q = session.CreateQuery(
                "select SUM(comments.CommentsCount) " +
                "from Orchard.ContentManagement.Records.ContentItemVersionRecord civ " +
                "join civ.ContentItemRecord ci " +
                "join ci.CommonPartRecord common " +
                "join ci.CommentsPartRecord comments " +
                "where common.Container = :containerid"
            );
            q.SetParameter("containerid", container.Id);
            var list = q.List();
            var count = (long)list[0];
            containerComments.Count = Convert.ToInt32(count);

            var pendingQuery = session.CreateQuery(@"select count(comment.Id)
                from Orchard.ContentManagement.Records.ContentItemVersionRecord civ
                join civ.ContentItemRecord ci 
                join ci.CommentPartRecord comment
                join comment.CommentsPartRecord comments
                join comments.ContentItemRecord cicomment
                join cicomment.CommonPartRecord common 
                where comment.Status = 'Pending' and common.Container = :containerid"
            );
            pendingQuery.SetParameter("containerid", container.Id);
            var pendingList = pendingQuery.List();
            var pendingCount = (long)pendingList[0];
            containerComments.PendingCount = Convert.ToInt32(pendingCount);
        }
    }
}