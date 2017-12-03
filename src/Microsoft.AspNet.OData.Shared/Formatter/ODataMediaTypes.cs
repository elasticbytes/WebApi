// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Microsoft.AspNet.OData.Formatter
{
    /// <summary>
    /// Contains media types used by the OData formatter.
    /// </summary>
    internal static class ODataMediaTypes
    {
        public static readonly string ApplicationJsonType = "application";
        public static readonly string ApplicationJsonSubtype = "json";
        public static readonly string ApplicationJson = "application/json";
        public static readonly string ApplicationJsonODataFullMetadata = "application/json; odata.metadata=full";
        public static readonly string ApplicationJsonODataFullMetadataStreamingFalse = "application/json; odata.metadata=full; odata.streaming=false";
        public static readonly string ApplicationJsonODataFullMetadataStreamingTrue = "application/json; odata.metadata=full; odata.streaming=true";
        public static readonly string ApplicationJsonODataMinimalMetadata = "application/json; odata.metadata=minimal";
        public static readonly string ApplicationJsonODataMinimalMetadataStreamingFalse = "application/json; odata.metadata=minimal; odata.streaming=false";
        public static readonly string ApplicationJsonODataMinimalMetadataStreamingTrue = "application/json; odata.metadata=minimal; odata.streaming=true";
        public static readonly string ApplicationJsonODataNoMetadata = "application/json; odata.metadata=none";
        public static readonly string ApplicationJsonODataNoMetadataStreamingFalse = "application/json; odata.metadata=none; odata.streaming=false";
        public static readonly string ApplicationJsonODataNoMetadataStreamingTrue = "application/json; odata.metadata=none; odata.streaming=true";
        public static readonly string ApplicationJsonStreamingFalse = "application/json; odata.streaming=false";
        public static readonly string ApplicationJsonStreamingTrue = "application/json; odata.streaming=true";
        public static readonly string ApplicationXml = "application/xml";

        public static ODataMetadataLevel GetMetadataLevel(ref MediaType mediaType)
        {
            if (!Equals(ApplicationJsonType, mediaType.Type, StringComparison.Ordinal) ||
                !Equals(ApplicationJsonSubtype, mediaType.SubType, StringComparison.Ordinal))
            {
                return ODataMetadataLevel.MinimalMetadata;
            }

            var odataParameter = mediaType.GetParameter ("odata.metadata");
            if (odataParameter.HasValue)
            {
                if (Equals("full", odataParameter, StringComparison.OrdinalIgnoreCase))
                {
                    return ODataMetadataLevel.FullMetadata;
                }
                if (Equals("none", odataParameter, StringComparison.OrdinalIgnoreCase))
                {
                    return ODataMetadataLevel.NoMetadata;
                }
            }

            // Minimal is the default metadata level
            return ODataMetadataLevel.MinimalMetadata;
        }

        private static bool Equals (string value, Microsoft.Extensions.Primitives.StringSegment segment, StringComparison comparison)
        {
            return segment.HasValue && segment.Length == value.Length &&
                String.Compare (segment.Buffer, segment.Offset, value, 0, value.Length, comparison) == 0;
        }
    }
}
