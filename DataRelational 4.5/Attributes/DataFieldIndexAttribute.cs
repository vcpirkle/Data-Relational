using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational.Attributes
{
    /// <summary>
    /// Allows a data field to be indexed
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public class DataFieldIndexAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the DataRelational.Attributes.DataFieldIndexAttribute class.
        /// </summary>
        /// <param name="name">The name of the index</param>
        public DataFieldIndexAttribute(string name)
        {
            this.Name = name;
            Ascending = true;
        }

        /// <summary>
        /// Initializes a new instance of the DataRelational.Attributes.DataFieldIndexAttribute class.
        /// </summary>
        /// <param name="name">The name of the index</param>
        /// <param name="ascending">Whether the index should be ascending</param>
        public DataFieldIndexAttribute(string name, bool ascending)
        {
            this.Name = name;
            this.Ascending = ascending;
        }

        /// <summary>
        /// Gets or sets the index name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets whether the index should be ascending
        /// </summary>
        public bool Ascending { get; set; }
    }
}
