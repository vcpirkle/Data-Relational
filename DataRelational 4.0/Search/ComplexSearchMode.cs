using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational.Search
{
    /// <summary>
    /// Describes the type of complex search.
    /// </summary>
    public enum ComplexSearchMode : byte
    {
        /// <summary>
        /// The complex search is being used to get a single object.
        /// </summary>
        GetSingle = 0,
        /// <summary>
        /// The complex search is beging used to get multiple objects.
        /// </summary>
        GetMultiple = 1,
        /// <summary>
        /// The complex search is being used to an object or objects history.
        /// </summary>
        GetHistory = 2,
        /// <summary>
        /// The complex search is being used as a search.
        /// </summary>
        Search = 3
    }

    /// <summary>
    /// Describes a the type of a complex search element.
    /// </summary>
    public enum ComplexSearchType : byte
    {
        /// <summary>
        /// The mode is and.
        /// </summary>
        And = 0,
        /// <summary>
        /// The mode is or.
        /// </summary>
        Or = 1
    }
}
