using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational.Script
{
    /// <summary>
    /// Used to parse and validate connection strings
    /// </summary>
    internal class ConnectionStringParser
    {
        /// <summary>
        /// Attempts to parse a SQL connection string
        /// </summary>
        /// <param name="connectionString">The SQL connection string to parse</param>
        /// <param name="server">The server name</param>
        /// <param name="database">The database name</param>
        /// <param name="options">The remaining connection string options excluding the server name and database name</param>
        /// <param name="parseError">The parse error message</param>
        /// <returns>True if the connection string appears valid, otherwise false</returns>
        internal bool TryParseSqlConnectionString(string connectionString, out string server, out string database, out string options, out string parseError)
        {
            int sIndex = connectionString.ToLower().IndexOf("data source=");
            int eIndex = -1;
            string removeStr = null;

            if (sIndex > -1)
            {
                sIndex += 12;
                eIndex = connectionString.IndexOf(";", sIndex);
                server = connectionString.Substring(sIndex, eIndex - sIndex);
                sIndex -= 12;
                removeStr = connectionString.Substring(sIndex, eIndex - sIndex + 1);
                connectionString = connectionString.Replace(removeStr, "");
            }
            else
            {
                sIndex = connectionString.ToLower().IndexOf("server=");
                if (sIndex > -1)
                {
                    sIndex += 7;
                    eIndex = connectionString.IndexOf(";", sIndex);
                    server = connectionString.Substring(sIndex, eIndex - sIndex);
                    sIndex -= 7;
                    removeStr = connectionString.Substring(sIndex, eIndex - sIndex + 1);
                    connectionString = connectionString.Replace(removeStr, "");
                }
                else
                {
                    server = null;
                    database = null;
                    options = null;
                    parseError = "The connection string does not contain a Data Source parameter or a Server parameter.";
                    return false;
                }
            }

            sIndex = connectionString.ToLower().IndexOf("initial catalog=");
            if (sIndex > -1)
            {
                sIndex += 16;
                eIndex = connectionString.IndexOf(";", sIndex);
                database = connectionString.Substring(sIndex, eIndex - sIndex);
                sIndex -= 16;
                removeStr = connectionString.Substring(sIndex, eIndex - sIndex + 1);
                connectionString = connectionString.Replace(removeStr, "");
            }
            else
            {
                sIndex = connectionString.ToLower().IndexOf("database=");
                if (sIndex > -1)
                {
                    sIndex += 9;
                    eIndex = connectionString.IndexOf(";", sIndex);
                    database = connectionString.Substring(sIndex, eIndex - sIndex);
                    sIndex -= 9;
                    removeStr = connectionString.Substring(sIndex, eIndex - sIndex + 1);
                    connectionString = connectionString.Replace(removeStr, "");
                }
                else
                {
                    server = null;
                    database = null;
                    options = null;
                    parseError = "The connection string does not contain an Initial Catalog parameter or a Database parameter.";
                    return false;
                }
            }

            options = connectionString;
            parseError = "";
            return true;
        }
    }
}
