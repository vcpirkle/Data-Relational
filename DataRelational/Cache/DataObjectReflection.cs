using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using DataRelational.Attributes;
using DataRelational.Script;
using DataRelational.Schema;

namespace DataRelational.Cache
{
    class DataObjectReflection
    {
        #region Fields
        private DataObjectSchema dataObjectSchema;
        private ConstructorInfo constructor;
        private List<FieldInfo> dataFields;
        private List<PropertyInfo> dataProperties;
        private List<FieldInfo> relationshipFields;
        private List<PropertyInfo> relationshipProperties;
        private long nextIdentifier;
        #endregion Fields

        #region Constructor
        /// <summary>
        /// Creates a new instance of ObjectReflection
        /// </summary>
        /// <param name="dataObjectSchema">The data object schema for the object</param>
        public DataObjectReflection(DataObjectSchema dataObjectSchema)
        {
            this.dataObjectSchema = dataObjectSchema;
            this.nextIdentifier = 1;
        }
        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets the data object type
        /// </summary>
        public DataObjectSchema Schema
        {
            get { return dataObjectSchema; }
        }

        /// <summary>
        /// Gets the empty constructor for the object type.
        /// </summary>
        public ConstructorInfo Constructor
        {
            get
            {
                return constructor;
            }
        }

        /// <summary>
        /// Gets the list of primative data fields marked by the data field attribute.
        /// </summary>
        public List<FieldInfo> DataFields
        {
            get
            {
                return new List<FieldInfo>(dataFields);
            }
        }

        /// <summary>
        /// Gets the list of primative data properties marked by the data field attribute.
        /// </summary>
        public List<PropertyInfo> DataProperties
        {
            get
            {
                return new List<PropertyInfo>( dataProperties);
            }
        }

        /// <summary>
        /// Gets the list of fields that at least implement the IDataObject interface.
        /// </summary>
        public List<FieldInfo> RelationshipFields
        {
            get
            {
                return new List<FieldInfo>(relationshipFields);
            }
        }

        /// <summary>
        /// Gets the list of properties that at least implement the IDataObject interface.
        /// </summary>
        public List<PropertyInfo> RelationshipProperties
        {
            get
            {
                return new List<PropertyInfo>(relationshipProperties);
            }
        }

        /// <summary>
        /// Gets the next identifier for this data object type
        /// </summary>
        public long NextIdentifier
        {
            get
            {
                long next = nextIdentifier;
                nextIdentifier++;
                return next;
            }
        }

        #endregion Properties

        #region Methods

        public void BuildReflection()
        {
            if (constructor == null)
            {
                constructor = dataObjectSchema.ObjectType.GetConstructor(Type.EmptyTypes);
                if (constructor == null)
                    throw new InvalidObjectSchemaException("The type " + dataObjectSchema.ObjectType.Name + " does not have an empty constructor.");

                dataFields = GetDataFields();
                dataProperties = GetDataProperties();
                if (dataFields.Count == 0 && dataProperties.Count == 0)
                    throw new InvalidObjectSchemaException("The type " + dataObjectSchema.ObjectType.Name + " does not have any fields marked as data fields.");

                relationshipFields = GetRelationshipFields();
                relationshipProperties = GetRelationshipProperties();
                nextIdentifier = DataManager.ScriptManager.GetNextIdentifier(dataObjectSchema.ObjectType);
            }
        }

