using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational.Search
{
    /// <summary>
    /// A method pointer to a search builder method.
    /// </summary>
    /// <typeparam name="T">The type of search expression builder.</typeparam>
    /// <param name="builder">The search expression builder.</param>
    public delegate void SearchExpression<T>(T builder);

    /// <summary>
    /// Builds complex searches.
    /// </summary>
    public class SearchBuilder
    {
        #region Fields

        private ComplexSearch search;

        #endregion Fields

        #region Constructor
        internal SearchBuilder(ComplexSearch search)
        {
            this.search = search;
        }
        #endregion Constructor

        #region Methods

        /// <summary>
        /// Creates a where serch condition.
        /// </summary>
        /// <param name="objectType">The type of object search.</param>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <returns>A complex search object.</returns>
        public static ComplexSearch Where(Type objectType, string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression)
        {
            ComplexSearch search = new ComplexSearch(ComplexSearchMode.Search)
            {
                SearchObjectType = objectType
            };
            ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.And, ComplexSearchOperator.Equal));
            search.Sections.Add(section);
            searchExpression(new SearchBuilder(search));

            return search;
        }

        /// <summary>
        /// Creates a where not serch condition.
        /// </summary>
        /// <param name="objectType">The type of object search.</param>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must not be equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <returns>A complex search object.</returns>
        public static ComplexSearch WhereNot(Type objectType, string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression)
        {
            ComplexSearch search = new ComplexSearch(ComplexSearchMode.Search)
            {
                SearchObjectType = objectType
            };
            ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.And, ComplexSearchOperator.NotEqual));
            search.Sections.Add(section);
            searchExpression(new SearchBuilder(search));

            return search;
        }

        /// <summary>
        /// Creates a where serch condition.
        /// </summary>
        /// <param name="objectType">The type of object search.</param>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <returns>A complex search object.</returns>
        public static ComplexSearch WhereGreater(Type objectType, string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression)
        {
            ComplexSearch search = new ComplexSearch(ComplexSearchMode.Search)
            {
                SearchObjectType = objectType
            };
            ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.And, ComplexSearchOperator.GreaterThan));
            search.Sections.Add(section);
            searchExpression(new SearchBuilder(search));

            return search;
        }

        /// <summary>
        /// Creates a where serch condition.
        /// </summary>
        /// <param name="objectType">The type of object search.</param>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <returns>A complex search object.</returns>
        public static ComplexSearch WhereLess(Type objectType, string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression)
        {
            ComplexSearch search = new ComplexSearch(ComplexSearchMode.Search)
            {
                SearchObjectType = objectType
            };
            ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.And, ComplexSearchOperator.LessThan));
            search.Sections.Add(section);
            searchExpression(new SearchBuilder(search));

            return search;
        }

        /// <summary>
        /// Creates a where serch condition.
        /// </summary>
        /// <param name="objectType">The type of object search.</param>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than or equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <returns>A complex search object.</returns>
        public static ComplexSearch WhereGreaterOrEqual(Type objectType, string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression)
        {
            ComplexSearch search = new ComplexSearch(ComplexSearchMode.Search)
            {
                SearchObjectType = objectType
            };
            ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.And, ComplexSearchOperator.GreaterThanOrEqual));
            search.Sections.Add(section);
            searchExpression(new SearchBuilder(search));

            return search;
        }

        /// <summary>
        /// Creates a where serch condition.
        /// </summary>
        /// <param name="objectType">The type of object search.</param>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than or equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <returns>A complex search object.</returns>
        public static ComplexSearch WhereLessOrEqual(Type objectType, string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression)
        {
            ComplexSearch search = new ComplexSearch(ComplexSearchMode.Search)
            {
                SearchObjectType = objectType
            };
            ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.And, ComplexSearchOperator.LessThanOrEqual));
            search.Sections.Add(section);
            searchExpression(new SearchBuilder(search));

            return search;
        }

        /// <summary>
        /// Creates a where serch condition.
        /// </summary>
        /// <param name="objectType">The type of object search.</param>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be between.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <returns>A complex search object.</returns>
        public static ComplexSearch WhereBetween(Type objectType, string fieldName, object equalsValues, SearchExpression<SearchBuilder> searchExpression)
        {
            ComplexSearch search = new ComplexSearch(ComplexSearchMode.Search)
            {
                SearchObjectType = objectType
            };
            ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValues, ComplexSearchType.And, ComplexSearchOperator.Between));
            search.Sections.Add(section);
            searchExpression(new SearchBuilder(search));

            return search;
        }

        /// <summary>
        /// Creates a where serch condition.
        /// </summary>
        /// <param name="objectType">The type of object search.</param>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be like.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <returns>A complex search object.</returns>
        public static ComplexSearch WhereLike(Type objectType, string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression)
        {
            ComplexSearch search = new ComplexSearch(ComplexSearchMode.Search)
            {
                SearchObjectType = objectType
            };
            ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.And, ComplexSearchOperator.Like));
            search.Sections.Add(section);
            searchExpression(new SearchBuilder(search));

            return search;
        }

        /// <summary>
        /// Creates a where serch condition.
        /// </summary>
        /// <param name="objectType">The type of object search.</param>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be in.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <returns>A complex search object.</returns>
        public static ComplexSearch WhereIn(Type objectType, string fieldName, object equalsValues, SearchExpression<SearchBuilder> searchExpression)
        {
            ComplexSearch search = new ComplexSearch(ComplexSearchMode.Search)
            {
                SearchObjectType = objectType
            };
            ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValues, ComplexSearchType.And, ComplexSearchOperator.In));
            search.Sections.Add(section);
            searchExpression(new SearchBuilder(search));

            return search;
        }

        /// <summary>
        /// Creates an and serch condition. AND ( Conditions [...] )
        /// </summary>
        /// <param name="conditionExpression">A condition expression for adding more conditions to the search.</param>
        public void And(SearchExpression<ConditionExpression> conditionExpression)
        {
            if (conditionExpression != null)
            {
                ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
                conditionExpression(new ConditionExpression(section));
                if (section.Conditions.Count > 0)
                {
                    search.Sections.Add(section);
                }
            }
        }

        /// <summary>
        /// Creates an or serch condition. Or ( Conditions [...] )
        /// </summary>
        /// <param name="conditionExpression">A condition expression for adding more conditions to the search.</param>
        public void Or(SearchExpression<ConditionExpression> conditionExpression)
        {
            if (conditionExpression != null)
            {
                ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.Or);
                conditionExpression(new ConditionExpression(section));
                if (section.Conditions.Count > 0)
                {
                    search.Sections.Add(section);
                }
            }
        }

        /// <summary>
        /// Creates an and serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be equal to.</param>
        public void AndEquals(string fieldName, object equalsValue)
        {
            ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.And, ComplexSearchOperator.Equal));
            search.Sections.Add(section);
        }

        /// <summary>
        /// Creates an and serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must not be equal to.</param>
        public void AndNotEquals(string fieldName, object equalsValue)
        {
            ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.And, ComplexSearchOperator.NotEqual));
            search.Sections.Add(section);
        }

        /// <summary>
        /// Creates an and serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than.</param>
        public void AndGreater(string fieldName, object equalsValue)
        {
            ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.And, ComplexSearchOperator.GreaterThan));
            search.Sections.Add(section);
        }

        /// <summary>
        /// Creates an and serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than.</param>
        public void AndLess(string fieldName, object equalsValue)
        {
            ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.And, ComplexSearchOperator.LessThan));
            search.Sections.Add(section);
        }

        /// <summary>
        /// Creates an and serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than or equal to.</param>
        public void AndGreaterOrEqual(string fieldName, object equalsValue)
        {
            ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.And, ComplexSearchOperator.GreaterThanOrEqual));
            search.Sections.Add(section);
        }

        /// <summary>
        /// Creates an and serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than or equal to.</param>
        public void AndLessOrEqual(string fieldName, object equalsValue)
        {
            ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.And, ComplexSearchOperator.LessThanOrEqual));
            search.Sections.Add(section);
        }

        /// <summary>
        /// Creates an and serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be between.</param>
        public void AndBetween(string fieldName, object equalsValues)
        {
            ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValues, ComplexSearchType.And, ComplexSearchOperator.Between));
            search.Sections.Add(section);
        }

        /// <summary>
        /// Creates an and serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be like.</param>
        public void AndLike(string fieldName, object equalsValue)
        {
            ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.And, ComplexSearchOperator.Like));
            search.Sections.Add(section);
        }

        /// <summary>
        /// Creates an and serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be in.</param>
        public void AndIn(string fieldName, object equalsValues)
        {
            ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValues, ComplexSearchType.And, ComplexSearchOperator.In));
            search.Sections.Add(section);
        }

        /// <summary>
        /// Creates an or serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be equal to.</param>
        public void OrEquals(string fieldName, object equalsValue)
        {
            ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.Or);
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.Or, ComplexSearchOperator.Equal));
            search.Sections.Add(section);
        }

        /// <summary>
        /// Creates an or serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must not be equal to.</param>
        public void OrNotEquals(string fieldName, object equalsValue)
        {
            ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.Or);
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.Or, ComplexSearchOperator.NotEqual));
            search.Sections.Add(section);
        }

        /// <summary>
        /// Creates an or serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than.</param>
        public void OrGreater(string fieldName, object equalsValue)
        {
            ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.Or);
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.Or, ComplexSearchOperator.GreaterThan));
            search.Sections.Add(section);
        }

        /// <summary>
        /// Creates an or serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than.</param>
        public void OrLess(string fieldName, object equalsValue)
        {
            ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.Or);
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.Or, ComplexSearchOperator.LessThan));
            search.Sections.Add(section);
        }

        /// <summary>
        /// Creates an or serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than or equal to.</param>
        public void OrGreaterOrEqual(string fieldName, object equalsValue)
        {
            ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.Or);
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.Or, ComplexSearchOperator.GreaterThanOrEqual));
            search.Sections.Add(section);
        }

        /// <summary>
        /// Creates an or serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than or equal to.</param>
        public void OrLessOrEqual(string fieldName, object equalsValue)
        {
            ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.Or);
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.Or, ComplexSearchOperator.LessThanOrEqual));
            search.Sections.Add(section);
        }

        /// <summary>
        /// Creates an or serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be between.</param>
        public void OrBetween(string fieldName, object equalsValues)
        {
            ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.Or);
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValues, ComplexSearchType.Or, ComplexSearchOperator.Between));
            search.Sections.Add(section);
        }

        /// <summary>
        /// Creates an or serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be like.</param>
        public void OrLike(string fieldName, object equalsValue)
        {
            ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.Or);
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.Or, ComplexSearchOperator.Like));
            search.Sections.Add(section);
        }

        /// <summary>
        /// Creates an or serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be in.</param>
        public void OrIn(string fieldName, object equalsValues)
        {
            ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.Or);
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValues, ComplexSearchType.Or, ComplexSearchOperator.In));
            search.Sections.Add(section);
        }

        #endregion Methods
    }

    /// <summary>
    /// Builds complex search conditions.
    /// </summary>
    public class ConditionExpression
    {
        #region Fields

        private ComplexSearchSection section;

        #endregion Fields

        #region Constructor
        internal ConditionExpression(ComplexSearchSection section)
        {
            this.section = section;
        }
        #endregion Constructor

        #region Methods

        /// <summary>
        /// Creates an and serch condition. AND ( Conditions [...] )
        /// </summary>
        /// <param name="conditionExpression">A condition expression for adding more conditions to the search.</param>
        public void And(SearchExpression<ConditionExpression> conditionExpression)
        {
            if (conditionExpression != null)
            {
                ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
                conditionExpression(new ConditionExpression(section));
                if (section.Conditions.Count > 0)
                {
                    this.section.Sections.Add(section);
                }
            }
        }

        /// <summary>
        /// Creates an or serch condition. Or ( Conditions [...] )
        /// </summary>
        /// <param name="conditionExpression">A condition expression for adding more conditions to the search.</param>
        public void Or(SearchExpression<ConditionExpression> conditionExpression)
        {
            if (conditionExpression != null)
            {
                ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.Or);
                conditionExpression(new ConditionExpression(section));
                if (section.Conditions.Count > 0)
                {
                    this.section.Sections.Add(section);
                }
            }
        }

        /// <summary>
        /// Creates an and serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be equal to.</param>
        public void AndEquals(string fieldName, object equalsValue)
        {
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.And, ComplexSearchOperator.Equal));
        }

        /// <summary>
        /// Creates an and serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must not be equal to.</param>
        public void AndNotEquals(string fieldName, object equalsValue)
        {
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.And, ComplexSearchOperator.NotEqual));
        }

        /// <summary>
        /// Creates an and serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than.</param>
        public void AndGreater(string fieldName, object equalsValue)
        {
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.And, ComplexSearchOperator.GreaterThan));
        }

        /// <summary>
        /// Creates an and serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than.</param>
        public void AndLess(string fieldName, object equalsValue)
        {
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.And, ComplexSearchOperator.LessThan));
        }

        /// <summary>
        /// Creates an and serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than or equal to.</param>
        public void AndGreaterOrEqual(string fieldName, object equalsValue)
        {
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.And, ComplexSearchOperator.GreaterThanOrEqual));
        }

        /// <summary>
        /// Creates an and serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than or equal to.</param>
        public void AndLessOrEqual(string fieldName, object equalsValue)
        {
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.And, ComplexSearchOperator.LessThanOrEqual));
        }

        /// <summary>
        /// Creates an and serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be between.</param>
        public void AndBetween(string fieldName, object equalsValues)
        {
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValues, ComplexSearchType.And, ComplexSearchOperator.Between));
        }

        /// <summary>
        /// Creates an and serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be like.</param>
        public void AndLike(string fieldName, object equalsValue)
        {
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.And, ComplexSearchOperator.Like));
        }

        /// <summary>
        /// Creates an and serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be in.</param>
        public void AndIn(string fieldName, object equalsValues)
        {
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValues, ComplexSearchType.And, ComplexSearchOperator.In));
        }

        /// <summary>
        /// Creates an or serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be equal to.</param>
        public void OrEquals(string fieldName, object equalsValue)
        {
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.Or, ComplexSearchOperator.Equal));
        }

        /// <summary>
        /// Creates an or serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must not be equal to.</param>
        public void OrNotEquals(string fieldName, object equalsValue)
        {
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.Or, ComplexSearchOperator.NotEqual));
        }

        /// <summary>
        /// Creates an or serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than.</param>
        public void OrGreater(string fieldName, object equalsValue)
        {
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.Or, ComplexSearchOperator.GreaterThan));
        }

        /// <summary>
        /// Creates an or serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than.</param>
        public void OrLess(string fieldName, object equalsValue)
        {
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.Or, ComplexSearchOperator.LessThan));
        }

        /// <summary>
        /// Creates an or serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than or equal to.</param>
        public void OrGreaterOrEqual(string fieldName, object equalsValue)
        {
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.Or, ComplexSearchOperator.GreaterThanOrEqual));
        }

        /// <summary>
        /// Creates an or serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than or equal to.</param>
        public void OrLessOrEqual(string fieldName, object equalsValue)
        {
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.Or, ComplexSearchOperator.LessThanOrEqual));
        }

        /// <summary>
        /// Creates an or serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be between.</param>
        public void OrBetween(string fieldName, object equalsValues)
        {
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValues, ComplexSearchType.Or, ComplexSearchOperator.Between));
        }

        /// <summary>
        /// Creates an or serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be like.</param>
        public void OrLike(string fieldName, object equalsValue)
        {
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.Or, ComplexSearchOperator.Like));
        }

        /// <summary>
        /// Creates an or serch condition.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be in.</param>
        public void OrIn(string fieldName, object equalsValues)
        {
            section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValues, ComplexSearchType.Or, ComplexSearchOperator.In));
        }

        #endregion Methods
    }
}