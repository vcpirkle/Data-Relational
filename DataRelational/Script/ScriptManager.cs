using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataRelational.Schema;
using System.Data.Common;
using DataRelational.Search;

namespace DataRelational.Script
{
    /// <summary>
    /// Manages the IDatabaseScripter objects in a centralized manor.
    /// </summary>
    class ScriptManager
    {
        #region Fields

        private bool _hasScriptedDatabase;
        private Dictionary<string,IDatabaseScripter> _scripters;

        #endregion Fields

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the DataRelational.Script.ScriptManager class.
        /// </summary>
        public ScriptManager()
        {
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Initializes the script manager
        /// </summary>
        public void Initialize()
        {
            DataManager.Settings.SettingsChanged += new EventHandler(Settings_SettingsChanged);
            _scripters = GetScripters();
        }

        /// <summary>
        /// Scripts the database according to the <see cref="DataRelational.DataManagerSettings"/>.
        /// </summary>
        public void ScriptDatabase()
        {
            if (!_hasScriptedDatabase)
            {
                Logging.Debug(string.Format("Attempting to script the data relational database on the connection {0}.", DataManager.Settings.ConnectionStrings[0].Name));
                _scripters.First().Value.ScriptDatabase(DataManager.Settings.ConnectionStrings[0].Name, true);
                _hasScriptedDatabase = true;
            }
        }

        /// <summary>
        /// Scripts the object type according to the <see cref="DataRelational.DataManagerSettings"/>.
        /// </summary>
        public DataObjectSchema ScriptObjectSchema(Type objectType)
        {
            Logging.Debug(string.Format("Attempting to script the data object schema {0} on the connection {1}.", objectType.Namespace + "." + objectType.Name, DataManager.Settings.ConnectionStrings[0].Name));
            return _scripters.First().Value.ScriptObjectSchema(objectType, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Scripts the object type according to the <see cref="DataRelational.DataManagerSettings"/>.
        /// </summary>
        public DataObjectSchema GetObjectSchema(short objectTypeId)
        {
            Logging.Debug(string.Format("Attempting to script the data object schema {0} on the connection {1}.", objectTypeId, DataManager.Settings.ConnectionStrings[0].Name));
            return _scripters.First().Value.GetObjectSchema(objectTypeId, DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Gets the next available identifier for an object type
        /// </summary>
        public long GetNextIdentifier(Type objectType)
        {
            Logging.Debug(string.Format("Attempting to get the next identifier for the data object type {0} on the connection {1}.", objectType.Namespace + "." + objectType.Name, DataManager.Settings.ConnectionStrings[0].Name));
            long nextIdentifier = _scripters.First().Value.GetNextIdentifier(objectType, DataManager.Settings.ConnectionStrings[0].Name);
            Logging.Information(string.Format("Next Available Identifer for for the data object type {0} - {1}.", objectType.Namespace + "." + objectType.Name, nextIdentifier));
            return nextIdentifier;
        }

        /// <summary>
        /// Opens a connection to the database.
        /// </summary>
        public DbConnection GetDatabaseConnection()
        {
            Logging.Debug(string.Format("Attempting to open a database connection using the connection string {0}.", DataManager.Settings.ConnectionStrings[0].Name));
            return _scripters.First().Value.GetDatabaseConnection(DataManager.Settings.ConnectionStrings[0].Name);
        }

        /// <summary>
        /// Opens a connection to the database.
        /// </summary>
        public DbConnection GetDatabaseConnection(string connectionName)
        {
            Logging.Debug(string.Format("Attempting to open a database connection using the connection string {0}.", connectionName));
            foreach (string key in _scripters.Keys)
            {
                if (key == connectionName)
                {
                    return _scripters[key].GetDatabaseConnection(connectionName);
                }
            }
            throw new ScriptException(string.Format("Could not find a connection string with the name {0}.", connectionName));
        }

        /// <summary>
        /// Gets a data adapter.
        /// </summary>
        public DbDataAdapter GetDataAdapter()
        {
            return _scripters.First().Value.GetDataAdapter();
        }

        private void Settings_SettingsChanged(object sender, EventArgs e)
        {
            _hasScriptedDatabase = false;
            _scripters = null;
            _scripters = GetScripters();
        }

        private Dictionary<string,IDatabaseScripter> GetScripters()
        {
            Dictionary<string, IDatabaseScripter> scripters = new Dictionary<string, IDatabaseScripter>();
            foreach (ConnectionStringElement cString in DataManager.Settings.ConnectionStrings)
            {
                switch (cString.Type)
                {
                    default:
                        scripters.Add(cString.Name, new SqlServerDatabaseScripter());
                        break;
                }
            }
            return scripters;
        }

        #endregion Methods
    }
}
