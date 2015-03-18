using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using DataRelational.Schema;

namespace DataRelational
{
    /// <summary>
    /// Represents the type of data relational object type.
    /// </summary>
    [DataContract, KnownType(typeof(DataObjectSchema))]
    public class DataObjectType
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the DataRelational.DataObjectType class.
        /// </summary>
        /// <param name="id">the data relational object type id</param>
        /// <param name="objectType">the data relational object type</param>
        /// <param name="typeName">the data relational object type name</param>
        /// <param name="tableName">the data relational object table name</param>
        public DataObjectType(short id, Type objectType, string typeName, string tableName)
        {
            _Id = id;
            ObjectType = objectType;
            TypeName = typeName;
            TableName = tableName;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets the data relational object type id.
        /// </summary>
        [DataMember]
        public short _Id { get; private set; }

        /// <summary>
        /// Gets the data relational object type.
        /// </summary>
        internal Type ObjectType { get; private set; }

        /// <summary>
        /// Gets the data relational object type name.
        /// </summary>
        [DataMember]
        public string TypeName { get; private set; }

        /// <summary>
        /// Gets the data relational object table name.
        /// </summary>
        internal string TableName { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Returns a System.String that represents the data object type
        /// </summary>
        public override string ToString()
        {
            return TypeName;
        }

        #endregion Methods
    }
}
