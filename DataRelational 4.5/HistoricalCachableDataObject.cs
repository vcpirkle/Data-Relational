using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataRelational
{
    /// <summary>
    /// The base class for all historical savable data relational objects.
    /// </summary>
    [DataContract]
    public class HistoricalCachableDataObject : HistoricalDataObject, ICacheableDataObject
    {
        #region ICacheableDataObject Members

        /// <summary>
        /// Updates this savable data object from another savable data object.  
        /// The intent of this method is that the object is cached on the server and the from object 
        /// has been deserialized or built in some other maner and is only used to update the cached object prior to saving it.
        /// All fields marked with the <see cref="DataRelational.Attributes.DataFieldAttribute">DataField</see> attribute will be updated
        /// as well as child relationships according to the <see cref="DataRelational.UpdateBehavior">UpdateBehavior</see> 
        /// defined in the <see cref="DataRelational.Attributes.SavableObjectAttribute">SavableObject</see> attribute.
        /// </summary>
        /// <param name="fromObject">The object to update this object from</param>
        public void UpdateFromObject(ICacheableDataObject fromObject)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
