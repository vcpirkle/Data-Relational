using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataRelational
{
    /// <summary>
    /// The base class for all historical data relational objects.
    /// </summary>
    [DataContract]
    public class HistoricalDataObject : DataObject, IHistoricalDataObject
    {
        #region Fields
        private int m_RevisionId;
        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the DataRelational.HistoricalDataObject class.
        /// </summary>
        public HistoricalDataObject() : base() { }

        /// <summary>
        /// Initializes a new instance of the DataRelational.HistoricalDataObject class.
        /// </summary>
        /// <param name="id">the historical data object id</param>
        /// <param name="revisionId">the historical data object revision id</param>
        /// <param name="objectType">the historical data object type</param>
        /// <param name="deleted">wether the historical data object is deleted</param>
        /// <param name="timestamp">the historical data object time stamp</param>
        public HistoricalDataObject(long id, int revisionId, DataObjectType objectType, bool deleted, DateTime timestamp)
            : base(id, objectType, deleted, timestamp)
        {
            _RevisionId = revisionId;
        }

        #endregion Constructor

        #region IHistoricalDataObject Members

        /// <summary>
        /// Gets or sets the historical data object revision id.
        /// </summary>
        [DataMember]
        public int _RevisionId
        {
            get { return m_RevisionId; }
            set
            {
                if (m_RevisionId != value)
                {
                    m_RevisionId = value;
                    RaisePropertyChanged("_RevisionId", false);
                }
            }
        }
        #endregion
    }
}
