using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataRelational.Schema
{
    [DataContract]
    class DataObjectSchema : DataObjectType
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the DataRelational.Schema.DataObjectSchema class.
        /// </summary>
        /// <param name="id">the data relational object type id</param>
        /// <param name="objectType">the data relational object type</param>
        /// <param name="typeName">the data relational object type name</param>
        /// <param name="tableName">the data relational object table name</param>
        public DataObjectSchema(short id, Type objectType, string typeName, string tableName)
            : base(id, objectType, typeName, tableName)
        {
            this.Fields = new List<DataObjectSchemaField>();
            this.Indexes = new List<DataObjectSchemaIndex>();
        }

        #endregion Constructor

        #region Properties

        internal List<DataObjectSchemaField> Fields { get; private set; }

        internal List<DataObjectSchemaIndex> Indexes { get; private set; }

        internal DataObjectSchemaField FindField(int fieldId)
        {
            foreach (DataObjectSchemaField field in this.Fields)
                if (field._Id == fieldId)
                    return field;
            return null;
        }

        internal DataObjectSchemaIndex FindIndex(int indexId)
        {
            foreach (DataObjectSchemaIndex index in this.Indexes)
                if (index._Id == indexId)
                    return index;
            return null;
        }

        internal DataObjectSchemaIndex FindIndex(string indexName)
        {
            foreach (DataObjectSchemaIndex index in this.Indexes)
                if (index.Name == indexName)
                    return index;
            return null;
        }

        #endregion Properties
    }
}
