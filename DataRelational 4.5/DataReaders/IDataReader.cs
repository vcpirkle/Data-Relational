using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataRelational.Search;
using System.Data;

namespace DataRelational.DataReaders
{
    /// <summary>
    /// The interface for all database readers.
    /// </summary>
    public interface IDataReader
    {
        /// <summary>
        /// Begins a batch read process.
        /// </summary>
        /// <param name="builder">The string build to write to.</param>
        void BeginRead(StringBuilder builder);

        /// <summary>
        /// Adds a get query to the batch read process.
        /// </summary>
        /// <param name="builder">The string builder to write to.</param>
        /// <param name="context">The context of the search.</param>
        void AddGetObjectQuery(StringBuilder builder, SearchContext context);

        /// <summary>
        /// Ends a batch read process and executes the batch.
        /// </summary>
        /// <param name="builder">The string build to write to.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>A data set containing the queried objects.  The data set will be formatted as Tables: {TypeName1, TypeName2, ... Relationships}</returns>
        DataSet EndRead(StringBuilder builder, string connectionName);

        /// <summary>
        /// Gets the next three levels of relationships.
        /// </summary>
        /// <param name="context">The context of the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>A search context with the next three levels of relationships.</returns>
        SearchContext GetRelationships(SearchContext context, string connectionName);
    }
}
