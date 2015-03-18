using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Configuration;

namespace DataRelational
{
    /// <summary>
    /// Contains the configuration settings for the <see cref="DataRelational.DataManager">DataManager</see>.
    /// </summary>
    public class DataManagerSettings : ConfigurationSection
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the DataRelational.DataManagerSettings class.
        /// </summary>
        internal DataManagerSettings()
        {
            this.ConnectionStrings.Changed += new EventHandler(ConnectionStrings_Changed);
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets the list of database connection strings.
        /// </summary>
        [ConfigurationProperty("ConnectionStrings", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ConnectionStringCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public ConnectionStringCollection ConnectionStrings
        {
            get
            {
                return base["ConnectionStrings"] as ConnectionStringCollection;
            }
        }

        /// <summary>
        /// Gets the logging settings.
        /// </summary>
        [ConfigurationProperty("Logging")]
        public LoggingElement Logging
        {
            get
            {
                return base["Logging"] as LoggingElement;
            }
        }

        /// <summary>
        /// Gets the type of database.
        /// </summary>
        public DatabaseType DatabaseType
        {
            get
            {
                if (ConnectionStrings.Count > 0)
                    return ConnectionStrings[0].Type;
                return DatabaseType.SqlServer;
            }
        }

        #endregion Properties

        #region Events

        internal event EventHandler SettingsChanged;

        #endregion Events

        #region Event Triggers

        void OnSettingsChanged(EventArgs e)
        {
            SafeInvoke.Invoke(SettingsChanged, null, new object[] { this, e });
        }

        void ConnectionStrings_Changed(object sender, EventArgs e)
        {
            OnSettingsChanged(new EventArgs());
        }

        #endregion Events Triggers
    }

    /// <summary>
    /// Contains a list of named connection strings
    /// </summary>
    public class ConnectionStringCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Initializes a new instance of the DataRelational.ConnectionStringCollection class.
        /// </summary>
        public ConnectionStringCollection()
        {
        }
        
        /// <summary>
        /// Gets the type of System.Configuration.ConfigurationElementCollection
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        /// <summary>
        /// When overridden in a derived class, creates a new System.Configuration.ConfigurationElement.
        /// </summary>
        /// <returns>A new DataRelational.ConnectionStringElement.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ConnectionStringElement();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The System.Configuration.ConfigurationElement to return the key for.</param>
        /// <returns>An System.Object that acts as the key for the specified System.Configuration.ConfigurationElement.</returns>
        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((ConnectionStringElement)element).Name;
        }

        /// <summary>
        /// Gets or sets the connection string at the specified index
        /// </summary>
        /// <param name="index">The index of the connection string</param>
        /// <returns>The connection string at the index</returns>
        public ConnectionStringElement this[int index]
        {
            get
            {
                return (ConnectionStringElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        /// <summary>
        /// Gets or sets the connection string with the specified name
        /// </summary>
        /// <param name="name">The name of the connection string</param>
        /// <returns>The connection string with the matching name</returns>
        new public ConnectionStringElement this[string name]
        {
            get
            {
                return (ConnectionStringElement)BaseGet(name);
            }
        }

        /// <summary>
        /// Gets the index of the specified connection string element.
        /// </summary>
        /// <param name="element">The connection string to get the index of.</param>
        /// <returns>The index of the connection string element.</returns>
        public int IndexOf(ConnectionStringElement element)
        {
            return BaseIndexOf(element);
        }

        /// <summary>
        /// Adds a connection string element to the collection.
        /// </summary>
        /// <param name="element">The connectio string element to add.</param>
        public void Add(ConnectionStringElement element)
        {
            BaseAdd(element);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        protected override void BaseAdd(ConfigurationElement element)
        {
            ((ConnectionStringElement)element).Changed += new EventHandler(ConnectionStringCollection_Changed);
            BaseAdd(element, false);
            if (Changed != null)
                Changed(this, new EventArgs());
        }

        /// <summary>
        /// Removes a connection string element from the collection.
        /// </summary>
        /// <param name="element">The connection string element to remove.</param>
        public void Remove(ConnectionStringElement element)
        {
            if (BaseIndexOf(element) >= 0)
                BaseRemove(element.Name);
            ((ConnectionStringElement)element).Changed -= new EventHandler(ConnectionStringCollection_Changed);
            if (Changed != null)
                Changed(this, new EventArgs());
        }

        /// <summary>
        /// Removes a connection string element from the collection.
        /// </summary>
        /// <param name="index">The index of connection string element to remove.</param>
        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
            if (Changed != null)
                Changed(this, new EventArgs());
        }

        /// <summary>
        /// Removes a connection string element from the collection.
        /// </summary>
        /// <param name="name">The name of connection string element to remove.</param>
        public void Remove(string name)
        {
            BaseRemove(name);
            if (Changed != null)
                Changed(this, new EventArgs());
        }

        /// <summary>
        /// Clears the connection string collection.
        /// </summary>
        public void Clear()
        {
            BaseClear();
            if (Changed != null)
                Changed(this, new EventArgs());
        }

        void ConnectionStringCollection_Changed(object sender, EventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }

        /// <summary>
        /// Occurs when the connection string collection is modified
        /// </summary>
        public event EventHandler Changed;
    }

    /// <summary>
    /// Contains a named connection string
    /// </summary>
    public class ConnectionStringElement : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the DataRelational.ConnectionStringElement class.
        /// </summary>
        public ConnectionStringElement()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DataRelational.ConnectionStringElement class.
        /// </summary>
        /// <param name="name">the name of the connection string</param>
        /// <param name="connection">the connection string</param>
        /// <param name="type">the type of database connection</param>
        public ConnectionStringElement(string name, string connection, DatabaseType type)
        {
            this.Name = name;
            this.Connection = connection;
            this.Type = type;
        }

        /// <summary>
        /// Gets or sets the name of the connection string
        /// </summary>
        [ConfigurationProperty("name", DefaultValue = "", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                if ((string)this["name"] != value)
                {
                    this["name"] = value;
                    if (Changed != null)
                        Changed(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Gets or sets the connection string
        /// </summary>
        [ConfigurationProperty("connection", DefaultValue = "", IsRequired = true)]
        public string Connection
        {
            get
            {
                return (string)this["connection"];
            }
            set
            {
                if ((string)this["connection"] != value)
                {
                    this["connection"] = value;
                    if (Changed != null)
                        Changed(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Gets the connection strings database type
        /// </summary>
        [ConfigurationProperty("type", DefaultValue = "SqlServer", IsRequired = true)]
        public DatabaseType Type
        {
            get
            {
                return (DatabaseType)this["type"];
            }
            set
            {
                if ((DatabaseType)this["type"] != value)
                {
                    this["type"] = value;
                    if (Changed != null)
                        Changed(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Occurs when the connection string is modified
        /// </summary>
        public event EventHandler Changed;
    }

    /// <summary>
    /// Contains logging settings
    /// </summary>
    public class LoggingElement : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the DataRelational.LoggingElement class.
        /// </summary>
        public LoggingElement()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DataRelational.LoggingElement class.
        /// </summary>
        /// <param name="information">whether information logs should be recorded.</param>
        /// <param name="debug">whether debug logs should be recorded.</param>
        /// <param name="warning">whether warning logs should be recorded.</param>
        /// <param name="exception">whether exception logs should be recorded.</param>
        public LoggingElement(bool information, bool debug, bool warning, bool exception)
        {
            this.Information = information;
            this.Debug = debug;
            this.Warning = warning;
            this.Exception = exception;
        }

        /// <summary>
        /// Gets or sets whether debug logs should be recorded.
        /// </summary>
        [ConfigurationProperty("debug", DefaultValue = "false", IsRequired = false, IsKey = true)]
        public bool Debug
        {
            get
            {
                return (bool)this["debug"];
            }
            set
            {
                this["debug"] = value;
            }
        }

        /// <summary>
        /// Gets or sets whether information logs should be recorded.
        /// </summary>
        [ConfigurationProperty("information", DefaultValue = "false", IsRequired = false, IsKey = true)]
        public bool Information
        {
            get
            {
                return (bool)this["information"];
            }
            set
            {
                this["information"] = value;
            }
        }

        /// <summary>
        /// Gets or sets whether warning logs should be recorded.
        /// </summary>
        [ConfigurationProperty("warning", DefaultValue = "false", IsRequired = false, IsKey = true)]
        public bool Warning
        {
            get
            {
                return (bool)this["warning"];
            }
            set
            {
                this["warning"] = value;
            }
        }

        /// <summary>
        /// Gets or sets whether exception logs should be recorded.
        /// </summary>
        [ConfigurationProperty("exception", DefaultValue = "false", IsRequired = false, IsKey = true)]
        public bool Exception
        {
            get
            {
                return (bool)this["exception"];
            }
            set
            {
                this["exception"] = value;
            }
        }
    }
}
