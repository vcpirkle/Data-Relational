using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational.Script
{
    /// <summary>
    /// The exception that is thrown when an error occurs scripting the database
    /// </summary>
    public class ScriptException : DataRelationalException
    {
        /// <summary>
        /// Initializes a new instance of the DataRelational.Script.ScriptException class.
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        internal ScriptException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the DataRelational.Script.ScriptException class.
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference</param>
        internal ScriptException(string message, Exception innerException) : base(message, innerException) { }
    }
}
