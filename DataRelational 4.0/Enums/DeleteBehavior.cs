using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational
{
    /// <summary>
    /// Determines the actions taken when a data relational object is deleted.
    /// </summary>
    public enum DeleteBehavior : byte
    {
        /// <summary>
        /// The object is soft deleted meaning that it still exists in the database but the _Deleted flag will be set to true.
        /// </summary>
        Soft = 0,
        /// <summary>
        /// The object is hard deleted meaning that it no longer exists in the database.
        /// </summary>
        Hard = 1,
        /// <summary>
        /// The object is archived meaning that it will be moved to an archive table.
        /// </summary>
        Archive = 2
    }
}
