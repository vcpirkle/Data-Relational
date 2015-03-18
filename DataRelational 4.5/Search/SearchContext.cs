using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Diagnostics;
using DataRelational.Cache;
using DataRelational.Script;

namespace DataRelational.Search
{
    /// <summary>
    /// Contains information about a search and its context.
    /// </summary>
    public class SearchContext
    {
        #region Properties

        /// <summary>
        /// Gets the initial complex search object.
        /// </summary>
        public ComplexSearch InitialSearch { get; private set; }

        /// <summary>
        /// Gets the first level data keys for this context.
        /// </summary>
        public List<DataKey> FirstLevel { get; private set; }

        /// <summary>
        /// Gets the list of relationships for this context.
        /// </summary>
        public Dictionary<int, List<Relationship>> Relationships { get; private set; }

        /// <summary>
        /// Gets the hash of objects that have been retieved in this context.
        /// </summary>
        public Dictionary<short, Dictionary<long, IDataObject>> RetrievedObjects { get; private set; }

        /// <summary>
        /// Gets or sets the current loaded depth level.
        /// </summary>
        internal int loadedDepth;

        internal List<Relationship> appliedRelationships;

        internal string connectionName;

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Creates a new instance of the DataRelational.Search.SearchContext class.
        /// </summary>
        /// <param name="initialSearch">The initial complex search object.</param>
        public SearchContext(ComplexSearch initialSearch)
        {
            this.loadedDepth = 0;
            this.appliedRelationships = new List<Relationship>();
            this.InitialSearch = initialSearch;
            this.FirstLevel = new List<DataKey>();
            this.RetrievedObjects = new Dictionary<short, Dictionary<long, IDataObject>>();
            this.Relationships = new Dictionary<int, List<Relationship>>();
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Loads the next 3 levels of depth for this search context
        /// <param name="requestingRelationship">The data relationship that is requesting the next level be loaded.</param>
        /// </summary>
        internal IDataRelationship LoadNextLevel(IDataRelationship requestingRelationship)
        {
            //Load the next batch of relationships into this search context
            DataManager.DataReader.GetRelationships(this, connectionName);

            //Ask the database for the next batch of objects
            StringBuilder builder = new StringBuilder();
            DataManager.DataReader.BeginRead(builder);
            DataManager.DataReader.AddGetObjectQuery(builder, this);
            DataSet result = DataManager.DataReader.EndRead(builder, connectionName);

            //Build the next batch of objects
            requestingRelationship = DataObjectBuilder.BuildObjects(this, requestingRelationship, result);

            this.ApplySearchContext();

            //Incriment the loaded depth counter
            this.loadedDepth += 3;

            if (this.FirstLevel.Count > 0)
            {
                List<Guid> instanceIdentifiers = new List<Guid>();
                DataObjectReflection objReflection = DataManager.ReflectionCache[this.FirstLevel[0].TypeId];
                foreach (DataKey dKey in this.FirstLevel)
                {
                    Dictionary<long, IDataObject> objectHash = null;
                    if (this.RetrievedObjects.TryGetValue(dKey.TypeId, out objectHash))
                    {
                        IDataObject obj = null;
                        if (objectHash.TryGetValue(dKey.Id, out obj))
                        {
                            this.ApplyLoadedDepthLevel(obj, ScriptCommon.GetChildRelationships(obj, objReflection), 2, instanceIdentifiers);
                        }
                    }
                }
            }
            return requestingRelationship;
        }

        internal void ApplyLoadedDepthLevel(IDataObject obj, List<IDataRelationship> children, int depth, List<Guid> instanceIdentifiers)
        {
            if (!instanceIdentifiers.Contains(obj._InstanceIdentifier))
            {
                instanceIdentifiers.Add(obj._InstanceIdentifier);
                foreach (IDataRelationship child in children)
                {
                    if (depth <= loadedDepth && child.Loaded == false)
                    {
                        child.SetLoaded(true);
                    }
                    if (child.GetHasValue() == true)
                    {
                        Type underlyingType = DataRelationship.GetUnderlyingType(child.GetType());
                        DataObjectReflection objReflection = null;
                        if (underlyingType.GetInterface("DataRelational.IDataObject", false) != null)
                        {
                            objReflection = DataManager.ReflectionCache[underlyingType];
                            ApplyLoadedDepthLevel((IDataObject)child.GetValue(), ScriptCommon.GetChildRelationships((IDataObject)child.GetValue(), objReflection), depth + 1, instanceIdentifiers);
                        }
                        else if (ScriptCommon.GetArrayType(underlyingType) != null && ScriptCommon.GetArrayType(underlyingType).GetInterface("DataRelational.IDataObject", false) != null)
                        {
                            objReflection = DataManager.ReflectionCache[ScriptCommon.GetArrayType(underlyingType)];
                            Array childArray = (Array)child.GetValue();
                            foreach (IDataObject c in childArray)
                            {
                                ApplyLoadedDepthLevel(c, ScriptCommon.GetChildRelationships(c, objReflection), depth + 1, instanceIdentifiers);
                            }
                        }
                        else if (underlyingType.IsGenericType && underlyingType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            objReflection = DataManager.ReflectionCache[underlyingType.GetGenericArguments()[0]];
                            IList childList = (IList)child.GetValue();
                            foreach (IDataObject c in childList)
                            {
                                ApplyLoadedDepthLevel(c, ScriptCommon.GetChildRelationships(c, objReflection), depth + 1, instanceIdentifiers);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Applies this search context to all of the retieved objects in the context.
        /// </summary>
        internal void ApplySearchContext()
        {
            foreach (short key in this.RetrievedObjects.Keys)
            {
                Dictionary<long, IDataObject> retrievedHash = this.RetrievedObjects[key];
                DataObjectReflection objReflection = DataManager.ReflectionCache[key];
                foreach (IDataObject retrievedObj in retrievedHash.Values)
                {
                    foreach (FieldInfo fld in objReflection.RelationshipFields)
                    {
                        if (fld.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Length == 0)
                        {
                            IDataRelationship dRelationship = (IDataRelationship)fld.GetValue(retrievedObj);
                            if (dRelationship == null)
                            {
                                Type underlyingType = DataRelationship.GetUnderlyingType(fld.FieldType);
                                dRelationship = ConstructGenericRelationship(underlyingType);
                                dRelationship.SetLoaded(false);
                            }
                            dRelationship.SetContext(this);
                            fld.SetValue(retrievedObj, dRelationship);
                        }
                    }
                    foreach (PropertyInfo prop in objReflection.RelationshipProperties)
                    {
                        IDataRelationship dRelationship = (IDataRelationship)prop.GetValue(retrievedObj, null);
                        if (dRelationship == null)
                        {
                            Type underlyingType = DataRelationship.GetUnderlyingType(prop.PropertyType);
                            dRelationship = ConstructGenericRelationship(underlyingType);
                            dRelationship.SetLoaded(false);
                        }
                        dRelationship.SetContext(this);
                        prop.SetValue(retrievedObj, dRelationship, null);
                    }
                }
            }
        }

        private IDataRelationship ConstructGenericRelationship(Type underlyingType)
        {
            if (underlyingType.GetInterface("DataRelational.IDataObject", false) != null)
            {
                Type dRelationType = typeof(DataRelationship<>);

                Type[] typeArgs = new Type[] { underlyingType };
                Type constructed = dRelationType.MakeGenericType(typeArgs);
                return (IDataRelationship)Activator.CreateInstance(constructed, new object[] { null });
            }
            else if (ScriptCommon.GetArrayType(underlyingType) != null && ScriptCommon.GetArrayType(underlyingType).GetInterface("DataRelational.IDataObject", false) != null)
            {
                Type dRelationType = typeof(DataRelationship<>);

                Array empty = Array.CreateInstance(ScriptCommon.GetArrayType(underlyingType), 0);

                Type[] typeArgs = new Type[] { empty.GetType() };
                Type constructed = dRelationType.MakeGenericType(typeArgs);
                return (IDataRelationship)Activator.CreateInstance(constructed, new object[] { null });
            }
            else if (underlyingType.IsGenericType && underlyingType.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type dRelationType = typeof(DataRelationship<>);

                Type lType = typeof(List<>);
                Type[] typeArgs = { underlyingType.GetGenericArguments()[0] };
                Type constructed = lType.MakeGenericType(typeArgs);

                typeArgs = new Type[] { constructed };
                constructed = dRelationType.MakeGenericType(typeArgs);
                return (IDataRelationship)Activator.CreateInstance(constructed, new object[] { null });
            }
            return null;
        }

        #endregion Methods
    }
}