        private List<FieldInfo> GetDataFields()
        {
            Type type = dataObjectSchema.ObjectType;
            Dictionary<string, FieldInfo> foundFields = new Dictionary<string, FieldInfo>();

            while (type.GetInterface("DataRelational.IDataObject", false) != null)
            {
                foreach (FieldInfo fld in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (fld.GetCustomAttributes(typeof(DataFieldAttribute), false).Length > 0)
                    {
                        if (ScriptCommon.PrimativeTypes.Contains(fld.FieldType) || fld.FieldType.IsEnum)
                        {
                            if (!foundFields.ContainsKey(fld.Name))
                                foundFields[fld.Name] = fld;
                        }
                        else if (ScriptCommon.GetArrayType(fld.FieldType) != null && (ScriptCommon.PrimativeTypes.Contains(ScriptCommon.GetArrayType(fld.FieldType)) || ScriptCommon.GetArrayType(fld.FieldType).IsEnum))
                        {
                            if (!foundFields.ContainsKey(fld.Name))
                                foundFields[fld.Name] = fld;
                        }
                        else if (fld.FieldType.IsGenericType && fld.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            Type[] genericArgs = fld.FieldType.GetGenericArguments();
                            if (genericArgs.Length > 0 && ScriptCommon.PrimativeTypes.Contains(genericArgs[0]))
                            {
                                if (!foundFields.ContainsKey(fld.Name))
                                    foundFields[fld.Name] = fld;
                            }
                        }
                        else if (fld.FieldType.IsGenericType && fld.FieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                        {
                            if ((ScriptCommon.PrimativeTypes.Contains(fld.FieldType.GetGenericArguments()[0]) || fld.FieldType.GetGenericArguments()[0].IsEnum) &&
                            (ScriptCommon.PrimativeTypes.Contains(fld.FieldType.GetGenericArguments()[1]) || fld.FieldType.GetGenericArguments()[1].IsEnum))
                            {
                                if (!foundFields.ContainsKey(fld.Name))
                                    foundFields[fld.Name] = fld;
                            }
                        }
                    }
                }
                type = type.BaseType;
            }
            return new List<FieldInfo>(foundFields.Values);
        }

        private List<PropertyInfo> GetDataProperties()
        {
            Type type = dataObjectSchema.ObjectType;
            Dictionary<string, PropertyInfo> foundProperties = new Dictionary<string, PropertyInfo>();

            while (type.GetInterface("DataRelational.IDataObject", false) != null)
            {
                foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (prop.GetCustomAttributes(typeof(DataFieldAttribute), false).Length > 0)
                    {
                        if (ScriptCommon.PrimativeTypes.Contains(prop.PropertyType) || prop.PropertyType.IsEnum)
                        {
                            if (!foundProperties.ContainsKey(prop.Name))
                                foundProperties[prop.Name] = prop;
                        }
                        else if (ScriptCommon.GetArrayType(prop.PropertyType) != null && (ScriptCommon.PrimativeTypes.Contains(ScriptCommon.GetArrayType(prop.PropertyType)) || ScriptCommon.GetArrayType(prop.PropertyType).IsEnum))
                        {
                            if (!foundProperties.ContainsKey(prop.Name))
                                foundProperties[prop.Name] = prop;
                        }
                        else if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            Type[] genericArgs = prop.PropertyType.GetGenericArguments();
                            if (genericArgs.Length > 0 && ScriptCommon.PrimativeTypes.Contains(genericArgs[0]))
                            {
                                if (!foundProperties.ContainsKey(prop.Name))
                                    foundProperties[prop.Name] = prop;
                            }
                        }
                        else if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                        {
                            if ((ScriptCommon.PrimativeTypes.Contains(prop.PropertyType.GetGenericArguments()[0]) || prop.PropertyType.GetGenericArguments()[0].IsEnum) &&
                            (ScriptCommon.PrimativeTypes.Contains(prop.PropertyType.GetGenericArguments()[1]) || prop.PropertyType.GetGenericArguments()[1].IsEnum))
                            {
                                if (!foundProperties.ContainsKey(prop.Name))
                                    foundProperties[prop.Name] = prop;
                            }
                        }
                    }
                }
                type = type.BaseType;
            }
            return new List<PropertyInfo>(foundProperties.Values);
        }

