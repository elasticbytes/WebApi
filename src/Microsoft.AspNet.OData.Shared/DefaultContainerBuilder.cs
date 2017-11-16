﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.OData.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;

namespace Microsoft.AspNet.OData
{
    /// <summary>
    /// The default container builder implementation based on the Microsoft dependency injection framework.
    /// </summary>
    public class DefaultContainerBuilder : IContainerBuilder
    {
        private IServiceCollection services;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultContainerBuilder"/> class.
        /// </summary>
        public DefaultContainerBuilder()
            : this(new ServiceCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultContainerBuilder"/> class.
        /// </summary>
        /// <param name="services">IServiceCollection collection in which to add services.</param>
        internal DefaultContainerBuilder(IServiceCollection services)
        {
            if (services == null)
            {
                throw Error.ArgumentNull("services");
            }

            this.services = services;
        }

        /// <summary>
        /// Adds a service of <paramref name="serviceType"/> with an <paramref name="implementationType"/>.
        /// </summary>
        /// <param name="lifetime">The lifetime of the service to register.</param>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        /// <returns>The <see cref="IContainerBuilder"/> instance itself.</returns>
        public virtual IContainerBuilder AddService(
            Microsoft.OData.ServiceLifetime lifetime,
            Type serviceType,
            Type implementationType)
        {
            if (serviceType == null)
            {
                throw Error.ArgumentNull("serviceType");
            }

            if (implementationType == null)
            {
                throw Error.ArgumentNull("implementationType");
            }

            services.Add(new ServiceDescriptor(
                serviceType, implementationType, TranslateServiceLifetime(lifetime)));

            return this;
        }

        /// <summary>
        /// Adds a service of <paramref name="serviceType"/> with an <paramref name="implementationFactory"/>.
        /// </summary>
        /// <param name="lifetime">The lifetime of the service to register.</param>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="implementationFactory">The factory that creates the service.</param>
        /// <returns>The <see cref="Microsoft.OData.IContainerBuilder"/> instance itself.</returns>
        public Microsoft.OData.IContainerBuilder AddService(
            Microsoft.OData.ServiceLifetime lifetime,
            Type serviceType,
            Func<IServiceProvider, object> implementationFactory)
        {
            if (serviceType == null)
            {
                throw Error.ArgumentNull("serviceType");
            }

            if (implementationFactory == null)
            {
                throw Error.ArgumentNull("implementationFactory");
            }

            services.Add(new ServiceDescriptor(
                serviceType, implementationFactory, TranslateServiceLifetime(lifetime)));

            return this;
        }

        /// <summary>
        /// Builds a container which implements <see cref="IServiceProvider"/> and contains
        /// all the services registered.
        /// </summary>
        /// <returns>The container built by this builder.</returns>
        public virtual IServiceProvider BuildContainer()
        {
            return services.BuildServiceProvider();
        }

        private static Microsoft.Extensions.DependencyInjection.ServiceLifetime TranslateServiceLifetime(
            Microsoft.OData.ServiceLifetime lifetime)
        {
            switch (lifetime)
            {
            case Microsoft.OData.ServiceLifetime.Scoped:
                return Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped;
            case Microsoft.OData.ServiceLifetime.Singleton:
                return Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton;
            default:
                return Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient;
            }
        }
    }
}
