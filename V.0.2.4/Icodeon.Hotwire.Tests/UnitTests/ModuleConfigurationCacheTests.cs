﻿using System.Collections.Generic;
using FluentAssertions;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Tests.Framework;
using NUnit.Framework;

namespace Icodeon.Hotwire.Tests.UnitTests
{
    [TestFixture]
    public class ModuleConfigurationCacheTests : UnitTest
    {
        [Test]
        public void CanReadConfiguration()
        {
            TraceTitle("Can read configuration:");

            Trace("given test-module-config is loaded in unit test app.config");
            Trace("When the configuration is read");
            IModuleConfiguration config = new ModuleConfigurationCache("test-module-config", new TestAppCache()).Configuration;

            Trace("Then the configuration should be read correctly.");
            ModuleConfigurationBaseTests.EnsureTestModuleConfigurationAndEndpointsAreReadCorrectly(config);

        }

        [Test]
        public void MustCacheTheConfigurationBetweenReads()
        {
            TraceTitle("Must cache the configuration between reads");

            Trace("Given the item is not in the cache");
            var testCache = new TestAppCache();
            testCache.SetHistory.Should().BeEmpty();

            Trace("When the item is first requested from the cache");
            IModuleConfiguration config = new ModuleConfigurationCache("test-module-config", testCache).Configuration;

            Trace("then it is written to the cache once");
            testCache.SetHistory.Should().HaveCount(1);

            Trace("When the item is requested from the cache again");
            IModuleConfiguration config2 = new ModuleConfigurationCache("test-module-config", testCache).Configuration;

            Trace("then the value should be retrieved from the cache without writing to the cache.");
            testCache.SetHistory.Should().HaveCount(1);

        }
    }

    

    public class TestAppCache : IAppCache
    {
        public List<string> SetHistory { get; private set; }

        public TestAppCache()
        {
            _cache = new Dictionary<string, object>();
            SetHistory = new List<string>();
        }

        private Dictionary<string, object> _cache;

        public void Set(string key, object source)
        {
            SetHistory.Add(key);
            _cache[key] = source;
        }

        public object Get(string key)
        {
            if (!_cache.ContainsKey(key)) return null;
            return _cache[key];
        }

        public void Lock()
        {
            // do nothing
        }

        public void Unlock()
        {
            // do nothing
        }
    }
}
