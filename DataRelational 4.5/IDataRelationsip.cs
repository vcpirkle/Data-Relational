using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataRelational.Search;

namespace DataRelational
{
    interface IDataRelationsip<T>
    {
        /// <summary>
        /// Gets whether the data relationship has been loaded.
        /// </summary>
        bool Loaded { get; }

        /// <summary>
        /// Gets a value indicating whether the data relationship has a value.
        /// </summary>
        bool HasValue { get; }

        /// <summary>
        /// Gets the value of the current data relationship value.
        /// </summary>
        T Value { get; }
    }

    interface IDataRelationship
    {
        /// <summary>
        /// Gets whether the data relationship has been loaded.
        /// </summary>
        bool Loaded { get; }

        /// <summary>
        /// Gets a value indicating whether the data relationship has a value.
        /// </summary>
        bool HasValue { get; }

        /// <summary>
        /// Gets the value of the current data relationship value.
        /// </summary>
        object GetValue();

        /// <summary>
        /// Sets the value of the current data relationship value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        void SetValue(object value);

        /// <summary>
        /// Sets the context of the search.
        /// </summary>
        /// <param name="context">The search context.</param>
        void SetContext(SearchContext context);

        /// <summary>
        /// Sets whether the data relationship has been loaded.
        /// </summary>
        /// <param name="loaded">Whether the data relationshp has been loaded.</param>
        void SetLoaded(bool loaded);

        /// <summary>
        /// Gets whether the data relationship has a value.
        /// </summary>
        bool GetHasValue();

        /// <summary>
        /// Gets instance identifer for this data relationshp.
        /// </summary>
        Guid GetInstanceIdentifier();
    }
}
