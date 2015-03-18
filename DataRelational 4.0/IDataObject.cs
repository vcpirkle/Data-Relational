using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational
{
    /// <summary>
    /// The base interface for all data relational objects.
    /// </summary>
    public interface IDataObject
    {
        /// <summary>
        /// Gets or sets the data object id.
        /// </summary>
        long _Id { get; set; }

        /// <summary>
        /// Gets or sets the data object type id.
        /// </summary>
        DataObjectType _ObjectType { get; set; }

        /// <summary>
        /// Gets or sets wether the data object is deleted.
        /// </summary>
        bool _Deleted { get; set; }

        /// <summary>
        /// Gets or sets the data object time stamp.
        /// </summary>
        DateTime _Timestamp { get; set; }

        /// <summary>
        /// Gets or sets whether the data object is dirty.
        /// </summary>
        bool _Dirty { get; set; }

        /// <summary>
        /// Gets the data object's instance identifier.  This identifier can be used until the object is destroyed, reloaded from the database, etc. 
        /// It will be different every time the object is constructed.
        /// </summary>
        Guid _InstanceIdentifier { get; }

        /// <summary>
        /// Sets the instance identifier.
        /// </summary>
        /// <param name="instanceIdentifier">A new guid.</param>
        void SetInstanceIdentifier(Guid instanceIdentifier);

        /// <summary>
        /// Used to notify implementors of initialization
        /// </summary>
        /// <param name="LoadedByBuilder">A flag specifying if the object is loaded by the data builder or not</param>
        void OnInitializing(bool LoadedByBuilder);

        /// <summary>
        /// Used to aquire a read / write lock for the data object.
        /// </summary>
        void Lock();

        /// <summary>
        /// Used to release the read / write lock for the data object.
        /// </summary>
        void Unlock();
    }
}
