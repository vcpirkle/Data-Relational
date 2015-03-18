using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational.Search
{
    /// <summary>
    /// A complex search object.
    /// </summary>
    public class ComplexSearch
    {
        #region Properties

        /// <summary>
        /// Gets or sets the complex search mode.
        /// </summary>
        public ComplexSearchMode SearchMode { get; set; }

        /// <summary>
        /// Gets or sets whether to include deleted objects in the search.
        /// </summary>
        public bool IncludeDeleted { get; set; }

        /// <summary>
        /// Gets or sets whether to include history objects in the search.
        /// </summary>
        public bool IncludeHistory { get; set; }

        /// <summary>
        /// Gets or sets whether to include archived objects in the search.
        /// </summary>
        public bool IncludeArchived { get; set; }

        /// <summary>
        /// The type of object being searched for.
        /// </summary>
        public Type SearchObjectType { get; set; }

        /// <summary>
        /// Gets the list of search sections.
        /// </summary>
        public ComplexSearchSectionCollection Sections { get; private set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Creates a new instance of the DataRelational.Search.ComplexSearch object.
        /// </summary>
        /// <param name="searchMode">The complex search mode.</param>
        public ComplexSearch(ComplexSearchMode searchMode)
        {
            this.SearchMode = searchMode;
            this.Sections = new ComplexSearchSectionCollection();
        }

        #endregion Constructors
    }
}
