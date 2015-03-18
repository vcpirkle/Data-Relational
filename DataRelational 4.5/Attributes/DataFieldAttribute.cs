using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational.Attributes
{
    /// <summary>
    /// Allows a data field to be stored
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class DataFieldAttribute : Attribute
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the DataRelational.Attributes.DataFieldAttribute class.
        /// </summary>
        public DataFieldAttribute()
        {
            this.FieldSize = 255;
        }
        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets or sets the field size of the column.  Ex: 50 = varchar(50), int.MaxValue = text or ntext depending on unicode characters.
        /// </summary>
        public int FieldSize { get; set; }

        /// <summary>
        /// Gets or sets whether the field supports unicode characters.  If false, strings will be stored as varchar instead of nvarchar or text rather than ntext.
        /// </summary>
        public bool Unicode { get; set; }

        #endregion Properties
    }
}