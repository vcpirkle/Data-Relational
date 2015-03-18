using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational.Schema
{
    class DataObjectSchemaField
    {
        #region Properties
        /// <summary>
        /// Gets or sets the identifier of this field
        /// </summary>
        public int _Id { get; set; }

        /// <summary>
        /// Gets or sets the name of this field
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of storage required for this field
        /// </summary>
        public DataObjectSchemaFieldType FieldType { get; set; }

        /// <summary>
        /// Gets or sets the length of the field
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Gets or sets whether this field is a unicode field
        /// </summary>
        public bool Unicode { get; set; }

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Creates a new instance of the DataRelational.Schema.DataObjectSchemaField class.
        /// </summary>
        /// <param name="id">the identifier of this field</param>
        /// <param name="name">the name of this field</param>
        /// <param name="fieldType">the type of storage required for this field</param>
        /// <param name="length">the length of the field</param>
        /// <param name="unicode">whether this field is a unicode field</param>
        public DataObjectSchemaField(int id, string name, DataObjectSchemaFieldType fieldType, int length, bool unicode)
        {
            this._Id = id;
            this.Name = name;
            this.FieldType = fieldType;
            this.Length = length;
            this.Unicode = unicode;
        }

        #endregion Constructor

        #region Methods

        public override string ToString()
        {
            return Name;
        }

        #endregion Methods
    }
}
