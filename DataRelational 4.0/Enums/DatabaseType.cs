using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational
{
    /// <summary>
    /// Determines the type of database to establish a connection to.
    /// </summary>
    public enum DatabaseType : byte
    {
        /// <summary>
        /// A connection will be made to a SQL Server database.
        /// </summary>
        SqlServer = 0,
        /// <summary>
        /// A connection will be made to a MySql database.
        /// </summary>
        MySql = 1,
        /// <summary>
        /// A connection will be made to an Oracle database.
        /// </summary>
        Oracle = 2,
        /// <summary>
        /// A connection will be made to a Sqlite database.
        /// </summary>
        Sqlite = 3,
        /// <summary>
        /// A connection will be made to a DBase database.
        /// </summary>
        DBase = 4,
        /// <summary>
        /// A connection will be made to an Access database.
        /// </summary>
        Access = 5,
        /// <summary>
        /// A connection will be made to a DB2 database.
        /// </summary>
        DB2 = 6,
        /// <summary>
        /// A connection will be made to a Firebird database.
        /// </summary>
        Firebird = 7,
        /// <summary>
        /// A connection will be made to a FoxPro database.
        /// </summary>
        FoxPro = 8,
        /// <summary>
        /// A connection will be made to a Sybase database.
        /// </summary>
        Sybase = 9
    }
}
