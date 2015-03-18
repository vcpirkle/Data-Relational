using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational
{
    /// <summary>
    /// Determines the actions taken when a data relational object is saved to the database.
    /// </summary>
    public enum SaveBehavior : byte
    {
        /// <summary>
        /// The object is always saved to the database.
        /// </summary>
        Always = 0,
        /// <summary>
        /// The object is only saved to the database when it is dirty, meaning a data field or child relationship has changed.
        /// </summary>
        Dirty = 1,
        /// <summary>
        /// The object is never saved to the database.
        /// </summary>
        Never = 2
    }
}
