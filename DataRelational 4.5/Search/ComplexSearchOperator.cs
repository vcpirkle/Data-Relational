using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational.Search
{
    /// <summary>
    /// Describes a the operation of a complex search element.
    /// </summary>
    public enum ComplexSearchOperator : byte
    {
        /// <summary>
        /// The field is equal to the value.
        /// </summary>
        Equal = 0,
        /// <summary>
        /// The field is not equal to the value.
        /// </summary>
        NotEqual = 1,
        /// <summary>
        /// The field is greater than the value.
        /// </summary>
        GreaterThan = 2,
        /// <summary>
        /// The field is less than the value.
        /// </summary>
        LessThan = 3,
        /// <summary>
        /// The field is greater than or equal to the value.
        /// </summary>
        GreaterThanOrEqual = 4,
        /// <summary>
        /// The field is less than or equal to the value.
        /// </summary>
        LessThanOrEqual = 5,
        /// <summary>
        /// The field is between the values.
        /// </summary>
        Between = 6,
        /// <summary>
        /// The field is like the value.
        /// </summary>
        Like = 7,
        /// <summary>
        /// The field is in the values.
        /// </summary>
        In = 8
    }
}
