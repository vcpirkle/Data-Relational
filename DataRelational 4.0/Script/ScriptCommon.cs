using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Reflection;
using DataRelational.Attributes;
using DataRelational.Cache;
using System.Collections;
using System.Runtime.CompilerServices;

namespace DataRelational.Script
{
    class ScriptCommon
    {
        #region Fields
        private static ConnectionStringParser _Parser;
        private static Type[] primativeTypes = new Type[]{
            typeof(bool),       //0
            typeof(byte),       //1 
            typeof(short),      //2 
            typeof(int),        //3 
            typeof(long),       //4
            typeof(double),     //5
            typeof(float),      //6
            typeof(decimal),    //7
            typeof(DateTime),   //8
            typeof(Guid),       //9
            typeof(string),     //10
            typeof(bool?),      //11
            typeof(byte?),      //12
            typeof(short?),     //13
            typeof(int?),       //14 
            typeof(long?),      //15
            typeof(double?),    //16
            typeof(float?),     //17
            typeof(decimal?),   //18
            typeof(DateTime?),  //19
            typeof(Guid?)};     //20
        #endregion Fields

        #region Properties
        /// <summary>
        /// Gets the list of supported primative types.  
        /// </summary>
        public static List<Type> PrimativeTypes
        {
            get
            {
                return new List<Type>(primativeTypes);
            }
        }

