using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

namespace DataRelational.Cache
{
    /// <summary>
    /// Represents a cached element which contains a field identifier and a value.
    /// </summary>
    [Serializable, DataContract, StructLayout(LayoutKind.Sequential)]
    public struct CacheElement : ISerializable
    {
        #region Fields

        /// <summary>
        /// Gets or sets the field id of the cache element.
        /// </summary>
        [DataMember]
        public int FieldId;

        /// <summary>
        /// Gets or sets the value of the cache element.
        /// </summary>
        [DataMember]
        public object Value;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Creates a new instance of the <see cref="CacheElement"/> structure.
        /// </summary>
        /// <param name="fieldId">The field id of the cache element.</param>
        /// <param name="value">The value of the cache element.</param>
        public CacheElement(int fieldId, object value)
        {
            FieldId = fieldId;
            Value = value;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CacheElement"/> structure.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo to populate with data.</param>
        /// <param name="context">The destination <see cref="StreamingContext"/> for this serialization.</param>
        public CacheElement(SerializationInfo info, StreamingContext context)
        {
            FieldId = info.GetInt32("1");
            Value = info.GetValue("2", typeof(object));
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
            info.AddValue("1", FieldId);
            info.AddValue("2", Value);
        }

        #endregion
    }
}