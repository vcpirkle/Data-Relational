using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataRelational.Search;
using System.Runtime.Serialization;

namespace DataRelational
{
    /// <summary>
    /// Represents a strongly typed data relationship. Relationships can be of the type IDataObject, Array, List, or Dictionary.
    /// </summary>
    /// <typeparam name="T">The type of data relationship.</typeparam>
    [DataContract]
    public class DataRelationship<T> : IDataRelationsip<T>, IDataRelationship
    {
        #region Fields

        private bool loaded;
        private bool hasValue;
        private SearchContext searchContext;
        private Guid instanceIdentifier;
        private T value;
        #endregion Fields

        #region IDataRelationsip<T> Members

        /// <summary>
        /// Gets whether the data relationship has been loaded.
        /// </summary>
        public bool Loaded
        {
            get { return loaded; }
            internal set { loaded = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the data relationship has a value.
        /// </summary>
        public bool HasValue
        {
            get
            {
                if (this.Loaded == false && searchContext != null)
                {
                    //Load the next batch from the search context
                    IDataRelationship result = searchContext.LoadNextLevel(this);
                    return result.GetHasValue();
                }
                return this.hasValue;
            }
        }

        /// <summary>
        /// Gets the value of the current data relationship value.
        /// </summary>
        /// <exception cref="DataRelational.DataRelationalException"></exception>
        public T Value
        {
            get
            {
                if (this.Loaded == false && searchContext != null)
                {
                    //Load the next batch from the search context
                    IDataRelationship result = searchContext.LoadNextLevel(this);
                    this.hasValue = result.GetHasValue();
                    this.value = (T)result.GetValue();
                }
                if (!this.hasValue)
                {
                    throw new DataRelationalException("The data relationship has no value.", new InvalidOperationException("Value accessed when no value was present."));
                }
                return this.value;
            }
        }

        #endregion

        #region IDataRelationsip Members

        /// <summary>
        /// Gets the value of the current data relationship value.
        /// </summary>
        object IDataRelationship.GetValue()
        {
            return this.value;
        }

        /// <summary>
        /// Sets the value of the current data relationship value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        void IDataRelationship.SetValue(object value)
        {
            this.value = (T)value;
            this.hasValue = value != null;
            this.loaded = true;
        }

        /// <summary>
        /// Sets the context of the search.
        /// </summary>
        /// <param name="context">The search context.</param>
        void IDataRelationship.SetContext(SearchContext context)
        {
            this.searchContext = context;
        }

        /// <summary>
        /// Sets whether the data relationship has been loaded.
        /// </summary>
        /// <param name="loaded">Whether the data relationshp has been loaded.</param>
        void IDataRelationship.SetLoaded(bool loaded)
        {
            this.loaded = loaded;
        }

        /// <summary>
        /// Gets whether the data relationship has a value.
        /// </summary>
        bool IDataRelationship.GetHasValue()
        {
            return this.hasValue;
        }

        /// <summary>
        /// Gets instance identifer for this data relationshp.
        /// </summary>
        Guid IDataRelationship.GetInstanceIdentifier()
        {
            if (this.instanceIdentifier == Guid.Empty)
                this.instanceIdentifier = Guid.NewGuid();
            return this.instanceIdentifier;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of the DataRelational.DataRelationship class.
        /// </summary>
        /// <param name="val">The value of the data relationship</param>
        public DataRelationship(T val)
        {
            instanceIdentifier = Guid.NewGuid();
            searchContext = null;
            loaded = true;
            value = val;
            hasValue = value != null;
        }
        #endregion Constructor

        #region Methods

        /// <summary>
        /// Explicit operator overload that converts a generic data relationship object to is underlying type.
        /// </summary>
        /// <param name="value">The generic data relationship value.</param>
        /// <returns>The value of the generic data relationship.</returns>
        public static explicit operator T(DataRelationship<T> value)
        {
            return value.Value;
        }

        /// <summary>
        /// Implicit operator overload that converts an underlying type to its generic data relationship type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The generic data relationship with the value set to value.</returns>
        public static implicit operator DataRelationship<T>(T value)
        {
            return new DataRelationship<T>(value);
        }

        #endregion Methods
    }

    /// <summary>
    /// Represents a strongly typed data relationship. Relationships can be of the type IDataObject, Array, List, or Dictionary.
    /// </summary>
    public static class DataRelationship
    {
        /// <summary>
        /// Returns the underlying type argument of the specified data relationship type.
        /// </summary>
        /// <param name="dataRelationshipType">A System.Type object that describes a closed generic data relationship type.</param>
        /// <returns>The type argument of the dataRelationshipType parameter, if the dataRelationshipType parameter is a closed generic data relationship type; otherwise, null.</returns>
        public static Type GetUnderlyingType(Type dataRelationshipType)
        {
            if (dataRelationshipType == null)
            {
                throw new ArgumentNullException("dataRelationshipType");
            }

            Type type = null;
            if ((dataRelationshipType.IsGenericType && !dataRelationshipType.IsGenericTypeDefinition) && (dataRelationshipType.GetGenericTypeDefinition() == typeof(DataRelationship<>)))
            {
                type = dataRelationshipType.GetGenericArguments()[0];
            }
            return type;
        }
    }
}