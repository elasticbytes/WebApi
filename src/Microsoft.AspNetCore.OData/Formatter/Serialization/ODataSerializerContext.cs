﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.OData.Adapters;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Microsoft.AspNet.OData.Adapters;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.AspNet.OData.Formatter.Serialization
{
    /// <summary>
    /// Context information used by the <see cref="ODataSerializer"/> when serializing objects in OData message format.
    /// </summary>
    public partial class ODataSerializerContext
    {
        private HttpRequestMessage _request;
        private IUrlHelper _urlHelper;

        /// <summary>
        /// Gets or sets the HTTP Request whose response is being serialized.
        /// </summary>
        public HttpRequestMessage Request
        {
            get { return _request; }

            set
            {
                _request = value;
                //InternalRequest = _request != null ? new WebApiRequestMessage(_request) : null;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="UrlHelper"/> to use for generating OData links.
        /// </summary>
        public IUrlHelper Url
        {
            get { return _urlHelper; }

            set
            {
                _urlHelper = value;
                InternalUrl = _urlHelper != null ? new WebApiUrlHelper(_urlHelper) : null;
            }
        }
    }
}