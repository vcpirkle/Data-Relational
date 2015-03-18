using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational.Script
{
    /// <summary>
    /// The exception that is thrown when the database connection string is not valid
    /// </summary>
    public class ConnectionStringException : DataRelationalException
    {
        /// <summary>
        /// Initializes a new instance of the DataRelational.Script.ConnectionStringException class.
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        internal ConnectionStringException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the DataRelational.Script.ConnectionStringException class.
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        internal ConnectionStringException(string message, Exception innerException) : base(message, innerException) { }
    }
}
