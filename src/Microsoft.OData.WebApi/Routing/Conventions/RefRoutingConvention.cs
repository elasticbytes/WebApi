﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.OData.WebApi.Common;
using Microsoft.OData.WebApi.Interfaces;

namespace Microsoft.OData.WebApi.Routing.Conventions
{
    /// <summary>
    /// An implementation of <see cref="IODataRoutingConvention"/> that handles entity reference manipulations.
    /// </summary>
    public class RefRoutingConvention : NavigationSourceRoutingConvention
    {
        private const string DeleteRefActionNamePrefix = "DeleteRef";
        private const string CreateRefActionNamePrefix = "CreateRef";
        private const string GetRefActionNamePrefix = "GetRef";

        /// <inheritdoc/>
        public override string SelectAction(ODataPath odataPath, IWebApiControllerContext controllerContext, IWebApiActionMatch actionMatch)
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
                throw Error.ArgumentNull("controllerContext");
            }

            string requestMethod = controllerContext.Request.Method;

            if (!IsSupportedRequestMethod(requestMethod))
            {
                return null;
            }

            if (odataPath.PathTemplate == "~/entityset/key/navigation/$ref" ||
                odataPath.PathTemplate == "~/entityset/key/cast/navigation/$ref" ||
                odataPath.PathTemplate == "~/singleton/navigation/$ref" ||
                odataPath.PathTemplate == "~/singleton/cast/navigation/$ref")
            {
                NavigationPropertyLinkSegment navigationLinkSegment = (NavigationPropertyLinkSegment)odataPath.Segments.Last();
                IEdmNavigationProperty navigationProperty = navigationLinkSegment.NavigationProperty;
                IEdmEntityType declaringType = navigationProperty.DeclaringEntityType();

                string refActionName = FindRefActionName(actionMatch, navigationProperty, declaringType, requestMethod);
                if (refActionName != null)
                {
                    if (odataPath.PathTemplate.StartsWith("~/entityset/key", StringComparison.Ordinal))
                    {
                        controllerContext.AddKeyValueToRouteData((KeySegment)odataPath.Segments[1]);
                    }

                    controllerContext.RouteData.Add(ODataRouteConstants.NavigationProperty, navigationLinkSegment.NavigationProperty.Name);
                    return refActionName;
                }
            }
            else if ((HttpMethodHelper.IsDelete(requestMethod)) && (
                odataPath.PathTemplate == "~/entityset/key/navigation/key/$ref" ||
                odataPath.PathTemplate == "~/entityset/key/cast/navigation/key/$ref" ||
                odataPath.PathTemplate == "~/singleton/navigation/key/$ref" ||
                odataPath.PathTemplate == "~/singleton/cast/navigation/key/$ref"))
            {
                // the second key segment is the last segment in the path.
                // So the previous of last segment is the navigation property link segment.
                NavigationPropertyLinkSegment navigationLinkSegment = (NavigationPropertyLinkSegment)odataPath.Segments[odataPath.Segments.Count - 2];
                IEdmNavigationProperty navigationProperty = navigationLinkSegment.NavigationProperty;
                IEdmEntityType declaringType = navigationProperty.DeclaringEntityType();

                string refActionName = FindRefActionName(actionMatch, navigationProperty, declaringType, requestMethod);
                if (refActionName != null)
                {
                    if (odataPath.PathTemplate.StartsWith("~/entityset/key", StringComparison.Ordinal))
                    {
                        controllerContext.AddKeyValueToRouteData((KeySegment)odataPath.Segments[1]);
                    }

                    controllerContext.RouteData.Add(ODataRouteConstants.NavigationProperty, navigationLinkSegment.NavigationProperty.Name);
                    controllerContext.AddKeyValueToRouteData((KeySegment)odataPath.Segments.Last(e => e is KeySegment), ODataRouteConstants.RelatedKey);
                    return refActionName;
                }
            }

            return null;
        }

        private static string FindRefActionName(IWebApiActionMatch actionMatch,
            IEdmNavigationProperty navigationProperty, IEdmEntityType declaringType, string method)
        {
            string actionNamePrefix;
            if (HttpMethodHelper.IsDelete(method))
            {
                actionNamePrefix = DeleteRefActionNamePrefix;
            }
            else if (HttpMethodHelper.IsGet(method))
            {
                actionNamePrefix = GetRefActionNamePrefix;
            }
            else
            {
                actionNamePrefix = CreateRefActionNamePrefix;
            }

            // Examples: CreateRefToOrdersFromCustomer, CreateRefToOrders, CreateRef.
            return actionMatch.FindMatchingAction(
                        actionNamePrefix + "To" + navigationProperty.Name + "From" + declaringType.Name,
                        actionNamePrefix + "To" + navigationProperty.Name,
                        actionNamePrefix);
        }

        private static bool IsSupportedRequestMethod(string method)
        {
            return (HttpMethodHelper.IsDelete(method) ||
                HttpMethodHelper.IsPut(method) ||
                HttpMethodHelper.IsPost(method) ||
                HttpMethodHelper.IsGet(method));
        }
    }
}