        /// <summary>
        /// Gets the connection string parser
        /// </summary>
        public static ConnectionStringParser Parser
        {
            get
            {
                if (_Parser == null)
                    _Parser = new ConnectionStringParser();
                return _Parser;
            }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Pareses a tsql command on the GO statement and executes each individual tsql block
        /// </summary>
        /// <param name="connection">The connection to execute the command on</param>
        /// <param name="commandText">The tsql command to execute</param>
        public static void ExecuteCommand(DbConnection connection, string commandText)
        {
            try
            {
                string[] commands = commandText.Split(new string[] { "GO\r\n", "GO ", "GO\t" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string commandBlock in commands)
                {
                    using (DbCommand command = connection.CreateCommand())
                    {
                        command.CommandText = commandBlock;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DataRelationalException("An exception occured while attempting to execute a database command.", ex);
            }
        }

        /// <summary>
        /// Gets the System.Type of an array type.
        /// </summary>
        /// <param name="type">The type of array.</param>
        public static Type GetArrayType(Type type)
        {
            if (type.IsArray && type.GetArrayRank() == 1)
            {
                foreach (MemberInfo member in type.GetMembers())
                {
                    if (member.Name == "Get")
                    {
                        Type t = Type.GetType(member.DeclaringType.FullName.Replace("[]", ""));
                        if (t != null)
                            return t;
                        else
                        {
                            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                            {
                                t = a.GetType(member.DeclaringType.FullName.Replace("[]", ""));
                                if (t != null)
                                    return t;
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the System.Type of a data relationship.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="objectReflection">The data relational object reflection.</param>
        /// <returns></returns>
        public static Type GetRelationshipType(string fieldName, DataObjectReflection objectReflection)
        {
            Type relationshipType = null;
            foreach (FieldInfo fld in objectReflection.RelationshipFields)
            {
                if (fld.Name.ToLower() == fieldName.ToLower())
                {
                    if (fld.FieldType.IsGenericType && fld.FieldType.GetGenericTypeDefinition() == typeof(DataRelationship<>))
                    {
                        if (fld.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Length == 0)
                        {
                            Type underlyingType = DataRelationship.GetUnderlyingType(fld.FieldType);
                            if (underlyingType.GetInterface("DataRelational.IDataObject", false) != null)
                            {
                                relationshipType = underlyingType;
                            }
                            else if (ScriptCommon.GetArrayType(underlyingType) != null && ScriptCommon.GetArrayType(underlyingType).GetInterface("DataRelational.IDataObject", false) != null)
                            {
                                relationshipType = ScriptCommon.GetArrayType(underlyingType);
                            }
                            else if (underlyingType.IsGenericType && underlyingType.GetGenericTypeDefinition() == typeof(List<>))
                            {
                                relationshipType = underlyingType.GetGenericArguments()[0];
                            }
                        }
                    }
                }
            }
            if (relationshipType != null)
            {
                return relationshipType;
            }
            else
            {
                foreach (PropertyInfo prop in objectReflection.RelationshipProperties)
                {
                    if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(DataRelationship<>))
                    {
                        Type underlyingType = DataRelationship.GetUnderlyingType(prop.PropertyType);
                        if (underlyingType.GetInterface("DataRelational.IDataObject", false) != null)
                        {
                            relationshipType = underlyingType;
                        }
                        else if (ScriptCommon.GetArrayType(underlyingType) != null && ScriptCommon.GetArrayType(underlyingType).GetInterface("DataRelational.IDataObject", false) != null)
                        {
                            relationshipType = ScriptCommon.GetArrayType(underlyingType);
                        }
                        else if (underlyingType.IsGenericType && underlyingType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            relationshipType = underlyingType.GetGenericArguments()[0];
                        }
                    }
                }
            }
            return relationshipType;
        }

        /// <summary>
        /// Gets the System.Type by name.
        /// </summary>
        /// <param name="typeName">The name of the type.</param>
        public static Type GetType(string typeName)
        {
            Type t = Type.GetType(typeName);
            if (t != null)
                return t;
            else
            {
                foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    t = a.GetType(typeName);
                    if (t != null)
                        return t;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the list of child data relational objects for a parent.
        /// </summary>
        /// <param name="obj">The parent data relational object.</param>
        /// <param name="objectReflection">The data relational object reflection.</param>
        public static List<FieldChildObject> GetChildren(IDataObject obj, DataObjectReflection objectReflection)
        {
            List<FieldChildObject> children = new List<FieldChildObject>();
            foreach (FieldInfo fld in objectReflection.RelationshipFields)
            {
                if (fld.FieldType.IsGenericType && fld.FieldType.GetGenericTypeDefinition() == typeof(DataRelationship<>))
                {
                    if (fld.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Length == 0)
                    {
                        Type underlyingType = DataRelationship.GetUnderlyingType(fld.FieldType);
                        if (underlyingType.GetInterface("DataRelational.IDataObject", false) != null)
                        {
                            IDataRelationship o = fld.GetValue(obj) as IDataRelationship;
                            if (o != null && o.HasValue) { children.Add(new FieldChildObject(fld.Name, o.GetValue() as IDataObject)); }
                        }
                        else if (ScriptCommon.GetArrayType(underlyingType) != null && ScriptCommon.GetArrayType(underlyingType).GetInterface("DataRelational.IDataObject", false) != null)
                        {
                            IDataRelationship dataRelationship = fld.GetValue(obj) as IDataRelationship;
                            if (dataRelationship != null && dataRelationship.HasValue)
                            {
                                IList list = dataRelationship.GetValue() as IList;
                                if (list != null)
                                {
                                    foreach (IDataObject o in list)
                                    {
                                        if (o != null) { children.Add(new FieldChildObject(fld.Name, o)); }
                                    }
                                }
                            }
                        }
                        else if (underlyingType.IsGenericType && underlyingType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            IDataRelationship dataRelationship = fld.GetValue(obj) as IDataRelationship;
                            if (dataRelationship != null && dataRelationship.HasValue)
                            {
                                IList list = dataRelationship.GetValue() as IList;
                                if (list != null)
                                {
                                    foreach (IDataObject o in list)
                                    {
                                        if (o != null) { children.Add(new FieldChildObject(fld.Name, o)); }
                                    }
                                }
                            }
                        }
                        else if (underlyingType.IsGenericType && underlyingType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                        {
                        }
                    }
                }
            }
            foreach (PropertyInfo prop in objectReflection.RelationshipProperties)
            {
                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(DataRelationship<>))
                {
                    Type underlyingType = DataRelationship.GetUnderlyingType(prop.PropertyType);
                    if (underlyingType.GetInterface("DataRelational.IDataObject", false) != null)
                    {
                        IDataRelationship o = prop.GetValue(obj, null) as IDataRelationship;
                        if (o != null && o.HasValue) { children.Add(new FieldChildObject(prop.Name, o.GetValue() as IDataObject)); }
                    }
                    else if (ScriptCommon.GetArrayType(underlyingType) != null && ScriptCommon.GetArrayType(underlyingType).GetInterface("DataRelational.IDataObject", false) != null)
                    {
                        IDataRelationship dataRelationship = prop.GetValue(obj, null) as IDataRelationship;
                        if (dataRelationship != null && dataRelationship.HasValue)
                        {
                            IList list = dataRelationship.GetValue() as IList;
                            if (list != null)
                            {
                                foreach (IDataObject o in list)
                                {
                                    if (o != null) { children.Add(new FieldChildObject(prop.Name, o)); }
                                }
                            }
                        }
                    }
                    else if (underlyingType.IsGenericType && underlyingType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        IDataRelationship dataRelationship = prop.GetValue(obj, null) as IDataRelationship;
                        if (dataRelationship != null && dataRelationship.HasValue)
                        {
                            IList list = dataRelationship.GetValue() as IList;
                            if (list != null)
                            {
                                foreach (IDataObject o in list)
                                {
                                    if (o != null) { children.Add(new FieldChildObject(prop.Name, o)); }
                                }
                            }
                        }
                    }
                    else if (underlyingType.IsGenericType && underlyingType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                    }
                }
            }
            return children;
        }

        /// <summary>
        /// Gets the list of child data relational objects for a parent.
        /// </summary>
        /// <param name="obj">The parent data relational object.</param>
        /// <param name="objectReflection">The data relational object reflection.</param>
        public static List<IDataRelationship> GetChildRelationships(IDataObject obj, DataObjectReflection objectReflection)
        {
            List<IDataRelationship> children = new List<IDataRelationship>();
            foreach (FieldInfo fld in objectReflection.RelationshipFields)
            {
                if (fld.FieldType.IsGenericType && fld.FieldType.GetGenericTypeDefinition() == typeof(DataRelationship<>))
                {
                    if (fld.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Length == 0)
                    {
                        children.Add((IDataRelationship)fld.GetValue(obj));
                    }
                }
            }
            foreach (PropertyInfo prop in objectReflection.RelationshipProperties)
            {
                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(DataRelationship<>))
                {
                    children.Add((IDataRelationship)prop.GetValue(obj, null));
                }
            }
            return children;
        }

        /// <summary>
        /// Applys the database write times to an object and its children
        /// </summary>
        /// <param name="obj">The parent data relational object.</param>
        /// <param name="writeTimes">The database write times</param>
        public static void ApplyWriteTimes(IDataObject obj, DataTable writeTimes)
        {
            _ApplyWriteTimes(obj, writeTimes, new List<Guid>());
        }

        private static void _ApplyWriteTimes(IDataObject obj, DataTable writeTimes, List<Guid> instanceIdentifiers)
        {
            //Make sure only one instance of an object gets processed
            if (!instanceIdentifiers.Contains(obj._InstanceIdentifier))
            {
                ApplyWriteTime(obj, writeTimes);
                instanceIdentifiers.Add(obj._InstanceIdentifier);
                foreach (FieldChildObject child in ScriptCommon.GetChildren(obj, DataManager.ReflectionCache[obj.GetType()]))
                {
                    _ApplyWriteTimes(child.ChildObject, writeTimes, instanceIdentifiers);
                }
            }
        }

        private static void ApplyWriteTime(IDataObject obj, DataTable writeTimes)
        {
            foreach (DataRow row in writeTimes.Rows)
            {
                if ((long)row[0] == obj._Id && (short)row[2] == obj._ObjectType._Id)
                {
                    IHistoricalDataObject hObj = obj as IHistoricalDataObject;
                    if (hObj != null)
                        hObj._RevisionId = (int)row[1];
                    obj._Timestamp = (DateTime)row[3];
                    obj._Dirty = false;
                    break;
                }
            }
        }

        #endregion Methods
    }

    class FieldChildObject
    {
        public string FieldName { get; private set; }
        public IDataObject ChildObject { get; private set; }
        public FieldChildObject(string fieldName, IDataObject childObject)
        {
            this.FieldName = fieldName;
            this.ChildObject = childObject;
        }
    }
}
