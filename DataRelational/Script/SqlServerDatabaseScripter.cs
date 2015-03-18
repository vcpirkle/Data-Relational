using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using DataRelational.Schema;
using DataRelational.Search;
using DataRelational.Cache;

namespace DataRelational.Script
{
    class SqlServerDatabaseScripter : IDatabaseScripter
    {
        #region IDatabaseScripter Members

        /// <summary>
        /// Gets the type of database
        /// </summary>
        public DatabaseType DatabaseType
        {
            get { return DatabaseType.SqlServer; }
        }

        /// <summary>
        /// Creates the database schema
        /// </summary>
        /// <param name="connectionName">The name of the connection string.</param>
        /// <param name="append">Whether to append the database changes</param>
        /// <remarks>
        /// The connection string should be formatted as Data Source={0};Initial Catalog={1};Integrated Security=True; or
        /// Data Source={0};Initial Catalog={1};User Id={2}; password={3}
        /// </remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="DataRelational.Script.ScriptException"></exception>
        /// <exception cref="DataRelational.Script.ConnectionStringException"></exception>
        public void ScriptDatabase(string connectionName, bool append)
        {
            //Validate the connection string
            string server = null, database = null, options = null, parseError = null;
            if (connectionName == null)
            {
                throw new ArgumentNullException("connectionName");
            }
            ConnectionStringElement cs = DataManager.Settings.ConnectionStrings[connectionName];
            if (cs == null)
                throw new ArgumentNullException("connectionName");

            if (!ScriptCommon.Parser.TryParseSqlConnectionString(cs.Connection, out server, out database, out options, out parseError))
            {
                throw new ConnectionStringException(parseError);
            }

            //Connection to the master database
            string cString = string.Format("Data Source={0};Initial Catalog={1};{2}", server, "Master", options);
            SqlConnection connection = new SqlConnection(cString);
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                throw new ScriptException("Could not establish a connection with the Master database.", ex);
            }

            try
            {
                if (!append)
                {
                    DropDatabase(connection, database);
                }
            }
            catch (Exception ex)
            {
                throw new ScriptException(string.Format("Could not drop the {0} database.", database), ex);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }

            connection = new SqlConnection(cString);
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                throw new ScriptException("Could not establish a connection with the Master database.", ex);
            }

