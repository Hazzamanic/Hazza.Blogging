﻿using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using Orchard.Environment.Extensions;

// This code was generated by Orchardizer

namespace Hazza.Blogging.Handlers {
    [OrchardFeature("Hazza.Blogging.ContainerCommentCount")]
    public class ContainerCommentsCountPartHandler : ContentHandler {
        public ContainerCommentsCountPartHandler() {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
    }
}