using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational.Cache
{
    /// <summary>
    /// The base interface for all data caches.
    /// </summary>
    public interface IDataCache
    {
        /// <summary>
        /// Stores an item in the cache.
        /// </summary>
        /// <param name="item">The item to store.</param>
        /// <param name="expires">The amount of time in minutes before a cached item expires. -1 never expires.</param>
        void Store(CacheItem item, int expires);

        /// <summary>
        /// Gets an item from the cache.
        /// </summary>
        /// <param name="key">The data key of the item to get.</param>
        /// <returns>The cached item if contained in the cache; otherwise null.</returns>
        CacheItem Get(DataKey key);

        /// <summary>
        /// Indicates whether the cache contains the data key.
        /// </summary>
        /// <param name="key">The data key.</param>
        /// <returns>true if the cache contains the data key; otherwise false.</returns>
        bool Contains(DataKey key);

        /// <summary>
        /// Removes an item from the cache.
        /// </summary>
        /// <param name="key">The data key of the item to remove.</param>
        void Remove(DataKey key);

        /// <summary>
        /// Clears all items from the cache.
        /// </summary>
        void Clear();
    }
}