            try
            {
                CreateDatabase(connection, database);
            }
            catch (Exception ex)
            {
                throw new ScriptException(string.Format("Could not create the {0} database.", database), ex);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Creates or modifies the schema for an object Type
        /// </summary>
        /// <param name="objectType">The System.Type of the object.  Must at least implement the IDataObject interface. </param>
        /// <param name="connectionName">The name of the connection string.</param>
        /// <returns>The data object type</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="DataRelational.Script.ScriptException"></exception>
        /// <exception cref="DataRelational.Script.ConnectionStringException"></exception>
        public DataObjectSchema ScriptObjectSchema(Type objectType, string connectionName)
        {
            DataObjectSchema dataObjectSchema = null;

            //Connect to the database
            using (DbConnection connection = GetDatabaseConnection(connectionName))
            {
                DataObjectSchemaFactory schemaFactory = new DataObjectSchemaFactory();

                try
                {
                    dataObjectSchema = schemaFactory.GetSchema(connection, objectType);
                }
                catch (Exception ex)
                {
                    throw new ScriptException("Could not retrieve the object schema from the database for the type " + objectType.Namespace + "." + objectType.Name + ".", ex);
                }

                if (dataObjectSchema != null)
                {
                    DataObjectSchema dataObjectSchemaMetaData = null;
                    try
                    {
                        dataObjectSchemaMetaData = schemaFactory.GetSchema(objectType);
                    }
                    catch (Exception ex)
                    {
                        throw new ScriptException("Could not retrieve the object schema from the meta data for the type " + objectType.Namespace + "." + objectType.Name + ".", ex);
                    }

                    try
                    {
                        dataObjectSchema = schemaFactory.ModifySchema(connection, objectType, dataObjectSchemaMetaData, dataObjectSchema);
                    }
                    catch (Exception ex)
                    {
                        throw new ScriptException("Could not alter the object schema for the type " + objectType.Namespace + "." + objectType.Name + ".", ex);
                    }
                }
                else
                {
                    //This is a new data relational object
                    try
                    {
                        DataObjectSchema newSchema = schemaFactory.GetSchema(objectType);
                        dataObjectSchema = schemaFactory.AddSchema(connection, newSchema);
                    }
                    catch (Exception ex)
                    {
                        throw new ScriptException("Could not create the object type for the type " + objectType.Namespace + "." + objectType.Name + ".", ex);
                    }
                }

                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            if (dataObjectSchema == null)
                throw new ScriptException("Could not create the object type for the type " + objectType.Namespace + "." + objectType.Name + ".");

            return dataObjectSchema;
        }

        /// <summary>
        /// Gets the schema for an object Type.
        /// </summary>
        /// <param name="objectTypeId">The object type identifier. </param>
        /// <param name="connectionName">The name of the connection string.</param>
        /// <returns>The data object type</returns>
        public DataObjectSchema GetObjectSchema(short objectTypeId, string connectionName)
        {
            DataObjectSchema dataObjectSchema = null;

            //Connect to the database
            using (DbConnection connection = GetDatabaseConnection(connectionName))
            {
                DataObjectSchemaFactory schemaFactory = new DataObjectSchemaFactory();

                try
                {
                    dataObjectSchema = schemaFactory.GetSchema(connection, objectTypeId);
                }
                catch (Exception ex)
                {
                    throw new ScriptException("Could not retrieve the object schema from the database for the type " + objectTypeId + ".", ex);
                }

                return dataObjectSchema;
            }
        }

        /// <summary>
        /// Gets the next available identifier for an object Type
        /// </summary>
        /// <param name="objectType">The System.Type of the object.  Must at least implement the IDataObject interface.</param>
        /// <param name="connectionName">The name of the connection string.</param>
        /// <returns>The next available identifier</returns>
        public long GetNextIdentifier(Type objectType, string connectionName)
        {
            if (connectionName == null)
            {
                throw new ArgumentNullException("connectionName");
            }
            ConnectionStringElement cString = DataManager.Settings.ConnectionStrings[connectionName];
            if (cString == null)
                throw new ArgumentNullException("connectionName");

            long nextIdentifier = 0;

            //Connect to the database
            using (DbConnection connection = GetDatabaseConnection(cString.Name))
            {
                try
                {
                    DbCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("SELECT Max(_Id) + 1 AS NextIdentifier FROM");
                    sb.AppendLine(string.Format("(SELECT CASE WHEN Max(_Id) IS NULL THEN 0 ELSE Max(_Id) END AS _Id FROM [{0}]", objectType.Name));
                    sb.AppendLine(string.Format("UNION SELECT CASE WHEN Max(_Id) IS NULL THEN 0 ELSE Max(_Id) END AS _Id FROM [{0}_History]", objectType.Name));
                    sb.AppendLine(string.Format("UNION SELECT CASE WHEN Max(_Id) IS NULL THEN 0 ELSE Max(_Id) END AS _Id FROM [{0}_Archive]) AS Identifiers", objectType.Name));
                    cmd.CommandText = sb.ToString();
                    nextIdentifier = (long)cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    throw new ScriptException("Could not get the next available identifier from the table " + objectType.Name + ".", ex);
                }
            }

            return nextIdentifier;
        }

        /// <summary>
        /// Opens a connection to the database.
        /// </summary>
        /// <param name="connectionName">The name of the connection string.</param>
        public DbConnection GetDatabaseConnection(string connectionName)
        {
            //Validate the connection string
            string server = null, database = null, options = null, parseError = null;
            if (connectionName == null)
            {
                throw new ArgumentNullException("connectionName");
            }
            ConnectionStringElement cString = DataManager.Settings.ConnectionStrings[connectionName];
            if(cString == null)
                throw new ArgumentNullException("connectionName");
            if (!ScriptCommon.Parser.TryParseSqlConnectionString(cString.Connection, out server, out database, out options, out parseError))
            {
                throw new ConnectionStringException(parseError);
            }

            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(cString.Connection);
                connection.Open();
                return connection;
            }
            catch (Exception ex)
            {
                throw new ScriptException("Could not establish a connection with the " + connection.Database + " database.", ex);
            }
        }

        /// <summary>
        /// Gets a data adapter.
        /// </summary>
        public DbDataAdapter GetDataAdapter()
        {
            return new SqlDataAdapter();
        }

        #endregion

        #region Methods

        private void DropDatabase(SqlConnection connection, string databaseName)
        {
            try
            {
                string dropStatement = "IF (SELECT Count(1) FROM sys.databases WHERE [Name] = '" + databaseName + "' ) > 0" +
                    " DROP DATABASE " + databaseName;
                ScriptCommon.ExecuteCommand(connection, dropStatement);
            }
            catch
            {
                throw;
            }
        }

        private void CreateDatabase(SqlConnection connection, string databaseName)
        {
            try
            {
                DbCommand cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT Count(1) FROM sys.databases WHERE [Name] = '" + databaseName + "'";
                if ((int)cmd.ExecuteScalar() < 1)
                {
                    cmd.Dispose();
                    ScriptCommon.ExecuteCommand(connection, "CREATE DATABASE [" + databaseName + "]");

                    //Connect to the created database
                    connection.ChangeDatabase(databaseName);
                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Table_DataObjectFieldType);
                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Table_DataObjectType);
                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Table_DataObjectField);
                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Table_DataObjectIndex);
                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Table_DataObjectRelationship);

                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Table_List_DateTime);
                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Table_List_DateTimeValue);

                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Table_List_Double);
                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Table_List_DoubleValue);

                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Table_List_Single);
                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Table_List_SingleValue);

                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Table_List_Decimal);
                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Table_List_DecimalValue);

                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Table_List_Int64);
                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Table_List_Int64Value);

                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Table_List_Int32);
                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Table_List_Int32Value);

                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Table_List_Int16);
                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Table_List_Int16Value);

                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Table_List_Byte);
                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Table_List_ByteValue);

                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Table_List_Boolean);
                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Table_List_BooleanValue);

                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Table_List_Guid);
                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Table_List_GuidValue);

                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Table_List_String);
                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Table_List_StringValue);

                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Index_IDX_List_DateTime);
                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Index_IDX_List_DateTimeValue);

                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Index_IDX_List_Double);
                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Index_IDX_List_DoubleValue);

                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Index_IDX_List_Single);
                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Index_IDX_List_SingleValue);

                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Index_IDX_List_Decimal);
                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Index_IDX_List_DecimalValue);

                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Index_IDX_List_Int64);
                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Index_IDX_List_Int64Value);

                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Index_IDX_List_Int32);
                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Index_IDX_List_Int32Value);

                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Index_IDX_List_Int16);
                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Index_IDX_List_Int16Value);

                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Index_IDX_List_Byte);
                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Index_IDX_List_ByteValue);

                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Index_IDX_List_Boolean);
                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Index_IDX_List_BooleanValue);

                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Index_IDX_List_Guid);
                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Index_IDX_List_GuidValue);

                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Index_IDX_List_String);
                    ScriptCommon.ExecuteCommand(connection, Properties.Resources.Index_IDX_List_StringValue);
                }
                cmd.Dispose();
            }
            catch
            {
                throw;
            }
        }

        #endregion Methods
    }
}
