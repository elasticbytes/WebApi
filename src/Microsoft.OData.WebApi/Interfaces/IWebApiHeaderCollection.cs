﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.OData.WebApi.Interfaces
{
    /// <summary>
    /// Represents the collection of Request Headers as defined in RFC 2616.
    /// </summary>
    public interface IWebApiHeaderCollection
    {
        /// <summary>
        /// Gets the value of the If-None-Match header for an HTTP request.
        /// </summary>
        IEnumerable<WebApiEntityTagHeaderValue> IfNoneMatch { get; }
        /// <summary>
        /// Gets the value of the If-Match header for an HTTP request.
        /// </summary>
        IEnumerable<WebApiEntityTagHeaderValue> IfMatch { get; }

        /// <summary>
        /// Return if a specified header and specified values are stored in the collection.
        /// </summary>
        /// <param name="key">The specified header.</param>
        /// <param name="values">The specified header values.</param>
        /// <returns>true is the specified header name and values are stored in the collection; otherwise false.</returns>
        bool TryGetValues(string key, out IEnumerable<string> values);
    }
}