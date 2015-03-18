using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational
{
    /// <summary>
    /// Determines the actions taken when one data relational object is updated from another data relational object.
    /// </summary>
    public enum UpdateBehavior : byte
    {
        /// <summary>
        /// Data fields and relationships are updated for the first level of the object graph meaning only the instance of the object will be updated.
        /// </summary>
        FirstLevel = 0,
        /// <summary>
        /// Data fields and relationships are updated for this object's entire object graph meaning that the data fields on this object as well as 
        /// data fields on all child objects will be updated recursivly.
        /// </summary>
        ObjectGraph = 1,
        /// <summary>
        /// Data feilds, relationships, and the dirty state are not automatically managed leaving it up to the user to define how the object is updated.
        /// </summary>
        UserDefined = 2
    }
}
