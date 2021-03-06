﻿#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using Autofac;
using MbUnit.Framework;
using Rhino.Mocks;
using SolrNet;

namespace AutofacContrib.SolrNet.Tests {
    [TestFixture]
    public class AutofacTests {
        [Test]
        [Category("Integration")]
        public void Ping_And_Query() {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new SolrNetModule("http://localhost:8983/solr"));
            var container = builder.Build();
            var solr = container.Resolve<ISolrOperations<Entity>>();
            solr.Ping();
            Console.WriteLine(solr.Query(SolrQuery.All).Count);
        }

        [Test]
        public void ReplaceMapper() {
            var builder = new ContainerBuilder();
            var mapper = MockRepository.GenerateMock<IReadOnlyMappingManager>();
            builder.RegisterModule(new SolrNetModule("http://localhost:8983/solr") {Mapper = mapper});
            var container = builder.Build();
            var m = container.Resolve<IReadOnlyMappingManager>();
            Assert.AreSame(mapper, m);
        }

        [Test]
        public void ResolveSolrOperations() {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new SolrNetModule("http://localhost:8983/solr"));
            var container = builder.Build();
            var m = container.Resolve<ISolrOperations<Entity>>();
        }

        /// <summary>
        /// Tests that resolving <see cref="ISolrBasicOperations{T}"/> and <see cref="ISolrBasicReadOnlyOperations{T}"/>
        /// resolve to the same instance.
        /// </summary>
        [Test]
        public void ResolveSolrBasicOperationsAndSolrBasicReadOnlyOperationsUseSameEntity()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new SolrNetModule("http://localhost:8983/solr"));
            var container = builder.Build();
            var basic = container.Resolve<ISolrBasicOperations<Entity>>();
            var basicReadonly = container.Resolve<ISolrBasicReadOnlyOperations<Entity>>();
            Assert.AreSame(basic, basicReadonly);
        }

        public class Entity {}
    }
}