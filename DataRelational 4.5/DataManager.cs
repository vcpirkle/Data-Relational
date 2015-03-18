using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataRelational.Script;
using DataRelational.Cache;
using DataRelational.Schema;
using DataRelational.Attributes;
using System.Web;
using System.Data;
using System.Configuration;
using DataRelational.DataWriters;
using DataRelational.Search;
using DataRelational.DataReaders;
using System.IO;

namespace DataRelational
{
    /// <summary>
    /// Manages the creation, modification, deletion, and retrieval of all data relational objects.
    /// </summary>
    public class DataManager
    {
        #region Constants

        /// <summary>
        /// Gets the maximum date time value.
        /// </summary>
        public static DateTime MaxDate
        {
            get { return new DateTime(2999, 12, 31); }
        }

        /// <summary>
        /// Gets the minimum date time value.
        /// </summary>
        public static DateTime MinDate
        {
            get { return new DateTime(1753, 1, 1); }
        }

        #endregion Constants

        #region Fields

        /// <summary>
        /// DataManager instance used in a windows forms environment
        /// </summary>
        private static DataManager _instance;
        private DataObjectReflectionCache _ReflectionCache;
        private DataManagerSettings _Settings;
        private ScriptManager _ScriptManager;
        private IDataWriter _dataWriter;
        private DataManagerSearch _search;
        private IDataCache _cache;
        private DataRelational.DataReaders.IDataReader _dataReader;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the data manager configuration settings.
        /// </summary>
        public static DataManagerSettings Settings
        {
            get { return DataManager.Instance._Settings; }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataRelational.DataWriters.IDataWriter"/> to use when writing data relational objects to the database.
        /// </summary>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static IDataWriter DataWriter
        {
            get { return DataManager.Instance._dataWriter; }
            set 
            {
                if (value == null)
                    throw new ArgumentNullException();

                DataManager.Instance._dataWriter = value; 
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataRelational.DataReaders.IDataReader"/> to use when reading data from the database.
        /// </summary>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static DataRelational.DataReaders.IDataReader DataReader
        {
            get { return DataManager.Instance._dataReader; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                DataManager.Instance._dataReader = value;
            }
        }

        /// <summary>
        /// Gets the data manager reflection cache.
        /// </summary>
        internal static DataObjectReflectionCache ReflectionCache
        {
            get
            {
                return DataManager.Instance._ReflectionCache;
            }
        }

        /// <summary>
        /// Gets the script manager.
        /// </summary>
        internal static ScriptManager ScriptManager
        {
            get
            {
                return DataManager.Instance._ScriptManager;
            }
        }

        /// <summary>
        /// Gets the data cahce.
        /// </summary>
        internal static IDataCache Cache
        {
            get
            {
                return DataManager.Instance._cache;
            }
        }

        /// <summary>
        /// Used to perform a search operation.
        /// </summary>
        public static DataManagerSearch Search
        {
            get { return DataManager.Instance._search; }
        }

        #endregion Properties

        #region Singleton
        private static DataManager Instance
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    HttpApplicationState App = HttpContext.Current.Application;
                    if (App["DataRelational.DataManager._instance"] == null)
                    {
                        DataManager dm = new DataManager();
                        App["DataRelational.DataManager._instance"] = dm;
                        dm._ScriptManager.Initialize();
                    }
                    return (DataManager)App["DataRelational.DataManager._instance"];
                }
                else
                {
                    if (_instance == null)
                    {
                        _instance = new DataManager();
                        _instance._ScriptManager.Initialize();
                    }
                    return _instance;
                }
            }
        }
        #endregion Singleton

        #region Constructor

        private DataManager()
        {
            this._Settings = ConfigurationManager.GetSection("DataManagerSettings") as DataManagerSettings;
            this._dataWriter = new TSqlDataWriter();
            this._dataReader = new TSqlDataReader();
            this._search = new DataManagerSearch();
            this._ScriptManager = new ScriptManager();
            this._ReflectionCache = new DataObjectReflectionCache();
            this._cache = new DataCache();
        }

        #endregion Constructor

        #region Save

