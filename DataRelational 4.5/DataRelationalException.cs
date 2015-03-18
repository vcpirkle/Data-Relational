using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational
{
    /// <summary>
    /// The base exception for data relational exceptions.
    /// </summary>
    public class DataRelationalException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the DataRelational.DataRelationalException class.
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        internal DataRelationalException(string message) : base(message)
        {
            Logging.Exception(this);
        }

        /// <summary>
        /// Initializes a new instance of the DataRelational.DataRelationalException class.
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        internal DataRelationalException(string message, Exception innerException) : base(message, innerException)
        {
            Logging.Exception(this);
        }
    }
}
