﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Microsoft.AspNet.OData.Extensions;
using Nuwa;
using WebStack.QA.Test.OData.Common;
using Xunit;
using Xunit.Extensions;

namespace WebStack.QA.Test.OData.ModelBoundQuerySettings.SelectAttributeTest
{
    public class SelectAttributeTest : NuwaTestBase
    {
        private const string CustomerBaseUrl = "{0}/enablequery/Customers";
        private const string OrderBaseUrl = "{0}/enablequery/Orders";
        private const string CarBaseUrl = "{0}/enablequery/Cars";
        private const string AutoSelectCustomerBaseUrl = "{0}/enablequery/AutoSelectCustomers";
        private const string ModelBoundCustomerBaseUrl = "{0}/modelboundapi/Customers";
        private const string ModelBoundOrderBaseUrl = "{0}/modelboundapi/Orders";
        private const string ModelBoundCarBaseUrl = "{0}/modelboundapi/Cars";
        private const string ModelBoundAutoSelectCustomerBaseUrl = "{0}/modelboundapi/AutoSelectCustomers";

        public SelectAttributeTest(NuwaClassFixture fixture)
            : base(fixture)
        {
        }

        [NuwaConfiguration]
        internal static void UpdateConfiguration(HttpConfiguration configuration)
        {
            configuration.Services.Replace(
                typeof (IAssembliesResolver),
                new TestAssemblyResolver(typeof(CustomersController), typeof(OrdersController),
                    typeof(CarsController), typeof(AutoSelectCustomersController)));
            configuration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling =
                Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            configuration.Expand();
            configuration.MapODataServiceRoute("enablequery", "enablequery",
                SelectAttributeEdmModel.GetEdmModel());
            configuration.MapODataServiceRoute("modelboundapi", "modelboundapi",
                SelectAttributeEdmModel.GetEdmModelByModelBoundAPI());
        }

        [NuwaTheory]
        [InlineData(CustomerBaseUrl + "?$select=*")]
        [InlineData(CustomerBaseUrl + "?$select=Id")]
        [InlineData(CustomerBaseUrl + "?$select=Id,Name")]
        [InlineData(ModelBoundCustomerBaseUrl + "?$select=*")]
        [InlineData(ModelBoundCustomerBaseUrl + "?$select=Id")]
        [InlineData(ModelBoundCustomerBaseUrl + "?$select=Id,Name")]
        public void NoSelectableByDefault(string url)
        {
            string queryUrl =
                string.Format(
                    url,
                    BaseAddress);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, queryUrl);
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json;odata.metadata=none"));
            HttpClient client = new HttpClient();

            HttpResponseMessage response = client.SendAsync(request).Result;
            string result = response.Content.ReadAsStringAsync().Result;

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("cannot be used in the $select query option.", result);
        }

