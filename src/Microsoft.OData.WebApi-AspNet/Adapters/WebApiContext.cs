﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Web.Http;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using Microsoft.OData.WebApi.Common;
using Microsoft.OData.WebApi.Extensions;
using Microsoft.OData.WebApi.Interfaces;
using Microsoft.OData.WebApi.Routing.Conventions;
using ODataPath = Microsoft.OData.WebApi.Routing.ODataPath;

namespace Microsoft.OData.WebApi.Adapters
{
    /// <summary>
    /// Adapter class to convert Asp.Net WebApi OData properties to OData WebApi.
    /// </summary>
    public class WebApiContext : IWebApiContext
    {
        /// <summary>
        /// The inner context wrapped by this instance.
        /// </summary>
        private HttpRequestMessageProperties innerContext;

        /// <summary>
        /// Initializes a new instance of the WebApiContext class.
        /// </summary>
        /// <param name="context">The inner context.</param>
        public WebApiContext(HttpRequestMessageProperties context)
        {
            if (context == null)
            {
                throw Error.ArgumentNull("context");
            }

            this.innerContext = context;
        }

        /// <summary>
        /// Gets or sets the parsed OData <see cref="ApplyClause"/> of the request.
        /// </summary>
        public ApplyClause ApplyClause
        {
            get { return this.innerContext.ApplyClause; }
            set { this.innerContext.ApplyClause = value; }
        }

        /// <summary>
        /// Get the type of an Http error.
        /// </summary>
        public Type HttpErrorType
        {
            get { return typeof(HttpError); }
        }

        /// <summary>
        /// Gets or sets the next link for the OData response.
        /// </summary>
        public Uri NextLink
        {
            get { return this.innerContext.NextLink; }
            set { this.innerContext.NextLink = value; }
        }

        /// <summary>
        /// Gets or sets the OData path.
        /// </summary>
        public ODataPath Path
        {
            get { return this.innerContext.Path; }
        }

        /// <summary>
        /// Gets the route name for generating OData links.
        /// </summary>
        public string RouteName
        {
            get { return this.innerContext.RouteName; }
        }

        /// <summary>
        /// Gets the data store used by <see cref="IODataRoutingConvention"/>s to store any custom route data.
        /// </summary>
        /// <value>Initially an empty <c>IDictionary&lt;string, object&gt;</c>.</value>
        public IDictionary<string, object> RoutingConventionsStore
        {
            get { return this.innerContext.RoutingConventionsStore; }
        }

        /// <summary>
        /// Gets or sets the parsed OData <see cref="SelectExpandClause"/> of the request.
        /// </summary>
        public SelectExpandClause SelectExpandClause
        {
            get { return this.innerContext.SelectExpandClause; }
            set { this.innerContext.SelectExpandClause = value; }
        }

        /// <summary>
        /// Gets or sets the total count for the OData response.
        /// </summary>
        /// <value><c>null</c> if no count should be sent back to the client.</value>
        public long? TotalCount
        {
            get { return this.innerContext.TotalCount; }
        }

        /// <summary>
        /// Gets or sets the total count function for the OData response.
        /// </summary>
        public Func<long> TotalCountFunc
        {
            get { return this.innerContext.TotalCountFunc; }
            set { this.innerContext.TotalCountFunc = value; }
        }
    }
}