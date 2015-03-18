using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Threading;

namespace DataRelational
{
    /// <summary>
    /// The base class for all data relational objects.
    /// </summary>
    [DataContract]
    public class DataObject : IDataObject, INotifyPropertyChanged, IDeserializationCallback 
    {
        #region Fields

        private bool m_Deleted = false;
        private long m_Id;
        private DataObjectType m_ObjectType;
        private DateTime m_Timestamp;
        private bool m_Dirty = true;
        private ReaderWriterLock m_lock;

        #endregion Fields

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the DataRelational.DataObject class.
        /// </summary>
        public DataObject()
        {
            this._InstanceIdentifier = Guid.NewGuid();
            this.m_lock = new ReaderWriterLock();
            OnInitializing(false);
        }

        /// <summary>
        /// Initializes a new instance of the DataRelational.DataObject class.
        /// </summary>
        /// <param name="id">the data object id</param>
        /// <param name="objectType">the data object type</param>
        /// <param name="deleted">wether the data object is deleted</param>
        /// <param name="timestamp">the data object time stamp</param>
        public DataObject(long id, DataObjectType objectType, bool deleted, DateTime timestamp) : this()
        {
            _Id = id;
            _ObjectType = objectType;
            _Deleted = deleted;
            _Timestamp = timestamp;
            OnInitializing(false);
        }

        #endregion Constructor

        #region IDataObject Members

        /// <summary>
        /// Gets or sets the data object id.
        /// </summary>
        [DataMember]
        public long _Id
        {
            get { return m_Id; }
            set
            {
                if (m_Id != value)
                {
                    m_Id = value;
                    RaisePropertyChanged("_Id", false);
                }
            }
        }

        /// <summary>
        /// Gets or sets the data object type id.
        /// </summary>
        [DataMember(IsRequired = false)]
        public DataObjectType _ObjectType
        {
            get { return m_ObjectType; }
            set
            {
                if (m_ObjectType != value)
                {
                    m_ObjectType = value;
                    RaisePropertyChanged("_ObjectType", false);
                }
            }
        }

        /// <summary>
        /// Gets or sets wether the data object is deleted.
        /// </summary>
        [DataMember]
        public bool _Deleted
        {
            get { return m_Deleted; }
            set 
            {
                if (m_Deleted != value)
                {
                    m_Deleted = value;
                    RaisePropertyChanged("_Deleted");
                }
            }
        }

        /// <summary>
        /// Gets or sets the data object time stamp.
        /// </summary>
        [DataMember]
        public DateTime _Timestamp 
        {
            get { return m_Timestamp; }
            set
            {
                if (m_Timestamp != value)
                {
                    m_Timestamp = value;
                    RaisePropertyChanged("_Timestamp", false);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the data object is dirty.
        /// </summary>
        public bool _Dirty
        {
            get { return m_Dirty; }
            set
            {
                ((IDataObject)this).Lock();
                bool valChanging = m_Dirty != value;
                if (valChanging)
                {
                    m_Dirty = value;
                }
                ((IDataObject)this).Unlock();

                if (valChanging)
                {
                    RaisePropertyChanged("_Dirty", false);
                }
            }
        }

        /// <summary>
        /// Gets the data object's instance identifier.  This identifier can be used until the object is destroyed, reloaded from the database, etc. 
        /// It will be different every time the object is constructed.
        /// </summary>
        public Guid _InstanceIdentifier { get; private set; }

        /// <summary>
        /// Sets the instance identifier.
        /// </summary>
        /// <param name="instanceIdentifier">A new guid.</param>
        void IDataObject.SetInstanceIdentifier(Guid instanceIdentifier)
        {
            this._InstanceIdentifier = instanceIdentifier;
        }

        /// <summary>
        /// Used to aquire a read / write lock for the data object.
        /// </summary>
        void IDataObject.Lock()
        {
            if (this.m_lock == null) { this.m_lock = new ReaderWriterLock(); }
            this.m_lock.AcquireWriterLock(-1);
        }

        /// <summary>
        /// Used to release the read / write lock for the data object.
        /// </summary>
        void IDataObject.Unlock()
        {
            if (this.m_lock == null) { this.m_lock = new ReaderWriterLock(); }
            this.m_lock.ReleaseWriterLock();
        }

        /// <summary>
        /// Called when constructing or after being loaded, typically used to setup initial values on inheriting classes
        /// </summary>
        /// <param name="LoadedByBuilder">A flag specifying if the object is loaded by the data builder or not</param>
        public virtual void OnInitializing(bool LoadedByBuilder)
        {
        }
        #endregion

        #region INotifyPropertyChanged and support
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event without setting the dirty flag
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            RaisePropertyChanged(propertyName, true);
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        /// <param name="setDirty">A flag specifying if the dirty flag can be should be set or not</param>
        protected void RaisePropertyChanged(string propertyName, bool setDirty)
        {
            if (setDirty){ _Dirty = true; }

            if (PropertyChanged != null)
            {
                PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName);
                SafeInvoke.Invoke(PropertyChanged, new object[] { this, e });
            }
        }
        #endregion INotifyPropertyChanged and support

        #region IDeserializationCallback Members

        void IDeserializationCallback.OnDeserialization(object sender)
        {
            this._InstanceIdentifier = Guid.NewGuid();
            this.m_lock = new ReaderWriterLock();
        }

        #endregion
    }
}
