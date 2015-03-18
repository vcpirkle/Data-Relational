using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational.Cache
{
    class DataObjectReflectionCache
    {
        #region Fields
        private Dictionary<Type, DataObjectReflection> cacheByObjectType;
        private Dictionary<short, DataObjectReflection> cacheByObjectTypeId;
        private Dictionary<string, DataObjectReflection> cacheByObjectTypeName;
        #endregion Fields

        #region Constructor

        public DataObjectReflectionCache()
        {
            cacheByObjectType = new Dictionary<Type, DataObjectReflection>();
            cacheByObjectTypeId = new Dictionary<short, DataObjectReflection>();
            cacheByObjectTypeName = new Dictionary<string, DataObjectReflection>();
        }

        #endregion Constructor

        #region Properties
        /// <summary>
        /// Gets an object reflection based on the object type
        /// </summary>
        /// <param name="objectType">The type of object</param>
        public DataObjectReflection this[Type objectType]
        {
            get
            {
                DataObjectReflection obj = null;
                if (cacheByObjectType.TryGetValue(objectType, out obj))
                {
                    return obj;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets an object reflection based on the object type id
        /// </summary>
        /// <param name="objectTypeId">The type of object</param>
        public DataObjectReflection this[short objectTypeId]
        {
            get
            {
                DataObjectReflection obj = null;
                if (cacheByObjectTypeId.TryGetValue(objectTypeId, out obj))
                {
                    return obj;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets an object reflection based on the object type name
        /// </summary>
        /// <param name="objectTypeName">The type of object</param>
        public DataObjectReflection this[string objectTypeName]
        {
            get
            {
                DataObjectReflection obj = null;
                if (cacheByObjectTypeName.TryGetValue(objectTypeName, out obj))
                {
                    return obj;
                }
                return null;
            }
        }
        #endregion Properties

        #region Methods

        /// <summary>
        /// Adds an object reflection to the object reflection cache
        /// </summary>
        /// <param name="objectReflection">The object reflection to add</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Add(DataObjectReflection objectReflection)
        {
            if (Contains(objectReflection.Schema.ObjectType))
            {
                throw new System.InvalidOperationException(string.Format("The data object reflection cache already contains the object type: {0}.", objectReflection.Schema.TypeName));
            }

            objectReflection.BuildReflection();
            cacheByObjectType.Add(objectReflection.Schema.ObjectType, objectReflection);
            cacheByObjectTypeId.Add(objectReflection.Schema._Id, objectReflection);
            cacheByObjectTypeName.Add(objectReflection.Schema.TypeName, objectReflection);
        }

        /// <summary>
        /// Determines whether the data object reflection cache contains the object type
        /// </summary>
        /// <param name="objectType">The type of object</param>
        /// <returns>true if the data object reflection cache contains the object type, otherwise false</returns>
        public bool Contains(Type objectType)
        {
            return cacheByObjectType.ContainsKey(objectType);
        }

        /// <summary>
        /// Determines whether the data object reflection cache contains the object type id
        /// </summary>
        /// <param name="objectTypeId">The type of object</param>
        /// <returns>true if the data object reflection cache contains the object type, otherwise false</returns>
        public bool Contains(short objectTypeId)
        {
            return cacheByObjectTypeId.ContainsKey(objectTypeId);
        }

        /// <summary>
        /// Determines whether the data object reflection cache contains the object type name
        /// </summary>
        /// <param name="objectTypeName">The type of object</param>
        /// <returns>true if the data object reflection cache contains the object type, otherwise false</returns>
        public bool Contains(string objectTypeName)
        {
            return cacheByObjectTypeName.ContainsKey(objectTypeName);
        }
       
        #endregion Methods

    }
}
