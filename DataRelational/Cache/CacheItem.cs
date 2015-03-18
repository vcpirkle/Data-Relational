using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

namespace DataRelational.Cache
{
    /// <summary>
    /// Represents a cached item which contains a data key and collection of cache elements associated with that key.
    /// </summary>
    [Serializable, DataContract]
    public class CacheItem : ISerializable
    {
        #region Fields

        /// <summary>
        /// Gets or sets the data key of the cache item.
        /// </summary>
        [DataMember]
        public DataKey Key;

        /// <summary>
        /// Gets or sets the time that the cache item was stored in the cache.
        /// </summary>
        [DataMember]
        public DateTime TimeStamp;

        /// <summary>
        /// Gets or sets the time in minutes that the cache item expires.
        /// </summary>
        [DataMember]
        public int Expires;

        /// <summary>
        /// Gets or sets the list of cache elements of the cahce item.
        /// </summary>
        [DataMember]
        public List<CacheElement> Elements;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Creates a new instance of the <see cref="CacheElement"/> structure.
        /// </summary>
        /// <param name="key">The data key of the cache item.</param>
        public CacheItem(DataKey key)
        {
            Key = key;
            Expires = -1;
            TimeStamp = DateTime.MinValue;
            Elements = new List<CacheElement>();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CacheElement"/> structure.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo to populate with data.</param>
        /// <param name="context">The destination <see cref="StreamingContext"/> for this serialization.</param>
        public CacheItem(SerializationInfo info, StreamingContext context)
        {
            Key = (DataKey)info.GetValue("1", typeof(DataKey));
            TimeStamp = info.GetDateTime("2");
            Expires = info.GetInt32("3");
            Elements = new List<CacheElement>();
            int elementCount = info.GetInt32("4");
            int serializationCounter = 5;
            for (int i = 0; i < elementCount; i++)
            {
                CacheElement element = (CacheElement)info.GetValue(serializationCounter.ToString(), typeof(CacheElement));
                Elements.Add(element);
                serializationCounter++;
            }
        }

        #endregion Constructor

        #region ISerializable Members

        /// <summary>
        /// Populates a System.Runtime.Serialization.SerializationInfo with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo to populate with data.</param>
        /// <param name="context">The destination <see cref="StreamingContext"/> for this serialization.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("1", Key);
            info.AddValue("2", TimeStamp);
            info.AddValue("3", Expires);
            info.AddValue("4", Elements.Count);
            int serializationCounter = 5;
            foreach (CacheElement element in Elements)
            {
                info.AddValue(serializationCounter.ToString(), element);
                serializationCounter++;
            }
        }

        #endregion
    }
}