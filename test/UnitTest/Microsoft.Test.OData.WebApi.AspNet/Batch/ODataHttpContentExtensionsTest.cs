﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Net.Http;
using System.Text;
using System.Threading;
using Microsoft.OData;
using Microsoft.OData.WebApi.Batch;
using Microsoft.Test.OData.WebApi.TestCommon;

namespace Microsoft.Test.OData.WebApi.AspNet.Batch
{
    public class ODataHttpContentExtensionsTest
    {
        [Fact]
        public void GetODataMessageReaderAsync_NullContent_Throws()
        {
            Assert.ThrowsArgumentNull(
                () => ODataHttpContentExtensions.GetODataMessageReaderAsync(null, new ODataMessageReaderSettings(), CancellationToken.None)
                    .Wait(),
                "content");
        }

        [Fact]
        public void GetODataMessageReaderAsync_ReturnsMessageReader()
        {
            StringContent content = new StringContent("foo", Encoding.UTF8, "multipart/mixed");

            Assert.NotNull(content.GetODataMessageReaderAsync(new ODataMessageReaderSettings(), CancellationToken.None).Result);
        }
    }
}