﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.OData.WebApi.Common;
using Microsoft.OData.WebApi.Formatter;
using Microsoft.OData.WebApi.Interfaces;

namespace Microsoft.OData.WebApi.Routing.Conventions
{
    /// <summary>
    /// An implementation of <see cref="IODataRoutingConvention"/> that handles dynamic properties for open type.
    /// </summary>
    public class DynamicPropertyRoutingConvention : NavigationSourceRoutingConvention
    {
        private readonly string _actionName = "DynamicProperty";

        /// <inheritdoc/>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity",
            Justification = "These are simple conversion function and cannot be split up.")]
        public override string SelectAction(ODataPath odataPath, IWebApiControllerContext controllerContext,
            IWebApiActionMap actionMap)
        {
            if (odataPath == null)
            {
                throw Error.ArgumentNull("odataPath");
            }

            if (controllerContext == null)
            {
                throw Error.ArgumentNull("controllerContext");
            }

            if (actionMap == null)
            {
                throw Error.ArgumentNull("actionMap");
            }

            string actionName = null;
            DynamicPathSegment dynamicPropertSegment = null;

            switch (odataPath.PathTemplate)
            {
                case "~/entityset/key/dynamicproperty":
                case "~/entityset/key/cast/dynamicproperty":
                case "~/singleton/dynamicproperty":
                case "~/singleton/cast/dynamicproperty":
                    dynamicPropertSegment = odataPath.Segments.Last() as DynamicPathSegment;
                    if (dynamicPropertSegment == null)
                    {
                        return null;
                    }

                    if (HttpMethodHelper.IsGet(controllerContext.Request.Method))
                    {
                        string actionNamePrefix = String.Format(CultureInfo.InvariantCulture, "Get{0}", _actionName);
                        actionName = actionMap.FindMatchingAction(actionNamePrefix);
                    }
                    break;
                case "~/entityset/key/property/dynamicproperty":
                case "~/entityset/key/cast/property/dynamicproperty":
                case "~/singleton/property/dynamicproperty":
                case "~/singleton/cast/property/dynamicproperty":
                    dynamicPropertSegment = odataPath.Segments.Last() as DynamicPathSegment;
                    if (dynamicPropertSegment == null)
                    {
                        return null;
                    }

                    PropertySegment propertyAccessSegment = odataPath.Segments[odataPath.Segments.Count - 2]
                            as PropertySegment;
                    if (propertyAccessSegment == null)
                    {
                        return null;
                    }

                    EdmComplexType complexType = propertyAccessSegment.Property.Type.Definition as EdmComplexType;
                    if (complexType == null)
                    {
                        return null;
                    }

                    if (HttpMethodHelper.IsGet(controllerContext.Request.Method))
                    {
                        string actionNamePrefix = String.Format(CultureInfo.InvariantCulture, "Get{0}", _actionName);
                        actionName = actionMap.FindMatchingAction(actionNamePrefix + "From" + propertyAccessSegment.Property.Name);
                    }
                    break;
                default: break;
            }

            if (actionName != null)
            {
                if (odataPath.PathTemplate.StartsWith("~/entityset/key", StringComparison.Ordinal))
                {
                    KeySegment keyValueSegment = (KeySegment)odataPath.Segments[1];
                    controllerContext.AddKeyValueToRouteData(keyValueSegment);
                }

                controllerContext.RouteData.Add(ODataRouteConstants.DynamicProperty, dynamicPropertSegment.Identifier);
                var key = ODataParameterValue.ParameterValuePrefix + ODataRouteConstants.DynamicProperty;
                var value = new ODataParameterValue(dynamicPropertSegment.Identifier, EdmLibHelpers.GetEdmPrimitiveTypeReferenceOrNull(typeof(string)));
                controllerContext.RouteData.Add(key, value);
                controllerContext.Request.Context.RoutingConventionsStore.Add(key, value);
                return actionName;
            }
            return null;
        }
    }
}