        [NuwaTheory]
        [InlineData(OrderBaseUrl + "?$select=Name", (int)HttpStatusCode.OK)]
        [InlineData(OrderBaseUrl + "?$select=Id", (int)HttpStatusCode.BadRequest)]
        [InlineData(OrderBaseUrl + "?$select=Id,Name", (int)HttpStatusCode.BadRequest)]
        [InlineData(OrderBaseUrl +
            "/WebStack.QA.Test.OData.ModelBoundQuerySettings.SelectAttributeTest.SpecialOrder?$select=Name",
            (int)HttpStatusCode.BadRequest)]
        [InlineData(OrderBaseUrl +
            "/WebStack.QA.Test.OData.ModelBoundQuerySettings.SelectAttributeTest.SpecialOrder?$select=Id",
            (int)HttpStatusCode.BadRequest)]
        [InlineData(OrderBaseUrl +
            "/WebStack.QA.Test.OData.ModelBoundQuerySettings.SelectAttributeTest.SpecialOrder?$select=Price",
            (int)HttpStatusCode.OK)]
        [InlineData(OrderBaseUrl +
            "/WebStack.QA.Test.OData.ModelBoundQuerySettings.SelectAttributeTest.SpecialOrder?$select=SpecialName",
            (int)HttpStatusCode.OK)]
        [InlineData(OrderBaseUrl + "?$expand=Cars($select=Id,Name)", (int)HttpStatusCode.OK)]
        [InlineData(OrderBaseUrl + "?$expand=Cars($select=CarNumber)", (int)HttpStatusCode.BadRequest)]
        [InlineData(CarBaseUrl + "?$select=Id,Name", (int)HttpStatusCode.OK)]
        [InlineData(CarBaseUrl + "?$select=CarNumber", (int)HttpStatusCode.BadRequest)]
        [InlineData(ModelBoundOrderBaseUrl + "?$select=Name", (int)HttpStatusCode.OK)]
        [InlineData(ModelBoundOrderBaseUrl + "?$select=Id", (int)HttpStatusCode.BadRequest)]
        [InlineData(ModelBoundOrderBaseUrl + "?$select=Id,Name", (int)HttpStatusCode.BadRequest)]
        [InlineData(ModelBoundOrderBaseUrl +
            "/WebStack.QA.Test.OData.ModelBoundQuerySettings.SelectAttributeTest.SpecialOrder?$select=Name",
            (int)HttpStatusCode.BadRequest)]
        [InlineData(ModelBoundOrderBaseUrl +
            "/WebStack.QA.Test.OData.ModelBoundQuerySettings.SelectAttributeTest.SpecialOrder?$select=Id",
            (int)HttpStatusCode.BadRequest)]
        [InlineData(ModelBoundOrderBaseUrl +
            "/WebStack.QA.Test.OData.ModelBoundQuerySettings.SelectAttributeTest.SpecialOrder?$select=Price",
            (int)HttpStatusCode.OK)]
        [InlineData(ModelBoundOrderBaseUrl +
            "/WebStack.QA.Test.OData.ModelBoundQuerySettings.SelectAttributeTest.SpecialOrder?$select=SpecialName",
            (int)HttpStatusCode.OK)]
        [InlineData(ModelBoundOrderBaseUrl + "?$expand=Cars($select=Id,Name)", (int)HttpStatusCode.OK)]
        [InlineData(ModelBoundOrderBaseUrl + "?$expand=Cars($select=CarNumber)", (int)HttpStatusCode.BadRequest)]
        [InlineData(ModelBoundCarBaseUrl + "?$select=Id,Name", (int)HttpStatusCode.OK)]
        [InlineData(ModelBoundCarBaseUrl + "?$select=CarNumber", (int)HttpStatusCode.BadRequest)]
        public void SelectOnEntityType(string entitySetUrl, int statusCode)
        {
            string queryUrl =
                string.Format(
                    entitySetUrl,
                    BaseAddress);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, queryUrl);
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json;odata.metadata=none"));
            HttpClient client = new HttpClient();

            HttpResponseMessage response = client.SendAsync(request).Result;
            string result = response.Content.ReadAsStringAsync().Result;

