using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace DataRelational.Cache
{
    /// <summary>
    /// Represents a parent child relationship between two objects.
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct Relationship
    {
        #region Properties

        /// <summary>
        /// Gets or sets the identifier for the relationship parent object.
        /// </summary>
        public long ParentId;

        /// <summary>
        /// Gets or sets the object type identifier for the relationship parent object.
        /// </summary>
        public short ParentTypeId;

        /// <summary>
        /// Gets or sets the field identifier for the relationship parent object.
        /// </summary>
        public int ParentFieldId;

        /// <summary>
        /// Gets or sets the identifier for the relationship child object.
        /// </summary>
        public long ChildId;

        /// <summary>
        ///  Gets or sets the object type identifier for the relationship child object.
        /// </summary>
        public short ChildTypeId;

        /// <summary>
        /// Gets or sets the date and time the relationship begins.
        /// </summary>
        public DateTime BeginDate;

        /// <summary>
        /// Gets or sets the date and time the relationship ends.
        /// </summary>
        public DateTime EndDate;

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Creates a new instnace of the DataRelational.Cache.Relationship structure.
        /// </summary>
        /// <param name="parentId">The identifier for the relationship parent object.</param>
        /// <param name="parentTypeId">The object type identifier for the relationship parent object.</param>
        /// <param name="parentFieldId">The field identifier for the relationship parent object.</param>
        /// <param name="childId">The identifier for the relationship child object.</param>
        /// <param name="childTypeId">The object type identifier for the relationship child object.</param>
        /// <param name="beginDate">The date and time the relationship begins.</param>
        /// <param name="endDate">The date and time the relationship ends.</param>
        public Relationship(long parentId, short parentTypeId, int parentFieldId, long childId, short childTypeId, DateTime beginDate, DateTime endDate)
        {
            ParentId = parentId;
            ParentTypeId = parentTypeId;
            ParentFieldId = parentFieldId;
            ChildId = childId;
            ChildTypeId = childTypeId;
            BeginDate = beginDate;
            EndDate = endDate;
        }

        #endregion Constructor
    }
}