        private List<FieldInfo> GetRelationshipFields()
        {
            Type type = dataObjectSchema.ObjectType;
            Dictionary<string, FieldInfo> foundFields = new Dictionary<string, FieldInfo>();

            while (type.GetInterface("DataRelational.IDataObject", false) != null)
            {
                foreach (FieldInfo fld in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (fld.FieldType.IsGenericType && fld.FieldType.GetGenericTypeDefinition() == typeof(DataRelationship<>))
                    {
                        Type underlyingType = DataRelationship.GetUnderlyingType(fld.FieldType);
                        if (underlyingType.GetInterface("DataRelational.IDataObject", false) != null)
                        {
                            if (!foundFields.ContainsKey(fld.Name))
                                foundFields[fld.Name] = fld;
                        }
                        else if (ScriptCommon.GetArrayType(underlyingType) != null && ScriptCommon.GetArrayType(underlyingType).GetInterface("DataRelational.IDataObject", false) != null)
                        {
                            if (!foundFields.ContainsKey(fld.Name))
                                foundFields[fld.Name] = fld;
                        }
                        else if (underlyingType.IsGenericType && underlyingType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            if (!foundFields.ContainsKey(fld.Name))
                                foundFields[fld.Name] = fld;
                        }
                        else if (underlyingType.IsGenericType && underlyingType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                        {
                            Type[] genericArgs = underlyingType.GetGenericArguments();
                            if (genericArgs[0].GetInterface("DataRelational.IDataObject", false) != null ||
                                genericArgs[1].GetInterface("DataRelational.IDataObject", false) != null)
                            {
                                if (genericArgs[0].GetInterface("DataRelational.IDataObject", false) != null &&
                                genericArgs[1].GetInterface("DataRelational.IDataObject", false) != null)
                                {
                                    if (!foundFields.ContainsKey(fld.Name))
                                        foundFields[fld.Name] = fld;
                                }
                                else if (ScriptCommon.PrimativeTypes.Contains(genericArgs[0]) || ScriptCommon.PrimativeTypes.Contains(genericArgs[1]))
                                {
                                    if (!foundFields.ContainsKey(fld.Name))
                                        foundFields[fld.Name] = fld;
                                }
                            }
                        }
                    }
                }
                type = type.BaseType;
            }
            return new List<FieldInfo>(foundFields.Values);
        }

        private List<PropertyInfo> GetRelationshipProperties()
        {
            Type type = dataObjectSchema.ObjectType;
            Dictionary<string, PropertyInfo> foundProperties = new Dictionary<string, PropertyInfo>();

            while (type.GetInterface("DataRelational.IDataObject", false) != null)
            {
                foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if(prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(DataRelationship<>))
                    {
                        Type underlyingType = DataRelationship.GetUnderlyingType(prop.PropertyType);
                        if (underlyingType.GetInterface("DataRelational.IDataObject", false) != null)
                        {
                            if (!foundProperties.ContainsKey(prop.Name))
                                foundProperties[prop.Name] = prop;
                        }
                        else if (ScriptCommon.GetArrayType(underlyingType) != null && ScriptCommon.GetArrayType(underlyingType).GetInterface("DataRelational.IDataObject", false) != null)
                        {
                            if (!foundProperties.ContainsKey(prop.Name))
                                foundProperties[prop.Name] = prop;
                        }
                        else if (underlyingType.IsGenericType && underlyingType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            if (!foundProperties.ContainsKey(prop.Name))
                                foundProperties[prop.Name] = prop;
                        }
                        else if (underlyingType.IsGenericType && underlyingType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                        {
                            Type[] genericArgs = underlyingType.GetGenericArguments();
                            if (genericArgs[0].GetInterface("DataRelational.IDataObject", false) != null ||
                                genericArgs[1].GetInterface("DataRelational.IDataObject", false) != null)
                            {
                                if (genericArgs[0].GetInterface("DataRelational.IDataObject", false) != null &&
                                genericArgs[1].GetInterface("DataRelational.IDataObject", false) != null)
                                {
                                    if (!foundProperties.ContainsKey(prop.Name))
                                        foundProperties[prop.Name] = prop;
                                }
                                else if (ScriptCommon.PrimativeTypes.Contains(genericArgs[0]) || ScriptCommon.PrimativeTypes.Contains(genericArgs[1]))
                                {
                                    if (!foundProperties.ContainsKey(prop.Name))
                                        foundProperties[prop.Name] = prop;
                                }
                            }
                        }
                    }
                }
                type = type.BaseType;
            }

            return new List<PropertyInfo>(foundProperties.Values);
        }

        #endregion Methods
    }
}
