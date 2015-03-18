using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational.Schema
{
    /// <summary>
    /// The exception that is thrown when a data relational object has an invalid schema
    /// </summary>
    public class InvalidObjectSchemaException : DataRelationalException
    {
        /// <summary>
        /// Initializes a new instance of the DataRelational.InvalidObjectSchemaException class.
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        internal InvalidObjectSchemaException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the DataRelational.InvalidObjectSchemaException class.
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        internal InvalidObjectSchemaException(string message, Exception innerException) : base(message, innerException) { }
    }
}
