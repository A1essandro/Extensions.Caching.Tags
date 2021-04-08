using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Tests
{
    public class CommonTest : IClassFixture<Fixture>
    {

        private readonly Fixture _fixture;
        private readonly IMemoryCache _cache;

        public CommonTest(Fixture fixture)
        {
            _fixture = fixture;
            _cache = fixture.ServiceProvider.GetRequiredService<IMemoryCache>();
        }

        [Fact]
        public void RemoveByTagTest()
        {
            _cache.Set("key0", "value0", new CacheTags("tag0"));
            _cache.Set("key1", "value1", new CacheTags { "tag0", "tag1" });
            _cache.Set("key2", "value2", new[] { "tag1" });

            var beforeRemoving0 = _cache.Get("key0");
            var beforeRemoving1 = _cache.Get("key1");
            var beforeRemoving2 = _cache.Get("key2");

            _cache.RemoveByTag("tag0");

            var afterRemoving0 = _cache.Get("key0");
            var afterRemoving1 = _cache.Get("key1");
            var afterRemoving2 = _cache.Get("key2");

            beforeRemoving0.ShouldBe("value0");
            beforeRemoving1.ShouldBe("value1");
            beforeRemoving2.ShouldBe("value2");

            afterRemoving0.ShouldBeNull();
            afterRemoving1.ShouldBeNull();
            afterRemoving2.ShouldBe("value2");
        }

        [Fact]
        public async Task RemoveByTagTestWithDelay()
        {
            var timespan = TimeSpan.FromMilliseconds(100);
            _cache.Set("key0", "value0", timespan, new CacheTags("tag0", "someTag"));
            _cache.Set("key1", "value1", timespan, new CacheTags { "tag0", "tag1" });
            _cache.Set("key2", "value2", timespan, new[] { "tag1", "someTag" });

            await Task.Delay(1100);

            var beforeRemoving0 = _cache.Get("key0");
            var beforeRemoving1 = _cache.Get("key1");
            var beforeRemoving2 = _cache.Get("key2");

            _cache.RemoveByTag("tag0");

            var afterRemoving0 = _cache.Get("key0");
            var afterRemoving1 = _cache.Get("key1");
            var afterRemoving2 = _cache.Get("key2");

            beforeRemoving0.ShouldBeNull();
            beforeRemoving1.ShouldBeNull();
            beforeRemoving2.ShouldBeNull();

            afterRemoving0.ShouldBeNull();
            afterRemoving1.ShouldBeNull();
            afterRemoving2.ShouldBeNull();
        }

    }
}
