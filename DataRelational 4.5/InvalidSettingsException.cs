using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational
{
    /// <summary>
    /// The exception that is thrown when the database connection string is not valid.
    /// </summary>
    public class InvalidSettingsException : DataRelationalException
    {
        /// <summary>
        /// Initializes a new instance of the DataRelational.InvalidSettingsException class.
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        internal InvalidSettingsException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the DataRelational.InvalidSettingsException class.
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        internal InvalidSettingsException(string message, Exception innerException) : base(message, innerException) { }
    }
}
