using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational
{
    /// <summary>
    /// Indicates that the data relational object can be cached.
    /// </summary>
    public interface ICacheableDataObject
    {
        /// <summary>
        /// Updates this cacheable data object from another cacheable data object.  
        /// The intent of this method is that the object is cached on the server and the from object 
        /// has been deserialized or built in some other manor and is only used to update the cached object prior to saving it.
        /// All fields marked with the <see cref="DataRelational.Attributes.DataFieldAttribute">DataField</see> attribute will be updated
        /// as well as child relationships according to the <see cref="DataRelational.UpdateBehavior">UpdateBehavior</see> 
        /// defined in the <see cref="DataRelational.Attributes.SavableObjectAttribute">SavableObject</see> attribute.
        /// </summary>
        /// <param name="fromObject">The object to update this data object from.</param>
        void UpdateFromObject(ICacheableDataObject fromObject);
    }
}