        /// <summary>
        /// Saves the object according to the <see cref="DataRelational.SaveBehavior">SaveBehavior</see> defined in the <see cref="DataRelational.Attributes.SavableObjectAttribute">SavableObject</see> attribute.
        /// </summary>
        /// <param name="obj">The data relational object to save.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="DataRelational.SaveException"></exception>
        /// <exception cref="DataRelational.InvalidSettingsException"></exception>
        /// <exception cref="DataRelational.Schema.InvalidObjectSchemaException"></exception>
        public static void Save(IDataObject obj)
        {
            Save(obj, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Saves the object according to the <see cref="DataRelational.SaveBehavior">SaveBehavior</see> defined in the <see cref="DataRelational.Attributes.SavableObjectAttribute">SavableObject</see> attribute.
        /// </summary>
        /// <param name="obj">The data relational object to save.</param>
        /// <param name="connectionName">The name of the connection to write to.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="DataRelational.SaveException"></exception>
        /// <exception cref="DataRelational.InvalidSettingsException"></exception>
        /// <exception cref="DataRelational.Schema.InvalidObjectSchemaException"></exception>
        public static void Save(IDataObject obj, string connectionName)
        {
            StringBuilder builder = new StringBuilder();
            DataManager.DataWriter.BeginWrite(builder);
            DataManager.Instance._Save(obj, builder, new List<Guid>(), new List<Guid>(), new List<Relationship>());
            DataTable writeTimes = DataManager.DataWriter.EndWrite(builder, connectionName);
            ScriptCommon.ApplyWriteTimes(obj, writeTimes);
        }

        /// <summary>
        /// Saves the object collection in a batch according to the <see cref="DataRelational.SaveBehavior">SaveBehavior</see> defined in the <see cref="DataRelational.Attributes.SavableObjectAttribute">SavableObject</see> attribute.
        /// </summary>
        /// <param name="objList">The data relational object collection to save.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="DataRelational.SaveException"></exception>
        /// <exception cref="DataRelational.InvalidSettingsException"></exception>
        /// <exception cref="DataRelational.Schema.InvalidObjectSchemaException"></exception>
        public static void Save(IEnumerable<IDataObject> objList)
        {
            Save(objList, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Saves the object collection in a batch according to the <see cref="DataRelational.SaveBehavior">SaveBehavior</see> defined in the <see cref="DataRelational.Attributes.SavableObjectAttribute">SavableObject</see> attribute.
        /// </summary>
        /// <param name="objList">The data relational object collection to save.</param>
        /// <param name="connectionName">The name of the connection to write to.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="DataRelational.SaveException"></exception>
        /// <exception cref="DataRelational.InvalidSettingsException"></exception>
        /// <exception cref="DataRelational.Schema.InvalidObjectSchemaException"></exception>
        public static void Save(IEnumerable<IDataObject> objList, string connectionName)
        {
            StringBuilder builder = new StringBuilder();
            DataManager.DataWriter.BeginWrite(builder);
            foreach (IDataObject obj in objList)
            {
                DataManager.Instance._Save(obj, builder, new List<Guid>(), new List<Guid>(), new List<Relationship>());
            }
            DataTable writeTimes = DataManager.DataWriter.EndWrite(builder, connectionName);
            foreach (IDataObject obj in objList)
            {
                ScriptCommon.ApplyWriteTimes(obj, writeTimes);
            }
        }
        
        private void _Save(IDataObject obj, StringBuilder builder, List<Guid> savedIds, List<Guid> savedRelationshipIds, List<Relationship> savedRelationships)
        {
            SavableObjectAttribute saveAttr = _PreSaveValidate(obj);
            bool workToDo = !savedIds.Contains(obj._InstanceIdentifier) || !savedRelationshipIds.Contains(obj._InstanceIdentifier);

            //Make sure we have some work to do
            if (workToDo)
            {
                DataObjectReflection objectReflection = null;

                if (!ReflectionCache.Contains(obj.GetType()))
                {
                    //If the reflection cache has never seen the object type, we need to make sure we have a valid place to store the object.
                    //The script manager will adjust the database footprint for the object if necessary.
                    DataObjectSchema dataObjectSchema = DataManager.ScriptManager.ScriptObjectSchema(obj.GetType());
                    objectReflection = new DataObjectReflection(dataObjectSchema);
                    ReflectionCache.Add(objectReflection);
                }
                else
                {
                    objectReflection = ReflectionCache[obj.GetType()];
                }

                //Make sure the object has a the object type set
                obj._ObjectType = objectReflection.Schema;

                try
                {
                    //Aquire a lock on the object before saving it (thread safety)
                    obj.Lock();

                    //Make sure only one instance of an object gets saved
                    if (!savedIds.Contains(obj._InstanceIdentifier))
                    {
                        savedIds.Add(obj._InstanceIdentifier);

                        //Write the IDataObject to the string builder
                        _dataWriter.Write(builder, obj, saveAttr);

                        Logging.Debug(string.Format("Added the object {0} - Id:{1} to the batch to be {2}.", obj.GetType().Namespace + "." + obj.GetType().Name, obj._Id, (obj._Deleted ? "deleted" : "saved")));

                        //Save the child data objects
                        foreach (FieldChildObject child in ScriptCommon.GetChildren(obj, objectReflection))
                        {
                            _Save(child.ChildObject, builder, savedIds, savedRelationshipIds, savedRelationships);
                        }
                    }

                    //Make sure only one instance of an object gets saved relationships
                    if (!savedRelationshipIds.Contains(obj._InstanceIdentifier))
                    {
                        savedRelationshipIds.Add(obj._InstanceIdentifier);
                        //Write the parent - child relationships to the string builder
                        _dataWriter.WriteRelationships(builder, obj, saveAttr, savedRelationships);
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    //Release the lock on the object after the save is complete (thread safety)
                    obj.Unlock();
                }
            }
        }

        /// <summary>
        /// Deletes the object according to the <see cref="DataRelational.DeleteBehavior">DeleteBehavior</see> defined in the <see cref="DataRelational.Attributes.SavableObjectAttribute">SavableObject</see> attribute.
        /// </summary>
        /// <param name="obj">The data relational object collection to save.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="DataRelational.SaveException"></exception>
        /// <exception cref="DataRelational.InvalidSettingsException"></exception>
        /// <exception cref="DataRelational.Schema.InvalidObjectSchemaException"></exception>
        public static void Delete(IDataObject obj)
        {
            DataManager.Instance._Delete(obj, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Deletes the object according to the <see cref="DataRelational.DeleteBehavior">DeleteBehavior</see> defined in the <see cref="DataRelational.Attributes.SavableObjectAttribute">SavableObject</see> attribute.
        /// </summary>
        /// <param name="obj">The data relational object collection to save.</param>
        /// <param name="connectionName">The name of the connection to write to.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="DataRelational.SaveException"></exception>
        /// <exception cref="DataRelational.InvalidSettingsException"></exception>
        /// <exception cref="DataRelational.Schema.InvalidObjectSchemaException"></exception>
        public static void Delete(IDataObject obj, string connectionName)
        {
            DataManager.Instance._Delete(obj, connectionName);
        }

        /// <summary>
        /// Deletes the collection in a batch according to the <see cref="DataRelational.DeleteBehavior">DeleteBehavior</see> defined in the <see cref="DataRelational.Attributes.SavableObjectAttribute">SavableObject</see> attribute.
        /// </summary>
        /// <param name="objList">The data relational object collection to delete.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="DataRelational.SaveException"></exception>
        /// <exception cref="DataRelational.InvalidSettingsException"></exception>
        /// <exception cref="DataRelational.Schema.InvalidObjectSchemaException"></exception>
        public static void Delete(IEnumerable<IDataObject> objList)
        {
            foreach (IDataObject obj in objList)
            {
                PreSaveValidate(obj);
                obj._Dirty = true;
                obj._Deleted = true;
            }
            Save(objList);
        }

        /// <summary>
        /// Deletes the collection in a batch according to the <see cref="DataRelational.DeleteBehavior">DeleteBehavior</see> defined in the <see cref="DataRelational.Attributes.SavableObjectAttribute">SavableObject</see> attribute.
        /// </summary>
        /// <param name="objList">The data relational object collection to delete.</param>
        /// <param name="connectionName">The name of the connection to write to.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="DataRelational.SaveException"></exception>
        /// <exception cref="DataRelational.InvalidSettingsException"></exception>
        /// <exception cref="DataRelational.Schema.InvalidObjectSchemaException"></exception>
        public static void Delete(IEnumerable<IDataObject> objList, string connectionName)
        {
            foreach (IDataObject obj in objList)
            {
                PreSaveValidate(obj);
                obj._Dirty = true;
                obj._Deleted = true;
            }
            Save(objList, connectionName);
        }
        
        private void _Delete(IDataObject obj, string connectionName)
        {
            SavableObjectAttribute saveAttr = _PreSaveValidate(obj);
            obj._Dirty = true;
            obj._Deleted = true;
            Save(obj, connectionName);
        }

        #endregion Save

        #region Get

        #region Get Object
        /// <summary>
        /// Gets the latest version of an object.
        /// </summary>
        /// <typeparam name="T">The type of object to get.</typeparam>
        /// <param name="id">The objects identifier.</param>
        public static T GetObject<T>(long id) where T : IDataObject
        {
            return DataManager.Instance._GetObject<T>(id, null, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Gets the latest version of an object.
        /// </summary>
        /// <typeparam name="T">The type of object to get.</typeparam>
        /// <param name="id">The objects identifier.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        public static T GetObject<T>(long id, string connectionName) where T : IDataObject
        {
            return DataManager.Instance._GetObject<T>(id, null, false, false, connectionName);
        }

        /// <summary>
        /// Gets the most recent version of an object prior to a date time.
        /// </summary>
        /// <typeparam name="T">The type of object to get.</typeparam>
        /// <param name="id">The objects identifier.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        public static T GetObject<T>(long id, DateTime timestamp) where T : IDataObject
        {
            return DataManager.Instance._GetObject<T>(id, timestamp, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Gets the most recent version of an object prior to a date time.
        /// </summary>
        /// <typeparam name="T">The type of object to get.</typeparam>
        /// <param name="id">The objects identifier.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        public static T GetObject<T>(long id, DateTime timestamp, string connectionName) where T : IDataObject
        {
            return DataManager.Instance._GetObject<T>(id, timestamp, false, false, connectionName);
        }

        /// <summary>
        /// Gets the most recent version of an object prior to a date time.
        /// </summary>
        /// <typeparam name="T">The type of object to get.</typeparam>
        /// <param name="id">The objects identifier.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects.</param>
        public static T GetObject<T>(long id, DateTime? timestamp, bool includeDeleted) where T : IDataObject
        {
            return DataManager.Instance._GetObject<T>(id, timestamp, includeDeleted, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Gets the most recent version of an object prior to a date time.
        /// </summary>
        /// <typeparam name="T">The type of object to get.</typeparam>
        /// <param name="id">The objects identifier.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        public static T GetObject<T>(long id, DateTime? timestamp, bool includeDeleted, string connectionName) where T : IDataObject
        {
            return DataManager.Instance._GetObject<T>(id, timestamp, includeDeleted, false, connectionName);
        }

        /// <summary>
        /// Gets the most recent version of an object prior to a date time.
        /// </summary>
        /// <typeparam name="T">The type of object to get.</typeparam>
        /// <param name="id">The objects identifier.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects.</param>
        /// <param name="includeArchive">Whether to include archived objects.</param>
        public static T GetObject<T>(long id, DateTime? timestamp, bool includeDeleted, bool includeArchive) where T : IDataObject
        {
            return DataManager.Instance._GetObject<T>(id, timestamp, includeDeleted, includeArchive, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Gets the most recent version of an object prior to a date time.
        /// </summary>
        /// <typeparam name="T">The type of object to get.</typeparam>
        /// <param name="id">The objects identifier.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects.</param>
        /// <param name="includeArchive">Whether to include archived objects.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        public static T GetObject<T>(long id, DateTime? timestamp, bool includeDeleted, bool includeArchive, string connectionName) where T : IDataObject
        {
            return DataManager.Instance._GetObject<T>(id, timestamp, includeDeleted, includeArchive, connectionName);
        }

        private T _GetObject<T>(long id, DateTime? timestamp, bool includeDeleted, bool includeArchive, string connectionName) where T : IDataObject
        {
            DataObjectReflection objectReflection = null;

            if (!ReflectionCache.Contains(typeof(T)))
            {
                //If the reflection cache has never seen the object type, we need to make sure we have a valid place to store the object.
                //The script manager will adjust the database footprint for the object if necessary.
                DataObjectSchema dataObjectSchema = DataManager.ScriptManager.ScriptObjectSchema(typeof(T));
                objectReflection = new DataObjectReflection(dataObjectSchema);
                ReflectionCache.Add(objectReflection);
            }
            else
            {
                objectReflection = ReflectionCache[typeof(T)];
            }

            ComplexSearch search = SearchBuilder.Where(typeof(T), "_Id", id, s =>
            {
                s.AndEquals("_ObjectTypeId", objectReflection.Schema._Id);
                if (timestamp.HasValue) { s.AndLessOrEqual("_Timestamp", timestamp.Value.ToUniversalTime()); }
            });

            search.SearchMode = ComplexSearchMode.GetSingle;
            search.IncludeDeleted = includeDeleted;
            search.IncludeArchived = includeArchive;
            SearchContext context = _dataReader.GetRelationships(new SearchContext(search), connectionName);
            context.connectionName = connectionName;

            StringBuilder builder = new StringBuilder();
            _dataReader.BeginRead(builder);
            _dataReader.AddGetObjectQuery(builder, context);
            DataSet result = _dataReader.EndRead(builder, connectionName);
            context.loadedDepth += 3;
            DataObjectBuilder.BuildObjects(context, null, result);
            Dictionary<long, IDataObject> retrieveHash = null;
            if (context.RetrievedObjects.TryGetValue(objectReflection.Schema._Id, out retrieveHash))
            {
                IDataObject obj = null;
                if (retrieveHash.TryGetValue(id, out obj))
                {
                    return (T)obj;
                }
            }
            return default(T);
        }

        #endregion Get Object

        #region Get Objects
        /// <summary>
        /// Gets the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to get.</typeparam>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static List<T> GetObjects<T>() where T : IDataObject
        {
            return DataManager.Instance._GetObjects<T>(new long[]{}, null, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Gets the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to get.</typeparam>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static List<T> GetObjects<T>(string connectionName) where T : IDataObject
        {
            return DataManager.Instance._GetObjects<T>(new long[] { }, null, false, false, connectionName);
        }

        /// <summary>
        /// Gets the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to get.</typeparam>
        /// <param name="ids">The object identifiers.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static List<T> GetObjects<T>(long[] ids) where T : IDataObject
        {
            return DataManager.Instance._GetObjects<T>(ids, null, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Gets the most recent version of a collection of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to get.</typeparam>
        /// <param name="ids">The object identifiers.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static List<T> GetObjects<T>(long[] ids, string connectionName) where T : IDataObject
        {
            return DataManager.Instance._GetObjects<T>(ids, null, false, false, connectionName);
        }

        /// <summary>
        /// Gets the most recent version of a collection of objects prior to a date time.
        /// </summary>
        /// <typeparam name="T">The type of objects to get.</typeparam>
        /// <param name="ids">The object identifiers.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static List<T> GetObjects<T>(long[] ids, DateTime timestamp) where T : IDataObject
        {
            return DataManager.Instance._GetObjects<T>(ids, timestamp, false, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Gets the most recent version of a collection of objects prior to a date time.
        /// </summary>
        /// <typeparam name="T">The type of objects to get.</typeparam>
        /// <param name="ids">The object identifiers.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static List<T> GetObjects<T>(long[] ids, DateTime timestamp, string connectionName) where T : IDataObject
        {
            return DataManager.Instance._GetObjects<T>(ids, timestamp, false, false, connectionName);
        }

        /// <summary>
        /// Gets the most recent version of a collection of objects prior to a date time.
        /// </summary>
        /// <typeparam name="T">The type of objects to get.</typeparam>
        /// <param name="ids">The object identifiers.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static List<T> GetObjects<T>(long[] ids, DateTime? timestamp, bool includeDeleted) where T : IDataObject
        {
            return DataManager.Instance._GetObjects<T>(ids, timestamp, includeDeleted, false, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Gets the most recent version of a collection of objects prior to a date time.
        /// </summary>
        /// <typeparam name="T">The type of objects to get.</typeparam>
        /// <param name="ids">The object identifiers.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static List<T> GetObjects<T>(long[] ids, DateTime? timestamp, bool includeDeleted, string connectionName) where T : IDataObject
        {
            return DataManager.Instance._GetObjects<T>(ids, timestamp, includeDeleted, false, connectionName);
        }

        /// <summary>
        /// Gets the most recent version of a collection of objects prior to a date time.
        /// </summary>
        /// <typeparam name="T">The type of objects to get.</typeparam>
        /// <param name="ids">The object identifiers.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects.</param>
        /// <param name="includeArchive">Whether to include archived objects.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static List<T> GetObjects<T>(long[] ids, DateTime? timestamp, bool includeDeleted, bool includeArchive) where T : IDataObject
        {
            return DataManager.Instance._GetObjects<T>(ids, timestamp, includeDeleted, includeArchive, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Gets the most recent version of a collection of objects prior to a date time.
        /// </summary>
        /// <typeparam name="T">The type of objects to get.</typeparam>
        /// <param name="ids">The object identifiers.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="includeDeleted">Whether to include deleted objects.</param>
        /// <param name="includeArchive">Whether to include archived objects.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static List<T> GetObjects<T>(long[] ids, DateTime? timestamp, bool includeDeleted, bool includeArchive, string connectionName) where T : IDataObject
        {
            return DataManager.Instance._GetObjects<T>(ids, timestamp, includeDeleted, includeArchive, connectionName);
        }

        private List<T> _GetObjects<T>(long[] ids, DateTime? timestamp, bool includeDeleted, bool includeArchive, string connectionName) where T : IDataObject
        {
            if (ids == null)
            {
                throw new ArgumentNullException("ids");
            }

            DataObjectReflection objectReflection = null;

            if (!ReflectionCache.Contains(typeof(T)))
            {
                //If the reflection cache has never seen the object type, we need to make sure we have a valid place to store the object.
                //The script manager will adjust the database footprint for the object if necessary.
                DataObjectSchema dataObjectSchema = DataManager.ScriptManager.ScriptObjectSchema(typeof(T));
                objectReflection = new DataObjectReflection(dataObjectSchema);
                ReflectionCache.Add(objectReflection);
            }
            else
            {
                objectReflection = ReflectionCache[typeof(T)];
            }

            ComplexSearch search = SearchBuilder.Where(typeof(T), "_Id", ids, s =>
            {
                s.AndEquals("_ObjectTypeId", objectReflection.Schema._Id);
                if (timestamp.HasValue) { s.AndLessOrEqual("_Timestamp", timestamp.Value.ToUniversalTime()); }
            });

            search.SearchMode = ComplexSearchMode.GetMultiple;
            search.IncludeDeleted = includeDeleted;
            search.IncludeArchived = includeArchive;
            SearchContext context = _dataReader.GetRelationships(new SearchContext(search), connectionName);
            context.connectionName = connectionName;

            StringBuilder builder = new StringBuilder();
            _dataReader.BeginRead(builder);
            _dataReader.AddGetObjectQuery(builder, context);
            DataSet result = _dataReader.EndRead(builder, connectionName);
            context.loadedDepth += 3; 
            DataObjectBuilder.BuildObjects(context, null, result);
            Dictionary<long, IDataObject> retrieveHash = null;
            if (context.RetrievedObjects.TryGetValue(objectReflection.Schema._Id, out retrieveHash))
            {
                List<T> returnObjects = new List<T>();
                if (ids.Length > 0)
                {
                    foreach (long id in ids)
                    {
                        IDataObject obj = null;
                        if (retrieveHash.TryGetValue(id, out obj))
                        {
                            returnObjects.Add((T)obj);
                        }
                    }
                }
                else
                {
                    foreach (IDataObject obj in retrieveHash.Values)
                    {
                        returnObjects.Add((T)obj);
                    }
                }
                return returnObjects;
            }
            return new List<T>();
        }
        #endregion Get Objects

        #region Get Archived Object
        /// <summary>
        /// Gets the latest version of an archived object.
        /// </summary>
        /// <typeparam name="T">The type of object to get.</typeparam>
        /// <param name="id">The objects identifier.</param>
        public static T GetArchivedObject<T>(long id) where T : IDataObject
        {
            return DataManager.Instance._GetArchivedObject<T>(id, null, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Gets the latest version of an archived object.
        /// </summary>
        /// <typeparam name="T">The type of object to get.</typeparam>
        /// <param name="id">The objects identifier.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        public static T GetArchivedObject<T>(long id, string connectionName) where T : IDataObject
        {
            return DataManager.Instance._GetArchivedObject<T>(id, null, connectionName);
        }

        /// <summary>
        /// Gets the most recent version of an archived object prior to a date time.
        /// </summary>
        /// <typeparam name="T">The type of object to get.</typeparam>
        /// <param name="id">The objects identifier.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        public static T GetArchivedObject<T>(long id, DateTime timestamp) where T : IDataObject
        {
            return DataManager.Instance._GetArchivedObject<T>(id, timestamp, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Gets the most recent version of an archived object prior to a date time.
        /// </summary>
        /// <typeparam name="T">The type of object to get.</typeparam>
        /// <param name="id">The objects identifier.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        public static T GetArchivedObject<T>(long id, DateTime timestamp, string connectionName) where T : IDataObject
        {
            return DataManager.Instance._GetArchivedObject<T>(id, timestamp, connectionName);
        }

        private T _GetArchivedObject<T>(long id, DateTime? timestamp, string connectionName) where T : IDataObject
        {
            DataObjectReflection objectReflection = null;

            if (!ReflectionCache.Contains(typeof(T)))
            {
                //If the reflection cache has never seen the object type, we need to make sure we have a valid place to store the object.
                //The script manager will adjust the database footprint for the object if necessary.
                DataObjectSchema dataObjectSchema = DataManager.ScriptManager.ScriptObjectSchema(typeof(T));
                objectReflection = new DataObjectReflection(dataObjectSchema);
                ReflectionCache.Add(objectReflection);
            }
            else
            {
                objectReflection = ReflectionCache[typeof(T)];
            }

            ComplexSearch search = SearchBuilder.Where(typeof(T), "_Id", id, s =>
            {
                s.AndEquals("_ObjectTypeId", objectReflection.Schema._Id);
                if (timestamp.HasValue) { s.AndLessOrEqual("_Timestamp", timestamp.Value.ToUniversalTime()); }
                s.AndEquals("_Deleted", 1);
            });

            search.SearchMode = ComplexSearchMode.GetSingle;
            search.IncludeDeleted = true;
            search.IncludeArchived = true;
            SearchContext context = _dataReader.GetRelationships(new SearchContext(search), connectionName);
            context.connectionName = connectionName;

            StringBuilder builder = new StringBuilder();
            _dataReader.BeginRead(builder);
            _dataReader.AddGetObjectQuery(builder, context);
            DataSet result = _dataReader.EndRead(builder, connectionName);
            context.loadedDepth += 3;
            DataObjectBuilder.BuildObjects(context, null, result);
            Dictionary<long, IDataObject> retrieveHash = null;
            if (context.RetrievedObjects.TryGetValue(objectReflection.Schema._Id, out retrieveHash))
            {
                IDataObject obj = null;
                if (retrieveHash.TryGetValue(id, out obj))
                {
                    return (T)obj;
                }
            }
            return default(T);
        }
        #endregion Get Archived Object

        #region Get Archived Objects

        /// <summary>
        /// Gets the most recent version of a collection of archived objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to get.</typeparam>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static List<T> GetArchivedObjects<T>() where T : IDataObject
        {
            return DataManager.Instance._GetArchivedObjects<T>(new long[] { }, null, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Gets the most recent version of a collection of archived objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to get.</typeparam>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static List<T> GetArchivedObjects<T>(string connectionName) where T : IDataObject
        {
            return DataManager.Instance._GetArchivedObjects<T>(new long[] { }, null, connectionName);
        }

        /// <summary>
        /// Gets the most recent version of a collection of archived objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to get.</typeparam>
        /// <param name="ids">The object identifiers.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static List<T> GetArchivedObjects<T>(long[] ids) where T : IDataObject
        {
            return DataManager.Instance._GetArchivedObjects<T>(ids, null, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Gets the most recent version of a collection of archived objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to get.</typeparam>
        /// <param name="ids">The object identifiers.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static List<T> GetArchivedObjects<T>(long[] ids, string connectionName) where T : IDataObject
        {
            return DataManager.Instance._GetArchivedObjects<T>(ids, null, connectionName);
        }

        /// <summary>
        /// Gets the most recent version of a collection of archived objects prior to a date time.
        /// </summary>
        /// <typeparam name="T">The type of objects to get.</typeparam>
        /// <param name="ids">The object identifiers.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static List<T> GetArchivedObjects<T>(long[] ids, DateTime timestamp) where T : IDataObject
        {
            return DataManager.Instance._GetArchivedObjects<T>(ids, timestamp, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Gets the most recent version of a collection of archived objects prior to a date time.
        /// </summary>
        /// <typeparam name="T">The type of objects to get.</typeparam>
        /// <param name="ids">The object identifiers.</param>
        /// <param name="timestamp">The objects timestamp.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static List<T> GetArchivedObjects<T>(long[] ids, DateTime timestamp, string connectionName) where T : IDataObject
        {
            return DataManager.Instance._GetArchivedObjects<T>(ids, timestamp, connectionName);
        }

        private List<T> _GetArchivedObjects<T>(long[] ids, DateTime? timestamp, string connectionName) where T : IDataObject
        {
            if (ids == null)
            {
                throw new ArgumentNullException("ids");
            }

            DataObjectReflection objectReflection = null;

            if (!ReflectionCache.Contains(typeof(T)))
            {
                //If the reflection cache has never seen the object type, we need to make sure we have a valid place to store the object.
                //The script manager will adjust the database footprint for the object if necessary.
                DataObjectSchema dataObjectSchema = DataManager.ScriptManager.ScriptObjectSchema(typeof(T));
                objectReflection = new DataObjectReflection(dataObjectSchema);
                ReflectionCache.Add(objectReflection);
            }
            else
            {
                objectReflection = ReflectionCache[typeof(T)];
            }

            ComplexSearch search = SearchBuilder.Where(typeof(T), "_Id", ids, s =>
            {
                s.AndEquals("_ObjectTypeId", objectReflection.Schema._Id);
                if (timestamp.HasValue) { s.AndLessOrEqual("_Timestamp", timestamp.Value.ToUniversalTime()); }
                s.AndEquals("_Deleted", 1);
            });

            search.SearchMode = ComplexSearchMode.GetMultiple;
            search.IncludeDeleted = true;
            search.IncludeArchived = true;
            SearchContext context = _dataReader.GetRelationships(new SearchContext(search), connectionName);
            context.connectionName = connectionName;

            StringBuilder builder = new StringBuilder();
            _dataReader.BeginRead(builder);
            _dataReader.AddGetObjectQuery(builder, context);
            DataSet result = _dataReader.EndRead(builder, connectionName);
            context.loadedDepth += 3;
            DataObjectBuilder.BuildObjects(context, null, result);
            Dictionary<long, IDataObject> retrieveHash = null;
            if (context.RetrievedObjects.TryGetValue(objectReflection.Schema._Id, out retrieveHash))
            {
                List<T> returnObjects = new List<T>();
                if (ids.Length > 0)
                {
                    foreach (long id in ids)
                    {
                        IDataObject obj = null;
                        if (retrieveHash.TryGetValue(id, out obj))
                        {
                            returnObjects.Add((T)obj);
                        }
                    }
                }
                else
                {
                    foreach (IDataObject obj in retrieveHash.Values)
                    {
                        returnObjects.Add((T)obj);
                    }
                }
                return returnObjects;
            }
            return new List<T>();
        }

        #endregion Get Archived Objects

        #endregion Get

        #region Client Side Scripting

        /// <summary>
        /// Registers an object for use with client side scripting.
        /// </summary>
        /// <typeparam name="T">The type of object to register.</typeparam>
        /// <param name="outStream">The output stream to register the object with. (Response stream)</param>
        public static void RegisterClass<T>(Stream outStream) where T : IDataObject
        {
            StringBuilder sb = new StringBuilder();

            DataObjectReflection objectReflection = null;

            if (!ReflectionCache.Contains(typeof(T)))
            {
                //If the reflection cache has never seen the object type, we need to make sure we have a valid place to store the object.
                //The script manager will adjust the database footprint for the object if necessary.
                DataObjectSchema dataObjectSchema = DataManager.ScriptManager.ScriptObjectSchema(typeof(T));
                objectReflection = new DataObjectReflection(dataObjectSchema);
                ReflectionCache.Add(objectReflection);
            }
            else
            {
                objectReflection = ReflectionCache[typeof(T)];
            }

            using (StreamWriter writer = new StreamWriter(outStream))
            {
                writer.Write("<script type=\"text/javascript\">");
                writer.Write("function " + objectReflection.Schema.ObjectType.Name + "(){");
                foreach (DataObjectSchemaField field in objectReflection.Schema.Fields)
                {
                    if ((byte)field.FieldType < 21 || field.FieldType == DataObjectSchemaFieldType.IDataObject)
                    {
                        writer.Write(string.Format("this.{0} = null;", field.Name));
                    }
                    else
                    {
                        writer.Write(string.Format("this.{0} = new Array();", field.Name));
                    }
                }
                writer.Write("return this;}");
                writer.Write("</script>");
            }
        }

        #endregion Client Side Scripting

        #region Misc

        private static SavableObjectAttribute PreSaveValidate(IDataObject obj)
        {
            return DataManager.Instance._PreSaveValidate(obj);
        }

        private SavableObjectAttribute _PreSaveValidate(IDataObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            string errorMessage = null;
            if (!this.ValidateSettings(out errorMessage))
            {
                throw new InvalidSettingsException(errorMessage);
            }

            object[] attributes = obj.GetType().GetCustomAttributes(typeof(SavableObjectAttribute), false);
            SavableObjectAttribute attribute = attributes.Length > 0 ? attributes[0] as SavableObjectAttribute : null;
            if (attribute == null)
            {
                throw new InvalidObjectSchemaException("The data relational object could not be saved because it is not tagged by the SavableObject attribute.");
            }

            try
            {
                DataManager.ScriptManager.ScriptDatabase();
            }
            catch (Exception ex)
            {
                throw new SaveException("The data relational object could not be saved because the data manager threw an exception while attempting to script the database.  See the inner exception for details.", ex);
            }

            return attribute;
        }

        private bool ValidateSettings(out string errorMessage)
        {
            if (DataManager.Settings.ConnectionStrings.Count == 0)
            {
                errorMessage = "The connection string collection cannot be empty.";
                return false;
            }

            errorMessage = null;
            return true;
        }

        #endregion Misc
    }
}
