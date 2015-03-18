using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational.Schema
{
    class DataObjectSchemaFieldIndex
    {
        #region Properties
       
        /// <summary>
        /// Gets or sets the name of the field
        /// </summary>
        public DataObjectSchemaField Field { get; set; }

        /// <summary>
        /// Gets or sets whether this indexed field is ascending 
        /// </summary>
        public bool Ascending { get; set; }
       
        #endregion Properties

        #region Methods

        public override string ToString()
        {
            return Field.Name + " " + (Ascending ? "Ascending" : "Descending");
        }

        #endregion Methods
    }
}
