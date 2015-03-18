using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataRelational.Attributes;
using DataRelational.Cache;

namespace DataRelational.DataWriters
{
    /// <summary>
    /// The interface for all database writers.
    /// </summary>
    public interface IDataWriter
    {
        /// <summary>
        /// Begins a batch write process.
        /// </summary>
        /// <param name="builder">The string builder to write to.</param>
        void BeginWrite(StringBuilder builder);

        /// <summary>
        /// Writes a data relational object to the batch.
        /// </summary>
        /// <param name="builder">The string builder to write to.</param>
        /// <param name="obj">The data relational object to write.</param>
        /// <param name="objAttribute">The savable object attribute associated with the data relational object.</param>
        void Write(StringBuilder builder, IDataObject obj, SavableObjectAttribute objAttribute);

        /// <summary>
        /// Writes the parent child relationships between a parent and child object to the batch.
        /// </summary>
        /// <param name="builder">The string builder to write to.</param>
        /// <param name="obj">The data relational object to write relationshps for.</param>
        /// <param name="objAttribute">The savable object attribute associated with the data relational parent object.</param>
        /// <param name="writtenRelationships">The list or relationships written in this save context.</param>
        void WriteRelationships(StringBuilder builder, IDataObject obj, SavableObjectAttribute objAttribute, List<Relationship> writtenRelationships);

        /// <summary>
        /// Ends a batch write process and executes the batch.
        /// </summary>
        /// <param name="builder">The string builder to write to.</param>
        /// <param name="connectionName">The name of the connection to write to.</param>
        /// <returns>A data table containing the columns {_Id, _RevisionId, _DataObjectTypeId, and WriteTime}</returns>
        DataTable EndWrite(StringBuilder builder, string connectionName);
    }
}
