using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.Extensions.Caching.Memory
{
    public static class CacheTagExtension
    {

        private static ConcurrentDictionary<string, List<object>> TagToKeys = new ConcurrentDictionary<string, List<object>>();

        /// <summary>
        /// Locker for thread safe handling of <see cref="TagToKeys" />
        /// </summary>
        /// <returns></returns>
        private static object Locker = new object();

        private static PostEvictionCallbackRegistration PostEvictionCallbackRegistration = new PostEvictionCallbackRegistration()
        {
            EvictionCallback = (k, v, r, s) =>
            {
                RemoveKeyWithLock(k);
            },
        };

        #region Set Methods

        public static TItem Set<TItem>(this IMemoryCache cache, object key, TItem value, CacheTags tags)
        {
            UpdateTagsDictionaryWithLock(key, tags);

            var options = GetMemoryCacheOptionsWithRegisteredCallback();

            return cache.Set(key, value, options);
        }

        public static TItem Set<TItem>(this IMemoryCache cache, object key, TItem value, DateTimeOffset absoluteExpiration, CacheTags tags)
        {
            UpdateTagsDictionaryWithLock(key, tags);

            var options = GetMemoryCacheOptionsWithRegisteredCallback();
            options.AbsoluteExpiration = absoluteExpiration;

            return cache.Set(key, value, options);
        }

        public static TItem Set<TItem>(this IMemoryCache cache, object key, TItem value, TimeSpan absoluteExpirationRelativeToNow, CacheTags tags)
        {
            UpdateTagsDictionaryWithLock(key, tags);

            var options = GetMemoryCacheOptionsWithRegisteredCallback();
            options.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;

            return cache.Set(key, value, options);
        }

        public static TItem Set<TItem>(this IMemoryCache cache, object key, TItem value, IChangeToken expirationToken, CacheTags tags)
        {
            UpdateTagsDictionaryWithLock(key, tags);

            var options = GetMemoryCacheOptionsWithRegisteredCallback();
            options.ExpirationTokens.Add(expirationToken);

            return cache.Set(key, value, options);
        }

        public static TItem Set<TItem>(this IMemoryCache cache, object key, TItem value, MemoryCacheEntryOptions options, CacheTags tags)
        {
            UpdateTagsDictionaryWithLock(key, tags);

            options.PostEvictionCallbacks.Add(PostEvictionCallbackRegistration);

            return cache.Set(key, value, options);
        }

        #endregion

        /// <summary>
        /// Removes the objects associated with the given tag.
        /// </summary>
        /// <param name="cache"><see cref="IMemoryCache"></param>
        /// <param name="tag">Tag for removing</param>
        /// <returns>Number of removed items</returns>
        public static void RemoveByTag(this IMemoryCache cache, string tag)
        {
            if (TagToKeys.TryGetValue(tag, out var keys))
            {
                foreach (var key in keys) cache.Remove(key);

                TagToKeys.TryRemove(tag, out var _);
            }
        }

        #region  Private methos

        private static MemoryCacheEntryOptions GetMemoryCacheOptionsWithRegisteredCallback()
        {
            var options = new MemoryCacheEntryOptions();
            options.PostEvictionCallbacks.Add(PostEvictionCallbackRegistration);

            return options;
        }

        private static void UpdateTagsDictionaryWithLock(object key, CacheTags tags)
        {
            foreach (var tag in tags)
            {
                lock (Locker)
                {
                    UpdateTagsDictionary(key, tag);
                }
            }
        }

        /// <summary>
        /// Wrapped by lock method <see cref="RemoveKey" />
        /// </summary>
        /// <param name="key"></param>
        private static void RemoveKeyWithLock(object key)
        {
            lock (Locker)
            {
                RemoveKey(key);
            }
        }

        /// <summary>
        /// Removing key from all entries of <see cref="TagToKeys" />
        /// </summary>
        /// <param name="key"></param>
        private static void RemoveKey(object key)
        {
            foreach (var kvPair in TagToKeys)
            {
                if (!kvPair.Value.Contains(key))
                {
                    continue;
                }

                if (kvPair.Value.Count > 1)
                {
                    kvPair.Value.Remove(key);
                }
                else
                {
                    TagToKeys.TryRemove(kvPair.Key, out var _);
                }
            }
        }

        private static void UpdateTagsDictionary(object key, string tag)
        {
            if (TagToKeys.TryGetValue(tag, out var keys))
            {
                keys.Add(key);
            }
            else
            {
                TagToKeys.TryAdd(tag, new List<object> { key });
            }
        }

        #endregion

    }
}
