using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational
{
    /// <summary>
    /// The base interface for all data relational historical objects.
    /// </summary>
    public interface IHistoricalDataObject: IDataObject
    {
        /// <summary>
        /// Gets or sets the historical data object revision id.
        /// </summary>
        int _RevisionId { get; set; }
    }
}
