using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational
{
    /// <summary>
    /// Determines the actions taken when a data relational object is returned from the database.
    /// </summary>
    public enum CacheBehavior : byte
    {
        /// <summary>
        /// The object is returned from the database each time it is requsted.
        /// </summary>
        NoCahce = 0,
        /// <summary>
        /// The object is returned from the database the first time it is requested.  All following request for the object will be returned from the cache until the object is removed from the cache.
        /// </summary>
        Cahce = 1
    }
}