            Assert.Equal(statusCode, (int)response.StatusCode);
            if (statusCode == (int)HttpStatusCode.BadRequest)
            {
                Assert.Contains("cannot be used in the $select query option.", result);
            }
        }

        [NuwaTheory]
        [InlineData(OrderBaseUrl + "?$expand=Customers($select=Id,Name)", (int)HttpStatusCode.OK)]
        [InlineData(OrderBaseUrl + "(1)/Customers?$select=Id,Name", (int)HttpStatusCode.OK)]
        [InlineData(CustomerBaseUrl + "?$expand=Orders($select=Name)", (int)HttpStatusCode.BadRequest)]
        [InlineData(CustomerBaseUrl + "(1)/Orders?$select=Name", (int)HttpStatusCode.BadRequest)]
        [InlineData(CustomerBaseUrl + "?$expand=Orders($select=Id)", (int)HttpStatusCode.OK)]
        [InlineData(CustomerBaseUrl + "(1)/Orders?$select=Id", (int)HttpStatusCode.OK)]
        [InlineData(ModelBoundOrderBaseUrl + "?$expand=Customers($select=Id,Name)", (int)HttpStatusCode.OK)]
        [InlineData(ModelBoundOrderBaseUrl + "(1)/Customers?$select=Id,Name", (int)HttpStatusCode.OK)]
        [InlineData(ModelBoundCustomerBaseUrl + "?$expand=Orders($select=Name)", (int)HttpStatusCode.BadRequest)]
        [InlineData(ModelBoundCustomerBaseUrl + "(1)/Orders?$select=Name", (int)HttpStatusCode.BadRequest)]
        [InlineData(ModelBoundCustomerBaseUrl + "?$expand=Orders($select=Id)", (int)HttpStatusCode.OK)]
        [InlineData(ModelBoundCustomerBaseUrl + "(1)/Orders?$select=Id", (int)HttpStatusCode.OK)]
        public void SelectOnProperty(string entitySetUrl, int statusCode)
        {
            string queryUrl =
                string.Format(
                    entitySetUrl,
                    BaseAddress);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, queryUrl);
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json;odata.metadata=none"));
            HttpClient client = new HttpClient();

            HttpResponseMessage response = client.SendAsync(request).Result;
            string result = response.Content.ReadAsStringAsync().Result;

            Assert.Equal(statusCode, (int)response.StatusCode);
            if (statusCode == (int)HttpStatusCode.BadRequest)
            {
                Assert.Contains("cannot be used in the $select query option.", result);
            }
        }

        [NuwaTheory]
        [InlineData(AutoSelectCustomerBaseUrl)]
        [InlineData(ModelBoundAutoSelectCustomerBaseUrl)]
        public void AutoSelectWithAutoExpand(string url)
        {
            string queryUrl =
                string.Format(
                    url,
                    BaseAddress);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, queryUrl);
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json;odata.metadata=none"));
            HttpClient client = new HttpClient();

            HttpResponseMessage response = client.SendAsync(request).Result;
            string result = response.Content.ReadAsStringAsync().Result;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.DoesNotContain("Id", result);
            Assert.Contains("Customer1", result);
            Assert.Contains("AutoExpandOrder1", result);
            Assert.Contains("Name", result);
        }

        [NuwaTheory]
        [InlineData(AutoSelectCustomerBaseUrl)]
        [InlineData(ModelBoundAutoSelectCustomerBaseUrl)]
        public void AutoSelectPropertyAccessWithAutoExpand(string url)
        {
            string queryUrl =
                string.Format(
                    url + "(1)/Order?$expand=Customer",
                    BaseAddress);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, queryUrl);
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json;odata.metadata=none"));
            HttpClient client = new HttpClient();

            HttpResponseMessage response = client.SendAsync(request).Result;
            string result = response.Content.ReadAsStringAsync().Result;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.DoesNotContain("Id", result);
            Assert.Contains("Customer3", result);
            Assert.Contains("AutoExpandOrder2", result);
            Assert.Contains("AutoExpandOrder3", result);
            Assert.Contains("Name", result);
        }

        [NuwaTheory]
        [InlineData(AutoSelectCustomerBaseUrl)]
        [InlineData(ModelBoundAutoSelectCustomerBaseUrl)]
        public void AutoSelectByDefault(string url)
        {
            string queryUrl =
                string.Format(
                    url + "(1)/Car",
                    BaseAddress);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, queryUrl);
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json;odata.metadata=none"));
            HttpClient client = new HttpClient();

            HttpResponseMessage response = client.SendAsync(request).Result;
            string result = response.Content.ReadAsStringAsync().Result;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.DoesNotContain("Name", result);
            Assert.Contains("2", result);
            Assert.Contains("Id", result);
        }

        [NuwaTheory]
        [InlineData(AutoSelectCustomerBaseUrl)]
        [InlineData(ModelBoundAutoSelectCustomerBaseUrl)]
        public void DollarSelectGetPrecedenceWithAutoSelect(string url)
        {
            string queryUrl =
                string.Format(
                    url + "(1)/Car?$select=CarNumber",
                    BaseAddress);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, queryUrl);
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json;odata.metadata=none"));
            HttpClient client = new HttpClient();

            HttpResponseMessage response = client.SendAsync(request).Result;
            string result = response.Content.ReadAsStringAsync().Result;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.DoesNotContain("Name", result);
            Assert.DoesNotContain("2", result);
            Assert.DoesNotContain("Id", result);
            Assert.Contains("CarNumber", result);
        }

        [NuwaTheory]
        [InlineData(AutoSelectCustomerBaseUrl)]
        [InlineData(ModelBoundAutoSelectCustomerBaseUrl)]
        public void NestedDollarSelectGetPrecedenceWithAutoSelect(string url)
        {
            string queryUrl =
                string.Format(
                    url + "(1)?$expand=Car($select=CarNumber)",
                    BaseAddress);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, queryUrl);
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json;odata.metadata=none"));
            HttpClient client = new HttpClient();

            HttpResponseMessage response = client.SendAsync(request).Result;
            string result = response.Content.ReadAsStringAsync().Result;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.DoesNotContain("Id", result);
            Assert.Contains("CarNumber", result);
        }

        [NuwaTheory]
        [InlineData(AutoSelectCustomerBaseUrl)]
        [InlineData(AutoSelectCustomerBaseUrl + "(9)")]
        [InlineData(ModelBoundAutoSelectCustomerBaseUrl)]
        [InlineData(ModelBoundAutoSelectCustomerBaseUrl + "(9)")]
        public void AutomaticSelectInDerivedType(string url)
        {
            string queryUrl = string.Format(url, BaseAddress);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, queryUrl);
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json;odata.metadata=none"));
            HttpClient client = new HttpClient();

            HttpResponseMessage response = client.SendAsync(request).Result;
            string result = response.Content.ReadAsStringAsync().Result;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("VIPNumber", result);
            Assert.DoesNotContain("Id", result);
        }
    }
}