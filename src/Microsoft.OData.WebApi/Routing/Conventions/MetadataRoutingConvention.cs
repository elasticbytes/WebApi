﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using Microsoft.OData.WebApi.Common;
using Microsoft.OData.WebApi.Interfaces;

namespace Microsoft.OData.WebApi.Routing.Conventions
{
    /// <summary>
    /// An implementation of <see cref="IODataRoutingConvention"/> that handles OData metadata requests.
    /// </summary>
    public class MetadataRoutingConvention : IODataRoutingConvention
    {
        /// <summary>
        /// Selects the controller for OData requests.
        /// </summary>
        /// <param name="odataPath">The OData path.</param>
        /// <param name="request">The request.</param>
        /// <returns>
        ///   <c>null</c> if the request isn't handled by this convention; otherwise, the name of the selected controller
        /// </returns>
        public SelectControllerResult SelectController(ODataPath odataPath, IWebApiRequestMessage request)
        {
            if (odataPath == null)
            {
                throw Error.ArgumentNull("odataPath");
            }

            if (request == null)
            {
                throw Error.ArgumentNull("request");
            }

            if (odataPath.PathTemplate == "~" ||
                odataPath.PathTemplate == "~/$metadata")
            {
                return new SelectControllerResult("Metadata");
            }

            return null;
        }

        /// <summary>
        /// Selects the action for OData requests.
        /// </summary>
        /// <param name="odataPath">The OData path.</param>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="actionMatch">The action matcher.</param>
        /// <returns>
        ///   <c>null</c> if the request isn't handled by this convention; otherwise, the name of the selected action
        /// </returns>
        public string SelectAction(ODataPath odataPath, IWebApiControllerContext controllerContext, IWebApiActionMatch actionMatch)
        {
            if (odataPath == null)
            {
                throw Error.ArgumentNull("odataPath");
            }

            if (controllerContext == null)
            {
                throw Error.ArgumentNull("controllerContext");
            }

            if (actionMatch == null)
            {
                throw Error.ArgumentNull("actionMatch");
            }

            if (odataPath.PathTemplate == "~")
            {
                return "GetServiceDocument";
            }

            if (odataPath.PathTemplate == "~/$metadata")
            {
                return "GetMetadata";
            }

            return null;
        }
    }
}