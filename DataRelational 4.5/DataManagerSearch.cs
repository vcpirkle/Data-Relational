using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataRelational.Search;
using DataRelational.Cache;
using DataRelational.Schema;
using System.Data;

namespace DataRelational
{
    /// <summary>
    /// Used to perform a search operation.
    /// </summary>
    public class DataManagerSearch
    {
        #region Constructor

        internal DataManagerSearch() { }

        #endregion Constructor

        #region Methods

        #region Where

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> Where<T>(SearchExpression<SearchBuilder> searchExpression) where T : IDataObject
        {
            return Where<T>(null, null, searchExpression, null, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> Where<T>(SearchExpression<SearchBuilder> searchExpression, string connectionName) where T : IDataObject
        {
            return Where<T>(null, null, searchExpression, null, false, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be equal to.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> Where<T>(string fieldName, object equalsValue) where T : IDataObject
        {
            return Where<T>(fieldName, equalsValue, null, null, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be equal to.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> Where<T>(string fieldName, object equalsValue, string connectionName) where T : IDataObject
        {
            return Where<T>(fieldName, equalsValue, null, null, false, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> Where<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression) where T : IDataObject
        {
            return Where<T>(fieldName, equalsValue, searchExpression, null, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> Where<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, string connectionName) where T : IDataObject
        {
            return Where<T>(fieldName, equalsValue, searchExpression, null, false, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> Where<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp) where T : IDataObject
        {
            return Where<T>(fieldName, equalsValue, searchExpression, timestamp, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> Where<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, string connectionName) where T : IDataObject
        {
            return Where<T>(fieldName, equalsValue, searchExpression, timestamp, false, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> Where<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted) where T : IDataObject
        {
            return Where<T>(fieldName, equalsValue, searchExpression, timestamp, includeDeleted, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> Where<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted, string connectionName) where T : IDataObject
        {
            return Where<T>(fieldName, equalsValue, searchExpression, timestamp, includeDeleted, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <param name="includeArchive">Whether to include archived objects in the search.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> Where<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted, bool includeArchive) where T : IDataObject
        {
            return Where<T>(fieldName, equalsValue, searchExpression, timestamp, includeDeleted, includeArchive, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <param name="includeArchive">Whether to include archived objects in the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> Where<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted, bool includeArchive, string connectionName) where T : IDataObject
        {
            ComplexSearch search = new ComplexSearch(ComplexSearchMode.Search)
            {
                SearchObjectType = typeof(T)
            };
            if (fieldName != null)
            {
                ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
                section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.And, ComplexSearchOperator.Equal));
                search.Sections.Add(section);
            }
            if (searchExpression != null) { searchExpression(new SearchBuilder(search)); }

            return _Search<T>(search, timestamp, includeDeleted, includeArchive, connectionName);
        }
        
        #endregion Where

        #region Where Not

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must not be equal to.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereNot<T>(string fieldName, object equalsValue) where T : IDataObject
        {
            return WhereNot<T>(fieldName, equalsValue, null, null, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must not be equal to.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereNot<T>(string fieldName, object equalsValue, string connectionName) where T : IDataObject
        {
            return WhereNot<T>(fieldName, equalsValue, null, null, false, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must not be equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereNot<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression) where T : IDataObject
        {
            return WhereNot<T>(fieldName, equalsValue, searchExpression, null, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must not be equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereNot<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, string connectionName) where T : IDataObject
        {
            return WhereNot<T>(fieldName, equalsValue, searchExpression, null, false, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must not be equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereNot<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp) where T : IDataObject
        {
            return WhereNot<T>(fieldName, equalsValue, searchExpression, timestamp, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must not be equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereNot<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, string connectionName) where T : IDataObject
        {
            return WhereNot<T>(fieldName, equalsValue, searchExpression, timestamp, false, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must not be equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereNot<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted) where T : IDataObject
        {
            return WhereNot<T>(fieldName, equalsValue, searchExpression, timestamp, includeDeleted, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must not be equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereNot<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted, string connectionName) where T : IDataObject
        {
            return WhereNot<T>(fieldName, equalsValue, searchExpression, timestamp, includeDeleted, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must not be equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <param name="includeArchive">Whether to include archived objects in the search.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereNot<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted, bool includeArchive) where T : IDataObject
        {
            return WhereNot<T>(fieldName, equalsValue, searchExpression, timestamp, includeDeleted, includeArchive, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must not be equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <param name="includeArchive">Whether to include archived objects in the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereNot<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted, bool includeArchive, string connectionName) where T : IDataObject
        {
            ComplexSearch search = new ComplexSearch(ComplexSearchMode.Search)
            {
                SearchObjectType = typeof(T)
            };
            if (fieldName != null)
            {
                ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
                section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.And, ComplexSearchOperator.NotEqual));
                search.Sections.Add(section);
            }
            if (searchExpression != null) { searchExpression(new SearchBuilder(search)); }

            return _Search<T>(search, timestamp, includeDeleted, includeArchive, connectionName);
        }

        #endregion Where Not

        #region Where Greater

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereGreater<T>(string fieldName, object equalsValue) where T : IDataObject
        {
            return WhereGreater<T>(fieldName, equalsValue, null, null, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereGreater<T>(string fieldName, object equalsValue, string connectionName) where T : IDataObject
        {
            return WhereGreater<T>(fieldName, equalsValue, null, null, false, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereGreater<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression) where T : IDataObject
        {
            return WhereGreater<T>(fieldName, equalsValue, searchExpression, null, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereGreater<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, string connectionName) where T : IDataObject
        {
            return WhereGreater<T>(fieldName, equalsValue, searchExpression, null, false, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereGreater<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp) where T : IDataObject
        {
            return WhereGreater<T>(fieldName, equalsValue, searchExpression, timestamp, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereGreater<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, string connectionName) where T : IDataObject
        {
            return WhereGreater<T>(fieldName, equalsValue, searchExpression, timestamp, false, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereGreater<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted) where T : IDataObject
        {
            return WhereGreater<T>(fieldName, equalsValue, searchExpression, timestamp, includeDeleted, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereGreater<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted, string connectionName) where T : IDataObject
        {
            return WhereGreater<T>(fieldName, equalsValue, searchExpression, timestamp, includeDeleted, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <param name="includeArchive">Whether to include archived objects in the search.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereGreater<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted, bool includeArchive) where T : IDataObject
        {
            return WhereGreater<T>(fieldName, equalsValue, searchExpression, timestamp, includeDeleted, includeArchive, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <param name="includeArchive">Whether to include archived objects in the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereGreater<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted, bool includeArchive, string connectionName) where T : IDataObject
        {
            ComplexSearch search = new ComplexSearch(ComplexSearchMode.Search)
            {
                SearchObjectType = typeof(T)
            };
            if (fieldName != null)
            {
                ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
                section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.And, ComplexSearchOperator.GreaterThan));
                search.Sections.Add(section);
            }
            if (searchExpression != null) { searchExpression(new SearchBuilder(search)); }

            return _Search<T>(search, timestamp, includeDeleted, includeArchive, connectionName);
        }

        #endregion Where Greater

        #region Where Less

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLess<T>(string fieldName, object equalsValue) where T : IDataObject
        {
            return WhereLess<T>(fieldName, equalsValue, null, null, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLess<T>(string fieldName, object equalsValue, string connectionName) where T : IDataObject
        {
            return WhereLess<T>(fieldName, equalsValue, null, null, false, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLess<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression) where T : IDataObject
        {
            return WhereLess<T>(fieldName, equalsValue, searchExpression, null, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLess<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, string connectionName) where T : IDataObject
        {
            return WhereLess<T>(fieldName, equalsValue, searchExpression, null, false, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLess<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp) where T : IDataObject
        {
            return WhereLess<T>(fieldName, equalsValue, searchExpression, timestamp, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLess<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, string connectionName) where T : IDataObject
        {
            return WhereLess<T>(fieldName, equalsValue, searchExpression, timestamp, false, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLess<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted) where T : IDataObject
        {
            return WhereLess<T>(fieldName, equalsValue, searchExpression, timestamp, includeDeleted, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLess<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted, string connectionName) where T : IDataObject
        {
            return WhereLess<T>(fieldName, equalsValue, searchExpression, timestamp, includeDeleted, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <param name="includeArchive">Whether to include archived objects in the search.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLess<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted, bool includeArchive) where T : IDataObject
        {
            return WhereLess<T>(fieldName, equalsValue, searchExpression, timestamp, includeDeleted, includeArchive, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <param name="includeArchive">Whether to include archived objects in the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLess<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted, bool includeArchive, string connectionName) where T : IDataObject
        {
            ComplexSearch search = new ComplexSearch(ComplexSearchMode.Search)
            {
                SearchObjectType = typeof(T)
            };
            if (fieldName != null)
            {
                ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
                section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.And, ComplexSearchOperator.LessThan));
                search.Sections.Add(section);
            }
            if (searchExpression != null) { searchExpression(new SearchBuilder(search)); }

            return _Search<T>(search, timestamp, includeDeleted, includeArchive, connectionName);
        }

        #endregion Where Less

        #region Where Greater Or Equal

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than or equal to.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereGreaterOrEqual<T>(string fieldName, object equalsValue) where T : IDataObject
        {
            return WhereGreaterOrEqual<T>(fieldName, equalsValue, null, null, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than or equal to.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereGreaterOrEqual<T>(string fieldName, object equalsValue, string connectionName) where T : IDataObject
        {
            return WhereGreaterOrEqual<T>(fieldName, equalsValue, null, null, false, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than or equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereGreaterOrEqual<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression) where T : IDataObject
        {
            return WhereGreaterOrEqual<T>(fieldName, equalsValue, searchExpression, null, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than or equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereGreaterOrEqual<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, string connectionName) where T : IDataObject
        {
            return WhereGreaterOrEqual<T>(fieldName, equalsValue, searchExpression, null, false, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than or equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereGreaterOrEqual<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp) where T : IDataObject
        {
            return WhereGreaterOrEqual<T>(fieldName, equalsValue, searchExpression, timestamp, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than or equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereGreaterOrEqual<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, string connectionName) where T : IDataObject
        {
            return WhereGreaterOrEqual<T>(fieldName, equalsValue, searchExpression, timestamp, false, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than or equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereGreaterOrEqual<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted) where T : IDataObject
        {
            return WhereGreaterOrEqual<T>(fieldName, equalsValue, searchExpression, timestamp, includeDeleted, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than or equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereGreaterOrEqual<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted, string connectionName) where T : IDataObject
        {
            return WhereGreaterOrEqual<T>(fieldName, equalsValue, searchExpression, timestamp, includeDeleted, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than or equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <param name="includeArchive">Whether to include archived objects in the search.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereGreaterOrEqual<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted, bool includeArchive) where T : IDataObject
        {
            return WhereGreaterOrEqual<T>(fieldName, equalsValue, searchExpression, timestamp, includeDeleted, includeArchive, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be greater than or equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <param name="includeArchive">Whether to include archived objects in the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereGreaterOrEqual<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted, bool includeArchive, string connectionName) where T : IDataObject
        {
            ComplexSearch search = new ComplexSearch(ComplexSearchMode.Search)
            {
                SearchObjectType = typeof(T)
            };
            if (fieldName != null)
            {
                ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
                section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.And, ComplexSearchOperator.GreaterThanOrEqual));
                search.Sections.Add(section);
            }
            if (searchExpression != null) { searchExpression(new SearchBuilder(search)); }

            return _Search<T>(search, timestamp, includeDeleted, includeArchive, connectionName);
        }

        #endregion Where Greater Or Equal

        #region Where Less Or Equal

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than or equal to.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLessOrEqual<T>(string fieldName, object equalsValue) where T : IDataObject
        {
            return WhereLessOrEqual<T>(fieldName, equalsValue, null, null, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than or equal to.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLessOrEqual<T>(string fieldName, object equalsValue, string connectionName) where T : IDataObject
        {
            return WhereLessOrEqual<T>(fieldName, equalsValue, null, null, false, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than or equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLessOrEqual<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression) where T : IDataObject
        {
            return WhereLessOrEqual<T>(fieldName, equalsValue, searchExpression, null, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than or equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLessOrEqual<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, string connectionName) where T : IDataObject
        {
            return WhereLessOrEqual<T>(fieldName, equalsValue, searchExpression, null, false, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than or equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLessOrEqual<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp) where T : IDataObject
        {
            return WhereLessOrEqual<T>(fieldName, equalsValue, searchExpression, timestamp, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than or equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLessOrEqual<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, string connectionName) where T : IDataObject
        {
            return WhereLessOrEqual<T>(fieldName, equalsValue, searchExpression, timestamp, false, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than or equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLessOrEqual<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted) where T : IDataObject
        {
            return WhereLessOrEqual<T>(fieldName, equalsValue, searchExpression, timestamp, includeDeleted, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than or equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLessOrEqual<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted, string connectionName) where T : IDataObject
        {
            return WhereLessOrEqual<T>(fieldName, equalsValue, searchExpression, timestamp, includeDeleted, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than or equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <param name="includeArchive">Whether to include archived objects in the search.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLessOrEqual<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted, bool includeArchive) where T : IDataObject
        {
            return WhereLessOrEqual<T>(fieldName, equalsValue, searchExpression, timestamp, includeDeleted, includeArchive, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be less than or equal to.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <param name="includeArchive">Whether to include archived objects in the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLessOrEqual<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted, bool includeArchive, string connectionName) where T : IDataObject
        {
            ComplexSearch search = new ComplexSearch(ComplexSearchMode.Search)
            {
                SearchObjectType = typeof(T)
            };
            if (fieldName != null)
            {
                ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
                section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.And, ComplexSearchOperator.LessThanOrEqual));
                search.Sections.Add(section);
            }
            if (searchExpression != null) { searchExpression(new SearchBuilder(search)); }

            return _Search<T>(search, timestamp, includeDeleted, includeArchive, connectionName);
        }

        #endregion Where Less Or Equal

        #region Where Between

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be between.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereBetween<T>(string fieldName, object equalsValues) where T : IDataObject
        {
            return WhereBetween<T>(fieldName, equalsValues, null, null, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be between.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereBetween<T>(string fieldName, object equalsValues, string connectionName) where T : IDataObject
        {
            return WhereBetween<T>(fieldName, equalsValues, null, null, false, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be between.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereBetween<T>(string fieldName, object equalsValues, SearchExpression<SearchBuilder> searchExpression) where T : IDataObject
        {
            return WhereBetween<T>(fieldName, equalsValues, searchExpression, null, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be between.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereBetween<T>(string fieldName, object equalsValues, SearchExpression<SearchBuilder> searchExpression, string connectionName) where T : IDataObject
        {
            return WhereBetween<T>(fieldName, equalsValues, searchExpression, null, false, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be between.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereBetween<T>(string fieldName, object equalsValues, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp) where T : IDataObject
        {
            return WhereBetween<T>(fieldName, equalsValues, searchExpression, timestamp, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be between.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereBetween<T>(string fieldName, object equalsValues, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, string connectionName) where T : IDataObject
        {
            return WhereBetween<T>(fieldName, equalsValues, searchExpression, timestamp, false, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be between.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereBetween<T>(string fieldName, object equalsValues, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted) where T : IDataObject
        {
            return WhereBetween<T>(fieldName, equalsValues, searchExpression, timestamp, includeDeleted, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be between.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereBetween<T>(string fieldName, object equalsValues, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted, string connectionName) where T : IDataObject
        {
            return WhereBetween<T>(fieldName, equalsValues, searchExpression, timestamp, includeDeleted, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be between.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <param name="includeArchive">Whether to include archived objects in the search.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereBetween<T>(string fieldName, object equalsValues, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted, bool includeArchive) where T : IDataObject
        {
            return WhereBetween<T>(fieldName, equalsValues, searchExpression, timestamp, includeDeleted, includeArchive, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be between.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <param name="includeArchive">Whether to include archived objects in the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereBetween<T>(string fieldName, object equalsValues, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted, bool includeArchive, string connectionName) where T : IDataObject
        {
            ComplexSearch search = new ComplexSearch(ComplexSearchMode.Search)
            {
                SearchObjectType = typeof(T)
            };
            if (fieldName != null)
            {
                ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
                section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValues, ComplexSearchType.And, ComplexSearchOperator.Between));
                search.Sections.Add(section);
            }
            if (searchExpression != null) { searchExpression(new SearchBuilder(search)); }

            return _Search<T>(search, timestamp, includeDeleted, includeArchive, connectionName);
        }

        #endregion Where Between

        #region Where Like

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be like.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLike<T>(string fieldName, object equalsValue) where T : IDataObject
        {
            return WhereLike<T>(fieldName, equalsValue, null, null, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be like.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLike<T>(string fieldName, object equalsValue, string connectionName) where T : IDataObject
        {
            return WhereLike<T>(fieldName, equalsValue, null, null, false, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be like.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLike<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression) where T : IDataObject
        {
            return WhereLike<T>(fieldName, equalsValue, searchExpression, null, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be like.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLike<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, string connectionName) where T : IDataObject
        {
            return WhereLike<T>(fieldName, equalsValue, searchExpression, null, false, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be like.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLike<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp) where T : IDataObject
        {
            return WhereLike<T>(fieldName, equalsValue, searchExpression, timestamp, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be like.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLike<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, string connectionName) where T : IDataObject
        {
            return WhereLike<T>(fieldName, equalsValue, searchExpression, timestamp, false, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be like.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLike<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted) where T : IDataObject
        {
            return WhereLike<T>(fieldName, equalsValue, searchExpression, timestamp, includeDeleted, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be like.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLike<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted, string connectionName) where T : IDataObject
        {
            return WhereLike<T>(fieldName, equalsValue, searchExpression, timestamp, includeDeleted, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be like.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <param name="includeArchive">Whether to include archived objects in the search.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLike<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted, bool includeArchive) where T : IDataObject
        {
            return WhereLike<T>(fieldName, equalsValue, searchExpression, timestamp, includeDeleted, includeArchive, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValue">The value the field must be like.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <param name="includeArchive">Whether to include archived objects in the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereLike<T>(string fieldName, object equalsValue, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted, bool includeArchive, string connectionName) where T : IDataObject
        {
            ComplexSearch search = new ComplexSearch(ComplexSearchMode.Search)
            {
                SearchObjectType = typeof(T)
            };
            if (fieldName != null)
            {
                ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
                section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValue, ComplexSearchType.And, ComplexSearchOperator.Like));
                search.Sections.Add(section);
            }
            if (searchExpression != null) { searchExpression(new SearchBuilder(search)); }

            return _Search<T>(search, timestamp, includeDeleted, includeArchive, connectionName);
        }

        #endregion Where Like

        #region Where In

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be in.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereIn<T>(string fieldName, object equalsValues) where T : IDataObject
        {
            return WhereIn<T>(fieldName, equalsValues, null, null, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be in.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereIn<T>(string fieldName, object equalsValues, string connectionName) where T : IDataObject
        {
            return WhereIn<T>(fieldName, equalsValues, null, null, false, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be in.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereIn<T>(string fieldName, object equalsValues, SearchExpression<SearchBuilder> searchExpression) where T : IDataObject
        {
            return WhereIn<T>(fieldName, equalsValues, searchExpression, null, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be in.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereIn<T>(string fieldName, object equalsValues, SearchExpression<SearchBuilder> searchExpression, string connectionName) where T : IDataObject
        {
            return WhereIn<T>(fieldName, equalsValues, searchExpression, null, false, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be in.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereIn<T>(string fieldName, object equalsValues, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp) where T : IDataObject
        {
            return WhereIn<T>(fieldName, equalsValues, searchExpression, timestamp, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be in.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereIn<T>(string fieldName, object equalsValues, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, string connectionName) where T : IDataObject
        {
            return WhereIn<T>(fieldName, equalsValues, searchExpression, timestamp, false, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be in.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereIn<T>(string fieldName, object equalsValues, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted) where T : IDataObject
        {
            return WhereIn<T>(fieldName, equalsValues, searchExpression, timestamp, includeDeleted, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be in.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereIn<T>(string fieldName, object equalsValues, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted, string connectionName) where T : IDataObject
        {
            return WhereIn<T>(fieldName, equalsValues, searchExpression, timestamp, includeDeleted, false, connectionName);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be in.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <param name="includeArchive">Whether to include archived objects in the search.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereIn<T>(string fieldName, object equalsValues, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted, bool includeArchive) where T : IDataObject
        {
            return WhereIn<T>(fieldName, equalsValues, searchExpression, timestamp, includeDeleted, includeArchive, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Searches for the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to search for.</typeparam>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="equalsValues">The values the field must be in.</param>
        /// <param name="searchExpression">A search expression for adding more conditions to the search.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects in the search.</param>
        /// <param name="includeArchive">Whether to include archived objects in the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>The list of objects found.</returns>
        public List<T> WhereIn<T>(string fieldName, object equalsValues, SearchExpression<SearchBuilder> searchExpression, DateTime? timestamp, bool includeDeleted, bool includeArchive, string connectionName) where T : IDataObject
        {
            ComplexSearch search = new ComplexSearch(ComplexSearchMode.Search)
            {
                SearchObjectType = typeof(T)
            };
            if (fieldName != null)
            {
                ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
                section.Conditions.Add(new ComplexSearchElement(fieldName, equalsValues, ComplexSearchType.And, ComplexSearchOperator.In));
                search.Sections.Add(section);
            }
            if (searchExpression != null) { searchExpression(new SearchBuilder(search)); }

            return _Search<T>(search, timestamp, includeDeleted, includeArchive, connectionName);
        }

        #endregion Where In

        #region Search

        private List<T> _Search<T>(ComplexSearch search, DateTime? timestamp, bool includeDeleted, bool includeArchive, string connectionName) where T : IDataObject
        {
            DataObjectReflection objectReflection = null;

            if (!DataManager.ReflectionCache.Contains(typeof(T)))
            {
                //If the reflection cache has never seen the object type, we need to make sure we have a valid place to store the object.
                //The script manager will adjust the database footprint for the object if necessary.
                DataObjectSchema dataObjectSchema = DataManager.ScriptManager.ScriptObjectSchema(typeof(T));
                objectReflection = new DataObjectReflection(dataObjectSchema);
                DataManager.ReflectionCache.Add(objectReflection);
            }
            else
            {
                objectReflection = DataManager.ReflectionCache[typeof(T)];
            }

            ComplexSearchSection section = new ComplexSearchSection(ComplexSearchType.And);
            section.Conditions.Add(new ComplexSearchElement("_ObjectTypeId", objectReflection.Schema._Id, ComplexSearchType.And, ComplexSearchOperator.Equal));
            search.Sections.Add(section);

            if (timestamp.HasValue && search.Sections.Find("_Timestamp") == null)
            {
                section = new ComplexSearchSection(ComplexSearchType.And);
                section.Conditions.Add(new ComplexSearchElement("_Timestamp", timestamp.Value.ToUniversalTime(), ComplexSearchType.And, ComplexSearchOperator.LessThanOrEqual));
                search.Sections.Add(section);
            }

            search.SearchMode = ComplexSearchMode.Search;
            search.IncludeDeleted = includeDeleted;
            search.IncludeArchived = includeArchive;
            SearchContext context = DataManager.DataReader.GetRelationships(new SearchContext(search), connectionName);
            context.connectionName = connectionName;

            StringBuilder builder = new StringBuilder();
            DataManager.DataReader.BeginRead(builder);
            DataManager.DataReader.AddGetObjectQuery(builder, context);
            DataSet result = DataManager.DataReader.EndRead(builder, connectionName);
            context.loadedDepth += 3;
            DataObjectBuilder.BuildObjects(context, null, result);
            Dictionary<long, IDataObject> retrieveHash = null;
            if (context.RetrievedObjects.TryGetValue(objectReflection.Schema._Id, out retrieveHash))
            {
                //Add the first level objects from the search results to the return list
                List<T> returnObjects = new List<T>();
                if (context.FirstLevel.Count > 0)
                {
                    DataObjectReflection objReflection = DataManager.ReflectionCache[context.FirstLevel[0].TypeId];
                    foreach (DataKey dKey in context.FirstLevel)
                    {
                        Dictionary<long, IDataObject> objectHash = null;
                        if (context.RetrievedObjects.TryGetValue(dKey.TypeId, out objectHash))
                        {
                            IDataObject obj = null;
                            if (objectHash.TryGetValue(dKey.Id, out obj))
                            {
                                returnObjects.Add((T)obj);
                            }
                        }
                    }
                }
                return returnObjects;
            }
            return new List<T>();
        }

        #endregion Search

        #endregion Methods
    }
}