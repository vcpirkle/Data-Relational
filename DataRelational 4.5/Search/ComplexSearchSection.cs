using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational.Search
{
    /// <summary>
    /// A section of a complex search elements.
    /// </summary>
    public class ComplexSearchSection
    {
        #region Properties

        /// <summary>
        /// Gets the list of search conditions.
        /// </summary>
        public ComplexSearchElementCollection Conditions { get; private set; }

        /// <summary>
        /// Gets or sets the search type to perform on the search element.
        /// </summary>
        public ComplexSearchType SearchType { get; set; }

        /// <summary>
        /// Gets the list of search sections.
        /// </summary>
        public ComplexSearchSectionCollection Sections { get; private set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Creates a new instance of the DataRelational.Search.ComplexSearchSection class.
        /// </summary>
        /// <param name="searchType">The search type to perform on the search element.</param>
        public ComplexSearchSection(ComplexSearchType searchType)
        {
            this.SearchType = searchType;
            this.Conditions = new ComplexSearchElementCollection();
            this.Sections = new ComplexSearchSectionCollection();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the string representation of the complex search section.
        /// </summary>
        public override string ToString()
        {
            return "(" + this.Conditions.ToString() + ")";
        }

        /// <summary>
        /// Gets the string representation of the complex search section.
        /// </summary>
        /// <param name="includeType">Whether to include the search type. (And / Or)</param>
        public string ToString(bool includeType)
        {
            string str = ToString();
            if (includeType)
                return SearchType == ComplexSearchType.And ? " AND " + str : " OR " + str;
            else
                return str;
        }

        #endregion Methods
    }

    /// <summary>
    /// A complex search section collection.
    /// </summary>
    public class ComplexSearchSectionCollection : List<ComplexSearchSection>
    {
        /// <summary>
        /// Finds the first element matching the field name.
        /// </summary>
        /// <param name="fieldName">The field name to find.</param>
        /// <returns>The first element matching the field name.</returns>
        public ComplexSearchElement Find(string fieldName)
        {
            return SearchElements(fieldName, this);
        }

        private ComplexSearchElement SearchElements(string fieldName, ComplexSearchSectionCollection sections)
        {
            foreach (ComplexSearchSection section in sections)
            {
                ComplexSearchElement found = section.Conditions.Find(fieldName);
                if (found != null)
                    return found;
                if (section.Sections != null && section.Sections.Count > 0)
                {
                    found = SearchElements(fieldName, section.Sections);
                    if (found != null)
                        return found;
                }
            }
            return null;
        }
    }
}
