using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational.Schema
{
    class DataObjectSchemaIndex
    {
        #region Properties

        /// <summary>
        /// Gets or sets the identifier of this index
        /// </summary>
        public int _Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the index
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the list fields that participate in this index
        /// </summary>
        public List<DataObjectSchemaFieldIndex> Fields { get; set; }
        #endregion Properties

        #region Constructor

        /// <summary>
        /// Creates a new instance of the DataRelational.Schema.DataObjectSchemaIndex class 
        /// </summary>
        /// <param name="id">the identifier of this index</param>
        /// <param name="name">the name of the index</param>
        public DataObjectSchemaIndex(int id, string name)
        {
            this._Id = id;
            this.Name = name;
            this.Fields = new List<DataObjectSchemaFieldIndex>();
        }

        #endregion Constructor

        #region Methods

        public override string ToString()
        {
            return Name;
        }

        public bool EqualsIndex(DataObjectSchemaIndex index)
        {
            if (index == null)
                return false;

            foreach (DataObjectSchemaFieldIndex thisField in this.Fields)
            {
                bool foundIndexField = false;
                foreach (DataObjectSchemaFieldIndex comareField in index.Fields)
                {
                    if (thisField.Field.Name == comareField.Field.Name && thisField.Ascending == comareField.Ascending)
                    {
                        foundIndexField = true;
                        break;
                    }
                }
                if (!foundIndexField)
                {
                    return false;
                }
            }

            foreach (DataObjectSchemaFieldIndex comareField in index.Fields)
            {
                bool foundIndexField = false;
                foreach (DataObjectSchemaFieldIndex thisField in this.Fields)
                {
                    if (thisField.Field.Name == comareField.Field.Name && thisField.Ascending == comareField.Ascending)
                    {
                        foundIndexField = true;
                        break;
                    }
                }
                if (!foundIndexField)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion Methods
    }
}
