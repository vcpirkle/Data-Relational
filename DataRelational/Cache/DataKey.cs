using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Runtime.Serialization;

namespace DataRelational.Cache
{
    /// <summary>
    /// Represents a data key which contains an object identifier, revision identifier, and object type identifier.
    /// </summary>
    [Serializable, DataContract, StructLayout(LayoutKind.Sequential)]
    public struct DataKey : ISerializable
    {
        #region Properties

        /// <summary>
        /// Gets the object identifer.
        /// </summary>
        [DataMember]
        public long Id;

        /// <summary>
        /// Gets the object revision identifier.
        /// </summary>
        [DataMember]
        public int RevisionId;

        /// <summary>
        /// Gets the object type identifier.
        /// </summary>
        [DataMember]
        public short TypeId;

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Creates a new instance of the DataRelational.Cache.DataKey structure.
        /// </summary>
        /// <param name="id">The object identifer.</param>
        /// <param name="revisionId">The object revision identifier.</param>
        /// <param name="typeId">The object type identifier.</param>
        public DataKey(long id, int revisionId, short typeId)
        {
            Id = id;
            RevisionId = revisionId;
            TypeId = typeId;
        }

        /// <summary>
        /// Creates a new instance of the DataRelational.Cache.DataKey structure.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo to populate with data.</param>
        /// <param name="context">The destination <see cref="StreamingContext"/> for this serialization.</param>
        public DataKey(SerializationInfo info, StreamingContext context)
        {
            Id = info.GetInt64("1");
            RevisionId = info.GetInt32("2");
            TypeId = info.GetInt16("3");
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>true if obj and this instance are the same type and represent the same value; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is DataKey)
            {
                DataKey dk = (DataKey)obj;
                return dk.Id == this.Id && dk.RevisionId == this.RevisionId && dk.TypeId == this.TypeId;
            }
            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// The equals operator.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>true if the left object equals the right object; otherwise false.</returns>
        public static bool operator ==(DataKey left, DataKey right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// The not equals operator.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>true if the left object does not equal the right object; otherwise false.</returns>
        public static bool operator !=(DataKey left, DataKey right)
        {
            return !left.Equals(right);
        }

        #endregion Methods

        #region ISerializable Members

        /// <summary>
        /// Populates a System.Runtime.Serialization.SerializationInfo with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo to populate with data.</param>
        /// <param name="context">The destination <see cref="StreamingContext"/> for this serialization.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("1", Id);
            info.AddValue("2", RevisionId);
            info.AddValue("3", TypeId);
        }

        #endregion
    }
}