﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Diagnostics.Contracts;
using Microsoft.AspNet.OData.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Adapters;
using Microsoft.AspNetCore.OData.Extensions;

namespace Microsoft.AspNet.OData.Query
{
    /// <summary>
    /// This defines a composite OData query options that can be used to perform query composition.
    /// Currently this only supports $filter, $orderby, $top, $skip, and $count.
    /// </summary>
    ////[ODataQueryParameterBinding]
    public partial class ODataQueryOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ODataQueryOptions"/> class based on the incoming request and some metadata information from
        /// the <see cref="ODataQueryContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="ODataQueryContext"/> which contains the <see cref="IEdmModel"/> and some type information.</param>
        /// <param name="request">The incoming request message.</param>
        /// <remarks>This signature uses types that are AspNetCore-specific.</remarks>
        public ODataQueryOptions(ODataQueryContext context, HttpRequest request)
        {
            if (context == null)
            {
                throw Error.ArgumentNull("context");
            }

            if (request == null)
            {
                throw Error.ArgumentNull("request");
            }

            // Set the request container into context
            Contract.Assert(context.RequestContainer == null);
            context.RequestContainer = request.GetRequestContainer();

            // Remember the context and request
            Context = context;
            Request = request;
            InternalRequest = new WebApiRequestMessage(request);
            InternalHeaders = new WebApiRequestHeaders(request.Headers);

            // Complete initialization.
            Initialize(context);
        }

        /// <summary>
        /// Gets the request message associated with this instance.
        /// </summary>
        /// <remarks>This signature uses types that are AspNetCore-specific.</remarks>
        public HttpRequest Request { get; private set; }
    }
}