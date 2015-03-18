using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational.Cache
{
    class DataCache : IDataCache
    {
        #region Fields

        private object sync;
        private Dictionary<short, Dictionary<DataKey, CacheItem>> cache;

        #endregion Fields

        #region Consructor

        public DataCache()
        {
            sync = new object();
            cache = new Dictionary<short, Dictionary<DataKey, CacheItem>>();
        }

        #endregion Constructor

        #region IDataCache Members

        /// <summary>
        /// Stores an item in the cache.
        /// </summary>
        /// <param name="item">The item to store.</param>
        /// <param name="expires">The amount of time in minutes before a cached item expires. -1 never expires.</param>
        public void Store(CacheItem item, int expires)
        {
            item.Expires = expires;
            lock (sync)
            {
                Dictionary<DataKey, CacheItem> objCache = null;
                if (!cache.TryGetValue(item.Key.TypeId, out objCache))
                {
                    objCache = new Dictionary<DataKey, CacheItem>();
                    cache.Add(item.Key.TypeId, objCache);
                }
                objCache[item.Key] = item;
            }
        }

        /// <summary>
        /// Gets an item from the cache.
        /// </summary>
        /// <param name="key">The data key of the item to get.</param>
        /// <returns>The cached item if contained in the cache; otherwise null.</returns>
        public CacheItem Get(DataKey key)
        {
            CacheItem item = null;
            lock (sync)
            {
                Dictionary<DataKey, CacheItem> objCache = null;
                if (cache.TryGetValue(key.TypeId, out objCache))
                {
                    if (objCache.TryGetValue(key, out item))
                    {
                        if (item.Expires != -1 && (DateTime.Now - item.TimeStamp).TotalMinutes > item.Expires)
                        {
                            //The item has expired and needs to be removed
                            objCache.Remove(key);
                            item = null;
                        }
                    }
                }
            }
            return item;
        }

        /// <summary>
        /// Indicates whether the cache contains the data key.
        /// </summary>
        /// <param name="key">The data key.</param>
        /// <returns>true if the cache contains the data key; otherwise false.</returns>
        public bool Contains(DataKey key)
        {
            bool contains = false;
            lock (sync)
            {
                Dictionary<DataKey, CacheItem> objCache = null;
                if (cache.TryGetValue(key.TypeId, out objCache))
                {
                    CacheItem item = null;
                    if (objCache.TryGetValue(key, out item))
                    {
                        if (item.Expires != -1 && (DateTime.Now - item.TimeStamp).TotalMinutes > item.Expires)
                        {
                            //The item has expired and needs to be removed
                            objCache.Remove(key);
                            contains = false;
                        }
                        else
                        {
                            contains = true;
                        }
                    }
                }
            }
            return contains;
        }

        /// <summary>
        /// Removes an item from the cache.
        /// </summary>
        /// <param name="key">The data key of the item to remove.</param>
        public void Remove(DataKey key)
        {
            lock (sync)
            {
                Dictionary<DataKey, CacheItem> objCache = null;
                if (cache.TryGetValue(key.TypeId, out objCache))
                {
                    objCache.Remove(key);
                }
            }
        }

        /// <summary>
        /// Clears all items from the cache.
        /// </summary>
        public void Clear()
        {
            lock (sync)
            {
                cache.Clear();
            }
        }

        #endregion
    }
}
