using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataRelational.Schema;
using System.Data.Common;
using System.Data;
using DataRelational.Search;

namespace DataRelational.Script
{
    /// <summary>
    /// The interface for all database scripters
    /// </summary>
    internal interface IDatabaseScripter
    {
        /// <summary>
        /// Gets the type of database.
        /// </summary>
        DatabaseType DatabaseType { get; }

        /// <summary>
        /// Creates the database schema.
        /// </summary>
        /// <param name="connectionName">The name of the connection string.</param>
        /// <param name="append">Whether to append the database changes.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="DataRelational.Script.ScriptException"></exception>
        /// <exception cref="DataRelational.Script.ConnectionStringException"></exception>
        void ScriptDatabase(string connectionName, bool append);

        /// <summary>
        /// Creates or modifies the schema for an object Type.
        /// </summary>
        /// <param name="objectType">The System.Type of the object.  Must at least implement the IDataObject interface. </param>
        /// <param name="connectionName">The name of the connection string.</param>
        /// <returns>The data object type</returns>
        DataObjectSchema ScriptObjectSchema(Type objectType, string connectionName);

        /// <summary>
        /// Gets the schema for an object Type.
        /// </summary>
        /// <param name="objectTypeId">The object type identifier. </param>
        /// <param name="connectionName">The name of the connection string.</param>
        /// <returns>The data object type</returns>
        DataObjectSchema GetObjectSchema(short objectTypeId, string connectionName);

        /// <summary>
        /// Gets the next available identifier for an object Type.
        /// </summary>
        /// <param name="objectType">The System.Type of the object.  Must at least implement the IDataObject interface.</param>
        /// <param name="connectionName">The name of the connection string.</param>
        /// <returns>The next available identifier</returns>
        long GetNextIdentifier(Type objectType, string connectionName);

        /// <summary>
        /// Opens a connection to the database.
        /// </summary>
        /// <param name="connectionName">The name of the connection string.</param>
        DbConnection GetDatabaseConnection(string connectionName);

        /// <summary>
        /// Gets a data adapter.
        /// </summary>
        DbDataAdapter GetDataAdapter();
    }
}
