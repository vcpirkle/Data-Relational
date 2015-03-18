using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Reflection;
using DataRelational.Attributes;
using DataRelational.Script;
using System.Runtime.CompilerServices;

namespace DataRelational.Schema
{
    class DataObjectSchemaFactory
    {
        /// <summary>
        /// Retrieves the data object schema from the database for an object type
        /// </summary>
        /// <param name="connection">The connection to the database</param>
        /// <param name="objectType">The object type to retrieve the schema for</param>
        /// <returns>The data object schema for the object type if it exists, otherwise null</returns>
        public DataObjectSchema GetSchema(DbConnection connection, Type objectType)
        {
            DataObjectSchema dataObjectSchema = null;
            Logging.Debug(string.Format("Attempting to get the data object schema for the type {0} from the database {1}.", objectType.Namespace + "." + objectType.Name, connection.Database));

            StringBuilder sqlTxt = new StringBuilder();
            sqlTxt.AppendLine("DECLARE @DataObjectTypeId smallint");
            sqlTxt.AppendLine(string.Format("SELECT @DataObjectTypeId = _Id FROM DataObjectType WHERE TypeName = '{0}'", objectType.Namespace + "." + objectType.Name));
            sqlTxt.AppendLine("");
            sqlTxt.AppendLine("SELECT DOT.[_Id]");
            sqlTxt.AppendLine("      ,DOT.[TypeName]");
            sqlTxt.AppendLine("      ,DOT.[TableName]");
            sqlTxt.AppendLine("      ,DOF.[_Id] [FieldId]");
            sqlTxt.AppendLine("      ,DOF.[FieldName]");
            sqlTxt.AppendLine("      ,DOF.[_DataObjectFieldTypeId]");
            sqlTxt.AppendLine("      ,DOF.[FieldLength]");
            sqlTxt.AppendLine("      ,DOF.[Unicode] [FieldUnicode]");
            sqlTxt.AppendLine("  FROM [DataObjectType] DOT");
            sqlTxt.AppendLine("  JOIN [DataObjectField] DOF");
            sqlTxt.AppendLine("    ON DOT._Id = DOF._DataObjectTypeId");
            sqlTxt.AppendLine(" WHERE DOT.[_Id] = @DataObjectTypeId");
            sqlTxt.AppendLine(" ORDER BY DOF.[_Id]");
            sqlTxt.AppendLine("");
            sqlTxt.AppendLine("SELECT [_Id]");
            sqlTxt.AppendLine("      ,[_DataObjectTypeId]");
            sqlTxt.AppendLine("      ,[_DataObjectFieldId]");
            sqlTxt.AppendLine("      ,[IndexName]");
            sqlTxt.AppendLine("      ,[Ascending]");
            sqlTxt.AppendLine("  FROM [DataObjectIndex]");
            sqlTxt.AppendLine(" WHERE [_DataObjectTypeId] = @DataObjectTypeId");
            sqlTxt.AppendLine(" ORDER BY [_Id], [_DataObjectFieldId]");

            DateTime start = DateTime.Now;
            using (DbCommand cmd = connection.CreateCommand())
            {
                DateTime end = DateTime.Now;
                TimeSpan ts = (end - start);
                Logging.Information(string.Format("Database read time - {0} seconds {1} milliseconds", ts.Seconds, ts.Milliseconds));

                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = sqlTxt.ToString();

                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //Load the data object fields
                        if (dataObjectSchema == null)
                            dataObjectSchema = new DataObjectSchema(reader.GetInt16(0), objectType, reader.GetString(1), reader.GetString(2));
                        DataObjectSchemaFieldType fieldType = (DataObjectSchemaFieldType)Enum.ToObject(typeof(DataObjectSchemaFieldType), reader.GetByte(5));
                        DataObjectSchemaField field = new DataObjectSchemaField(reader.GetInt32(3), reader.GetString(4), fieldType, reader.GetInt32(6), reader.GetBoolean(7));
                        dataObjectSchema.Fields.Add(field);
                    }
                    if (reader.NextResult())
                    {
                        //Load the data object indexes
                        while (reader.Read())
                        {
                            DataObjectSchemaIndex dataObjectSchemaIndex = dataObjectSchema.FindIndex(reader.GetInt32(0));
                            if (dataObjectSchemaIndex == null)
                            {
                                dataObjectSchemaIndex = new DataObjectSchemaIndex(reader.GetInt32(0), reader.GetString(3));
                                dataObjectSchema.Indexes.Add(dataObjectSchemaIndex);
                            }
                            DataObjectSchemaFieldIndex dataObjectSchemaFieldIndex = new DataObjectSchemaFieldIndex();
                            dataObjectSchemaFieldIndex.Field = dataObjectSchema.FindField(reader.GetInt32(2));
                            dataObjectSchemaFieldIndex.Ascending = reader.GetBoolean(4);
                            dataObjectSchemaIndex.Fields.Add(dataObjectSchemaFieldIndex);
                        }
                    }
                    reader.Close();
                }
            }

            if(dataObjectSchema == null)
                Logging.Debug(string.Format("The data object schema for the type {0} was not found in the database {1}.", objectType.Namespace + "." + objectType.Name, connection.Database));
            else
                Logging.Debug(string.Format("Found data object schema for the type {0} in the database {1}.", objectType.Namespace + "." + objectType.Name, connection.Database));

            return dataObjectSchema;
        }

        /// <summary>
        /// Retrieves the data object schema from the database for an object type
        /// </summary>
        /// <param name="connection">The connection to the database</param>
        /// <param name="objectTypeId">The object type to retrieve the schema for</param>
        /// <returns>The data object schema for the object type if it exists, otherwise null</returns>
        public DataObjectSchema GetSchema(DbConnection connection, short objectTypeId)
        {
            DataObjectSchema dataObjectSchema = null;
            Logging.Debug(string.Format("Attempting to get the data object schema for the type {0} from the database {1}.", objectTypeId, connection.Database));

            StringBuilder sqlTxt = new StringBuilder();
            sqlTxt.AppendLine("DECLARE @DataObjectTypeId smallint");
            sqlTxt.AppendLine(string.Format("SELECT @DataObjectTypeId = _Id FROM DataObjectType WHERE _Id = {0}", objectTypeId));
            sqlTxt.AppendLine("");
            sqlTxt.AppendLine("SELECT DOT.[_Id]");
            sqlTxt.AppendLine("      ,DOT.[TypeName]");
            sqlTxt.AppendLine("      ,DOT.[TableName]");
            sqlTxt.AppendLine("      ,DOF.[_Id] [FieldId]");
            sqlTxt.AppendLine("      ,DOF.[FieldName]");
            sqlTxt.AppendLine("      ,DOF.[_DataObjectFieldTypeId]");
            sqlTxt.AppendLine("      ,DOF.[FieldLength]");
            sqlTxt.AppendLine("      ,DOF.[Unicode] [FieldUnicode]");
            sqlTxt.AppendLine("  FROM [DataObjectType] DOT");
            sqlTxt.AppendLine("  JOIN [DataObjectField] DOF");
            sqlTxt.AppendLine("    ON DOT._Id = DOF._DataObjectTypeId");
            sqlTxt.AppendLine(" WHERE DOT.[_Id] = @DataObjectTypeId");
            sqlTxt.AppendLine(" ORDER BY DOF.[_Id]");
            sqlTxt.AppendLine("");
            sqlTxt.AppendLine("SELECT [_Id]");
            sqlTxt.AppendLine("      ,[_DataObjectTypeId]");
            sqlTxt.AppendLine("      ,[_DataObjectFieldId]");
            sqlTxt.AppendLine("      ,[IndexName]");
            sqlTxt.AppendLine("      ,[Ascending]");
            sqlTxt.AppendLine("  FROM [DataObjectIndex]");
            sqlTxt.AppendLine(" WHERE [_DataObjectTypeId] = @DataObjectTypeId");
            sqlTxt.AppendLine(" ORDER BY [_Id], [_DataObjectFieldId]");

            DateTime start = DateTime.Now;
            using (DbCommand cmd = connection.CreateCommand())
            {
                DateTime end = DateTime.Now;
                TimeSpan ts = (end - start);
                Logging.Information(string.Format("Database read time - {0} seconds {1} milliseconds", ts.Seconds, ts.Milliseconds));

                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = sqlTxt.ToString();

                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    Type objectType = null;
                    while (reader.Read())
                    {
                        if (objectType == null)
                        {
                            objectType = ScriptCommon.GetType(reader.GetString(1));
                        }

                        //Load the data object fields
                        if (dataObjectSchema == null)
                            dataObjectSchema = new DataObjectSchema(reader.GetInt16(0), objectType, reader.GetString(1), reader.GetString(2));
                        DataObjectSchemaFieldType fieldType = (DataObjectSchemaFieldType)Enum.ToObject(typeof(DataObjectSchemaFieldType), reader.GetByte(5));
                        DataObjectSchemaField field = new DataObjectSchemaField(reader.GetInt32(3), reader.GetString(4), fieldType, reader.GetInt32(6), reader.GetBoolean(7));
                        dataObjectSchema.Fields.Add(field);
                    }
                    if (reader.NextResult())
                    {
                        //Load the data object indexes
                        while (reader.Read())
                        {
                            DataObjectSchemaIndex dataObjectSchemaIndex = dataObjectSchema.FindIndex(reader.GetInt32(0));
                            if (dataObjectSchemaIndex == null)
                            {
                                dataObjectSchemaIndex = new DataObjectSchemaIndex(reader.GetInt32(0), reader.GetString(3));
                                dataObjectSchema.Indexes.Add(dataObjectSchemaIndex);
                            }
                            DataObjectSchemaFieldIndex dataObjectSchemaFieldIndex = new DataObjectSchemaFieldIndex();
                            dataObjectSchemaFieldIndex.Field = dataObjectSchema.FindField(reader.GetInt32(2));
                            dataObjectSchemaFieldIndex.Ascending = reader.GetBoolean(4);
                            dataObjectSchemaIndex.Fields.Add(dataObjectSchemaFieldIndex);
                        }
                    }
                    reader.Close();
                }
            }

            if (dataObjectSchema == null)
                Logging.Debug(string.Format("The data object schema for the type {0} was not found in the database {1}.", objectTypeId, connection.Database));
            else
                Logging.Debug(string.Format("Found data object schema for the type {0} in the database {1}.", objectTypeId, connection.Database));

            return dataObjectSchema;
        }

        /// <summary>
        /// Retrieves the data object schema from meta data for an object type
        /// </summary>
        /// <param name="objectType">The object type to retrieve the schema for</param>
        /// <returns>The data object schema for the object type</returns>
        public DataObjectSchema GetSchema(Type objectType)
        {
            Logging.Debug(string.Format("Attempting to get the data object schema for the type {0} from metadata.", objectType.Namespace + "." + objectType.Name));
            DataObjectSchema dataObjectSchema = new DataObjectSchema(-1, objectType, objectType.Namespace + "." + objectType.Name, objectType.Name);
            int dataObjectFieldId = -1;
            int dataObjectIndexId = -1;

            Type type = objectType;
            List<string> foundFields = new List<string>();
            while (type.GetInterface("DataRelational.IDataObject", false) != null)
            {
                foreach (FieldInfo fld in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    object[] attributes = fld.GetCustomAttributes(typeof(DataFieldAttribute), false);
                    DataFieldAttribute attribute = attributes.Length > 0 ? attributes[0] as DataFieldAttribute : null;
                    if (attribute != null)
                    {
                        if (ScriptCommon.PrimativeTypes.Contains(fld.FieldType) || fld.FieldType.IsEnum)
                        {
                            if (foundFields.Contains(fld.Name) == false)
                            {
                                DataObjectSchemaField field = new DataObjectSchemaField(dataObjectFieldId--, fld.Name, GetFieldType(fld.FieldType), attribute.FieldSize, attribute.Unicode);
                                dataObjectSchema.Fields.Add(field);

                                object[] indexAttributes = fld.GetCustomAttributes(typeof(DataFieldIndexAttribute), false);
                                foreach (DataFieldIndexAttribute indexAttribute in indexAttributes)
                                {
                                    DataObjectSchemaIndex index = dataObjectSchema.FindIndex(indexAttribute.Name);
                                    if (index == null)
                                    {
                                        index = new DataObjectSchemaIndex(dataObjectIndexId--, indexAttribute.Name);
                                        dataObjectSchema.Indexes.Add(index);
                                    }
                                    DataObjectSchemaFieldIndex fieldIndex = new DataObjectSchemaFieldIndex();
                                    fieldIndex.Field = field;
                                    fieldIndex.Ascending = indexAttribute.Ascending;
                                    index.Fields.Add(fieldIndex);
                                }
                                foundFields.Add(fld.Name);
                            }
                        }
                        else if (ScriptCommon.GetArrayType(fld.FieldType) != null && (ScriptCommon.PrimativeTypes.Contains(ScriptCommon.GetArrayType(fld.FieldType)) || ScriptCommon.GetArrayType(fld.FieldType).IsEnum))
                        {
                            if (foundFields.Contains(fld.Name) == false)
                            {
                                DataObjectSchemaField field = new DataObjectSchemaField(dataObjectFieldId--, fld.Name, DataObjectSchemaFieldType.PrimativeArray, attribute.FieldSize, attribute.Unicode);
                                dataObjectSchema.Fields.Add(field);
                                foundFields.Add(fld.Name);
                            }
                        }
                        else if (fld.FieldType.IsGenericType && fld.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            if (foundFields.Contains(fld.Name) == false)
                            {
                                if (ScriptCommon.PrimativeTypes.Contains(fld.FieldType.GetGenericArguments()[0]) || fld.FieldType.GetGenericArguments()[0].IsEnum)
                                {
                                    DataObjectSchemaField field = new DataObjectSchemaField(dataObjectFieldId--, fld.Name, DataObjectSchemaFieldType.PrimativeList, attribute.FieldSize, attribute.Unicode);
                                    dataObjectSchema.Fields.Add(field);
                                }
                                else
                                    throw new InvalidObjectSchemaException(string.Format("The field {0} cannot be tagged with the DataField attribute.", fld.Name));
                                foundFields.Add(fld.Name);
                            }
                        }
                        else if (fld.FieldType.IsGenericType && fld.FieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                        {
                            if (foundFields.Contains(fld.Name) == false)
                            {
                                if ((ScriptCommon.PrimativeTypes.Contains(fld.FieldType.GetGenericArguments()[0]) || fld.FieldType.GetGenericArguments()[0].IsEnum) &&
                                (ScriptCommon.PrimativeTypes.Contains(fld.FieldType.GetGenericArguments()[1]) || fld.FieldType.GetGenericArguments()[1].IsEnum))
                                {
                                    DataObjectSchemaField field = new DataObjectSchemaField(dataObjectFieldId--, fld.Name, DataObjectSchemaFieldType.PrimativeDictionary, attribute.FieldSize, attribute.Unicode);
                                    dataObjectSchema.Fields.Add(field);
                                }
                                else
                                    throw new InvalidObjectSchemaException(string.Format("The field {0} cannot be tagged with the DataField attribute.", fld.Name));
                                foundFields.Add(fld.Name);
                            }
                        }
                        else
                        {
                            throw new InvalidObjectSchemaException(string.Format("The field {0} cannot be tagged with the DataField attribute.", fld.Name));
                        }
                    }

                    if (fld.FieldType.IsGenericType && fld.FieldType.GetGenericTypeDefinition() == typeof(DataRelationship<>))
                    {
                        if (fld.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Length == 0)
                        {
                            Type underlyingType = DataRelationship.GetUnderlyingType(fld.FieldType);
                            if (underlyingType.GetInterface("DataRelational.IDataObject", false) != null)
                            {
                                if (foundFields.Contains(fld.Name) == false)
                                {
                                    DataObjectSchemaField field = new DataObjectSchemaField(dataObjectFieldId--, fld.Name, DataObjectSchemaFieldType.IDataObject, 0, false);
                                    dataObjectSchema.Fields.Add(field);
                                    foundFields.Add(fld.Name);
                                }
                            }
                            else if (ScriptCommon.GetArrayType(underlyingType) != null && ScriptCommon.GetArrayType(underlyingType).GetInterface("DataRelational.IDataObject", false) != null)
                            {
                                if (foundFields.Contains(fld.Name) == false)
                                {
                                    DataObjectSchemaField field = new DataObjectSchemaField(dataObjectFieldId--, fld.Name, DataObjectSchemaFieldType.IDataObjectArray, 0, false);
                                    dataObjectSchema.Fields.Add(field);
                                    foundFields.Add(fld.Name);
                                }
                            }
                            else if (underlyingType.IsGenericType && underlyingType.GetGenericTypeDefinition() == typeof(List<>))
                            {
                                if (underlyingType.GetGenericArguments()[0].GetInterface("DataRelational.IDataObject", false) != null)
                                {
                                    if (foundFields.Contains(fld.Name) == false)
                                    {
                                        DataObjectSchemaField field = new DataObjectSchemaField(dataObjectFieldId--, fld.Name, DataObjectSchemaFieldType.IDataObjectList, 0, false);
                                        dataObjectSchema.Fields.Add(field);
                                        foundFields.Add(fld.Name);
                                    }
                                }
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
                                        if (foundFields.Contains(fld.Name) == false)
                                        {
                                            DataObjectSchemaField field = new DataObjectSchemaField(dataObjectFieldId--, fld.Name, DataObjectSchemaFieldType.IDataObjectDictionary, 0, false);
                                            dataObjectSchema.Fields.Add(field);
                                            foundFields.Add(fld.Name);
                                        }
                                    }
                                    else if (ScriptCommon.PrimativeTypes.Contains(genericArgs[0]) || ScriptCommon.PrimativeTypes.Contains(genericArgs[1]))
                                    {
                                        if (foundFields.Contains(fld.Name) == false)
                                        {
                                            DataObjectSchemaField field = new DataObjectSchemaField(dataObjectFieldId--, fld.Name, DataObjectSchemaFieldType.IDataObjectDictionary, 0, false);
                                            dataObjectSchema.Fields.Add(field);
                                            foundFields.Add(fld.Name);
                                        }
                                    }
                                    else
                                    {
                                        throw new InvalidObjectSchemaException(string.Format("The field {0} cannot be tagged with the DataRelationship attribute.", fld.Name));
                                    }
                                }
                                else
                                {
                                    throw new InvalidObjectSchemaException(string.Format("The field {0} cannot be tagged with the DataRelationship attribute.", fld.Name));
                                }
                            }
                            else
                                throw new InvalidObjectSchemaException(string.Format("The field {0} cannot be tagged with the DataRelationship attribute.", fld.Name));
                        }
                    }
                }

                foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    object[] attributes = prop.GetCustomAttributes(typeof(DataFieldAttribute), false);
                    DataFieldAttribute attribute = attributes.Length > 0 ? attributes[0] as DataFieldAttribute : null;
                    if (attribute != null)
                    {
                        if (ScriptCommon.PrimativeTypes.Contains(prop.PropertyType) || prop.PropertyType.IsEnum)
                        {
                            if (foundFields.Contains(prop.Name) == false)
                            {
                                DataObjectSchemaField field = new DataObjectSchemaField(dataObjectFieldId--, prop.Name, GetFieldType(prop.PropertyType), attribute.FieldSize, attribute.Unicode);
                                dataObjectSchema.Fields.Add(field);

                                object[] indexAttributes = prop.GetCustomAttributes(typeof(DataFieldIndexAttribute), false);
                                foreach (DataFieldIndexAttribute indexAttribute in indexAttributes)
                                {
                                    DataObjectSchemaIndex index = dataObjectSchema.FindIndex(indexAttribute.Name);
                                    if (index == null)
                                    {
                                        index = new DataObjectSchemaIndex(dataObjectIndexId--, indexAttribute.Name);
                                        dataObjectSchema.Indexes.Add(index);
                                    }
                                    DataObjectSchemaFieldIndex fieldIndex = new DataObjectSchemaFieldIndex();
                                    fieldIndex.Field = field;
                                    fieldIndex.Ascending = indexAttribute.Ascending;
                                    index.Fields.Add(fieldIndex);
                                }
                                foundFields.Add(prop.Name);
                            }
                        }
                        else if (ScriptCommon.GetArrayType(prop.PropertyType) != null && (ScriptCommon.PrimativeTypes.Contains(ScriptCommon.GetArrayType(prop.PropertyType)) || ScriptCommon.GetArrayType(prop.PropertyType).IsEnum))
                        {
                            if (foundFields.Contains(prop.Name) == false)
                            {
                                DataObjectSchemaField field = new DataObjectSchemaField(dataObjectFieldId--, prop.Name, DataObjectSchemaFieldType.PrimativeArray, attribute.FieldSize, attribute.Unicode);
                                dataObjectSchema.Fields.Add(field);
                                foundFields.Add(prop.Name);
                            }
                        }
                        else if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            if (ScriptCommon.PrimativeTypes.Contains(prop.PropertyType.GetGenericArguments()[0]) || prop.PropertyType.GetGenericArguments()[0].IsEnum)
                            {
                                if (foundFields.Contains(prop.Name) == false)
                                {
                                    DataObjectSchemaField field = new DataObjectSchemaField(dataObjectFieldId--, prop.Name, DataObjectSchemaFieldType.PrimativeList, attribute.FieldSize, attribute.Unicode);
                                    dataObjectSchema.Fields.Add(field);
                                    foundFields.Add(prop.Name);
                                }
                            }
                            else
                                throw new InvalidObjectSchemaException(string.Format("The property {0} cannot be tagged with the DataField attribute.", prop.Name));
                        }
                        else if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                        {
                            if ((ScriptCommon.PrimativeTypes.Contains(prop.PropertyType.GetGenericArguments()[0]) || prop.PropertyType.GetGenericArguments()[0].IsEnum) &&
                            (ScriptCommon.PrimativeTypes.Contains(prop.PropertyType.GetGenericArguments()[1]) || prop.PropertyType.GetGenericArguments()[1].IsEnum))
                            {
                                if (foundFields.Contains(prop.Name) == false)
                                {
                                    DataObjectSchemaField field = new DataObjectSchemaField(dataObjectFieldId--, prop.Name, DataObjectSchemaFieldType.PrimativeDictionary, attribute.FieldSize, attribute.Unicode);
                                    dataObjectSchema.Fields.Add(field);
                                    foundFields.Add(prop.Name);
                                }
                            }
                            else
                                throw new InvalidObjectSchemaException(string.Format("The property {0} cannot be tagged with the DataField attribute.", prop.Name));
                        }
                        else
                        {
                            throw new InvalidObjectSchemaException(string.Format("The property {0} cannot be tagged with the DataField attribute.", prop.Name));
                        }
                    }

                    if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(DataRelationship<>))
                    {
                        Type underlyingType = DataRelationship.GetUnderlyingType(prop.PropertyType);
                        if (underlyingType.GetInterface("DataRelational.IDataObject", false) != null)
                        {
                            if (foundFields.Contains(prop.Name) == false)
                            {
                                DataObjectSchemaField field = new DataObjectSchemaField(dataObjectFieldId--, prop.Name, DataObjectSchemaFieldType.IDataObject, 0, false);
                                dataObjectSchema.Fields.Add(field);
                                foundFields.Add(prop.Name);
                            }
                        }
                        else if (ScriptCommon.GetArrayType(underlyingType) != null && ScriptCommon.GetArrayType(underlyingType).GetInterface("DataRelational.IDataObject", false) != null)
                        {
                            if (foundFields.Contains(prop.Name) == false)
                            {
                                DataObjectSchemaField field = new DataObjectSchemaField(dataObjectFieldId--, prop.Name, DataObjectSchemaFieldType.IDataObjectArray, 0, false);
                                dataObjectSchema.Fields.Add(field);
                                foundFields.Add(prop.Name);
                            }
                        }
                        else if (underlyingType.IsGenericType && underlyingType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            if (underlyingType.GetGenericArguments()[0].GetInterface("DataRelational.IDataObject", false) != null)
                            {
                                if (foundFields.Contains(prop.Name) == false)
                                {
                                    DataObjectSchemaField field = new DataObjectSchemaField(dataObjectFieldId--, prop.Name, DataObjectSchemaFieldType.IDataObjectList, 0, false);
                                    dataObjectSchema.Fields.Add(field);
                                    foundFields.Add(prop.Name);
                                }
                            }
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
                                    if (foundFields.Contains(prop.Name) == false)
                                    {
                                        DataObjectSchemaField field = new DataObjectSchemaField(dataObjectFieldId--, prop.Name, DataObjectSchemaFieldType.IDataObjectDictionary, 0, false);
                                        dataObjectSchema.Fields.Add(field);
                                        foundFields.Add(prop.Name);
                                    }
                                }
                                else if (ScriptCommon.PrimativeTypes.Contains(genericArgs[0]) || ScriptCommon.PrimativeTypes.Contains(genericArgs[1]))
                                {
                                    if (foundFields.Contains(prop.Name) == false)
                                    {
                                        DataObjectSchemaField field = new DataObjectSchemaField(dataObjectFieldId--, prop.Name, DataObjectSchemaFieldType.IDataObjectDictionary, 0, false);
                                        dataObjectSchema.Fields.Add(field);
                                        foundFields.Add(prop.Name);
                                    }
                                }
                                else
                                {
                                    throw new InvalidObjectSchemaException(string.Format("The property {0} cannot be tagged with the DataRelationship attribute.", prop.Name));
                                }
                            }
                            else
                            {
                                throw new InvalidObjectSchemaException(string.Format("The property {0} cannot be tagged with the DataRelationship attribute.", prop.Name));
                            }
                        }
                        else
                            throw new InvalidObjectSchemaException(string.Format("The property {0} cannot be tagged with the DataRelationship attribute.", prop.Name));
                    }
                }
                type = type.BaseType;
            }

            return dataObjectSchema;
        }

        /// <summary>
        /// Adds a new data object schema to the database and creates the necessary tables for that schema
        /// </summary>
        /// <param name="connection">The connection to the database</param>
        /// <param name="schema">The data object schema to add</param>
        public DataObjectSchema AddSchema(DbConnection connection, DataObjectSchema schema)
        {
            Logging.Debug(string.Format("Attempting to add the data object schema for the type {0} to the database {1}.", schema.ObjectType.Namespace + "." + schema.ObjectType.Name, connection.Database));
            CreateTable(connection, schema.ObjectType, schema);
            return (DataObjectSchema)CreateDataObjectSchema(connection, schema);
        }

        /// <summary>
        /// Modifies an existing data object schema in the database and modifies the data tables for that schema
        /// </summary>
        /// <param name="connection">The connection to the database</param>
        /// <param name="objectType">The object type to modify the schema for</param>
        /// <param name="schema">The data object schema to modify</param>
        public DataObjectSchema ModifySchema(DbConnection connection, Type objectType, DataObjectSchema schema)
        {
            return ModifySchema(connection, objectType, schema, GetSchema(connection, objectType));
        }

        /// <summary>
        /// Modifies an existing data object schema in the database and modifies the data tables for that schema
        /// </summary>
        /// <param name="connection">The connection to the database</param>
        /// <param name="objectType">The object type to modify the schema for</param>
        /// <param name="schema">The data object schema to modify</param>
        /// <param name="schemaDb">The data object schem currently stored in the database</param>
        public DataObjectSchema ModifySchema(DbConnection connection, Type objectType, DataObjectSchema schema, DataObjectSchema schemaDb)
        {
            Logging.Debug(string.Format("Attempting to modify the data object schema for the type {0} in the database {1}.", objectType.Namespace + "." + objectType.Name, connection.Database));
            AlterTable(connection, objectType, schemaDb, schema);
            return AlterDataObjectSchema(connection, schemaDb, schema);
        }

        private DataObjectSchemaFieldType GetFieldType(Type fieldType)
        {
            byte index = 0;
            foreach (Type primativeType in ScriptCommon.PrimativeTypes)
            {
                if (fieldType.IsEnum)
                {
                    if (primativeType == Enum.GetUnderlyingType(fieldType))
                        return (DataObjectSchemaFieldType)Enum.ToObject(typeof(DataObjectSchemaFieldType), index);
                }
                else
                {
                    if (primativeType == fieldType)
                        return (DataObjectSchemaFieldType)index;
                }
                index++;
            }
            throw new Exception("Could not get data object schema field type for the type " + fieldType.Namespace + "." + fieldType.Name + ".");
        }

        private DataObjectSchema CreateDataObjectSchema(DbConnection connection, DataObjectSchema objectSchema)
        {
            StringBuilder sqlTxt = new StringBuilder();
            sqlTxt.AppendLine("DECLARE @DataObjectTypeId INT");
            sqlTxt.AppendLine("DECLARE @DataObjectFieldId INT");
            sqlTxt.AppendLine("DECLARE @DataObjectIndexId INT");
            sqlTxt.AppendLine();
            sqlTxt.AppendLine(string.Format("INSERT INTO [DataObjectType](TypeName, TableName) Values('{0}', '{1}') ", objectSchema.TypeName, objectSchema.TableName));
            sqlTxt.AppendLine("SELECT @DataObjectTypeId = @@Identity");

            foreach (DataObjectSchemaField field in objectSchema.Fields)
            {
                sqlTxt.Append("INSERT INTO [DataObjectField](_DataObjectTypeId, FieldName, _DataObjectFieldTypeId, FieldLength, Unicode) ");
                sqlTxt.AppendLine(string.Format("VALUES (@DataObjectTypeId, '{0}', {1}, {2}, {3})", field.Name, (byte)field.FieldType, field.Length, (field.Unicode ? 1 : 0)));
            }

            foreach (DataObjectSchemaIndex index in objectSchema.Indexes)
            {
                sqlTxt.AppendLine("SELECT @DataObjectIndexId = Max(_Id) + 1 FROM DataObjectIndex");
                sqlTxt.AppendLine("IF @DataObjectIndexId IS NULL");
                sqlTxt.AppendLine("   SET @DataObjectIndexId = 1");

                foreach (DataObjectSchemaFieldIndex indexField in index.Fields)
                {
                    sqlTxt.AppendLine(string.Format("SELECT @DataObjectFieldId = _Id FROM DataObjectField WHERE _DataObjectTypeId = @DataObjectTypeId AND FieldName = '{0}' AND _DataObjectFieldTypeId = {1}",
                        indexField.Field.Name,
                        (byte)indexField.Field.FieldType));
                    sqlTxt.AppendLine("INSERT INTO DataObjectIndex ([_Id],[_DataObjectTypeId],[_DataObjectFieldId],[IndexName],[Ascending])");
                    sqlTxt.AppendLine(string.Format("VALUES (@DataObjectIndexId, @DataObjectTypeId, @DataObjectFieldId, '{0}', {1})", index.Name, (indexField.Ascending ? 1 : 0)));
                }
            }

            ScriptCommon.ExecuteCommand(connection, sqlTxt.ToString());
            return GetSchema(connection, objectSchema.ObjectType);
        }

        private DataObjectSchema AlterDataObjectSchema(DbConnection connection, DataObjectSchema dataObjectSchemaDatabase, DataObjectSchema dataObjectSchemaMetaData)
        {
            List<DataObjectSchemaField> newFields = new List<DataObjectSchemaField>();
            List<DataObjectSchemaField> alterFields = new List<DataObjectSchemaField>();
            foreach (DataObjectSchemaField mdField in dataObjectSchemaMetaData.Fields)
            {
                bool containsField = false;
                foreach (DataObjectSchemaField dbField in dataObjectSchemaDatabase.Fields)
                {
                    if (dbField.Name.ToLower() == mdField.Name.ToLower())
                    {
                        containsField = true;
                        if (dbField.FieldType != mdField.FieldType ||
                            dbField.Length != mdField.Length ||
                            dbField.Unicode != mdField.Unicode)
                        {
                            dbField.FieldType = mdField.FieldType;
                            dbField.FieldType = mdField.FieldType;
                            dbField.FieldType = mdField.FieldType;
                            alterFields.Add(dbField);
                        }
                    }
                }
                if (!containsField)
                {
                    newFields.Add(mdField);
                }
            }

            if (newFields.Count > 0)
            {
                StringBuilder sqlTxt = new StringBuilder();
                foreach (DataObjectSchemaField field in newFields)
                {
                    sqlTxt.Append("INSERT INTO [DataObjectField](_DataObjectTypeId, FieldName, _DataObjectFieldTypeId, FieldLength, Unicode) ");
                    sqlTxt.AppendLine(string.Format("VALUES ({0}, '{1}', {2}, {3}, {4})", dataObjectSchemaDatabase._Id, field.Name, (byte)field.FieldType, field.Length, (field.Unicode ? 1 : 0)));
                }
                ScriptCommon.ExecuteCommand(connection, sqlTxt.ToString());
            }

            //TODO: Implement safe exception friendly field type modification

            AlterDataObjectSchemaIndexes(connection, dataObjectSchemaDatabase.TableName, dataObjectSchemaDatabase, dataObjectSchemaMetaData);

            return GetSchema(connection, dataObjectSchemaDatabase.ObjectType);
        }

        private void AlterDataObjectSchemaIndexes(DbConnection connection, string tableName, DataObjectSchema dataObjectSchemaDatabase, DataObjectSchema dataObjectSchemaMetaData)
        {
            List<DataObjectSchemaIndex> newIndexes = new List<DataObjectSchemaIndex>();
            List<DataObjectSchemaIndex> alterIndexes = new List<DataObjectSchemaIndex>();
            List<DataObjectSchemaIndex> dropIndexes = new List<DataObjectSchemaIndex>();
            foreach (DataObjectSchemaIndex mdIndex in dataObjectSchemaMetaData.Indexes)
            {
                bool containsIndex = false;
                foreach (DataObjectSchemaIndex dbIndex in dataObjectSchemaDatabase.Indexes)
                {
                    if (dbIndex.Name == mdIndex.Name)
                    {
                        containsIndex = true;

                        if (!mdIndex.EqualsIndex(dbIndex))
                            alterIndexes.Add(mdIndex);
                        break;
                    }
                }
                if (!containsIndex)
                    newIndexes.Add(mdIndex);
            }
            foreach (DataObjectSchemaIndex dbIndex in dataObjectSchemaDatabase.Indexes)
            {
                bool containsIndex = false;
                foreach (DataObjectSchemaIndex mdIndex in dataObjectSchemaMetaData.Indexes)
                {

                    if (dbIndex.Name == mdIndex.Name)
                    {
                        containsIndex = true;
                        break;
                    }
                }
                if (!containsIndex)
                    dropIndexes.Add(dbIndex);
            }

            if (newIndexes.Count > 0 || alterIndexes.Count > 0 || dropIndexes.Count > 0)
            {
                StringBuilder sqlTxt = new StringBuilder();
                sqlTxt.AppendLine("DECLARE @DataObjectIndexId INT");
                sqlTxt.AppendLine("DECLARE @DataObjectFieldId INT");
                sqlTxt.AppendLine();

                foreach (DataObjectSchemaIndex index in dropIndexes)
                {
                    sqlTxt.AppendLine(string.Format("DELETE FROM [DataObjectIndex] WHERE [_Id] = {0} AND [_DataObjectTypeId] = {1}", index._Id, dataObjectSchemaDatabase._Id));
                }
                foreach (DataObjectSchemaIndex index in alterIndexes)
                {
                    DataObjectSchemaIndex storedIndex = dataObjectSchemaDatabase.FindIndex(index.Name);
                    if (storedIndex != null)
                    {
                        sqlTxt.AppendLine(string.Format("DELETE FROM [DataObjectIndex] WHERE [_Id] = {0} AND [_DataObjectTypeId] = {1}", storedIndex._Id, dataObjectSchemaDatabase._Id));
                    }
                }
                sqlTxt.AppendLine();

                foreach (DataObjectSchemaIndex index in newIndexes)
                {
                    sqlTxt.AppendLine("SELECT @DataObjectIndexId = Max(_Id) + 1 FROM DataObjectIndex");
                    sqlTxt.AppendLine("IF @DataObjectIndexId IS NULL");
                    sqlTxt.AppendLine("   SET @DataObjectIndexId = 1");

                    foreach (DataObjectSchemaFieldIndex indexField in index.Fields)
                    {
                        sqlTxt.AppendLine(string.Format("SELECT @DataObjectFieldId = _Id FROM DataObjectField WHERE _DataObjectTypeId = {0} AND FieldName = '{1}' AND _DataObjectFieldTypeId = {2}",
                            dataObjectSchemaDatabase._Id,
                            indexField.Field.Name,
                            (byte)indexField.Field.FieldType));
                        sqlTxt.AppendLine("INSERT INTO DataObjectIndex ([_Id],[_DataObjectTypeId],[_DataObjectFieldId],[IndexName],[Ascending])");
                        sqlTxt.AppendLine(string.Format("VALUES (@DataObjectIndexId, {0}, @DataObjectFieldId, '{1}', {2})", dataObjectSchemaDatabase._Id, index.Name, (indexField.Ascending ? 1 : 0)));
                    }
                }

                foreach (DataObjectSchemaIndex index in alterIndexes)
                {
                    sqlTxt.AppendLine("SELECT @DataObjectIndexId = Max(_Id) + 1 FROM DataObjectIndex");
                    sqlTxt.AppendLine("IF @DataObjectIndexId IS NULL");
                    sqlTxt.AppendLine("   SET @DataObjectIndexId = 1");

                    foreach (DataObjectSchemaFieldIndex indexField in index.Fields)
                    {
                        sqlTxt.AppendLine(string.Format("SELECT @DataObjectFieldId = _Id FROM DataObjectField WHERE _DataObjectTypeId = {0} AND FieldName = '{1}' AND _DataObjectFieldTypeId = {2}",
                            dataObjectSchemaDatabase._Id,
                            indexField.Field.Name,
                            (byte)indexField.Field.FieldType));
                        sqlTxt.AppendLine("INSERT INTO DataObjectIndex ([_Id],[_DataObjectTypeId],[_DataObjectFieldId],[IndexName],[Ascending])");
                        sqlTxt.AppendLine(string.Format("VALUES (@DataObjectIndexId, {0}, @DataObjectFieldId, '{1}', {2})", dataObjectSchemaDatabase._Id, index.Name, (indexField.Ascending ? 1 : 0)));
                    }
                }
                ScriptCommon.ExecuteCommand(connection, sqlTxt.ToString());
            }
        }

        private void CreateTable(DbConnection connection, Type objectType, DataObjectSchema dataObjectSchemaMetaData)
        {
            string[] tableNames = new string[] { objectType.Name, objectType.Name + "_History", objectType.Name + "_Archive" };
            foreach (string tableName in tableNames)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("CREATE TABLE [dbo].[" + tableName + "](");
                sb.AppendLine("[_Id] [bigint] NOT NULL,");
                sb.AppendLine("[_RevisionId] [int] NOT NULL,");
                sb.AppendLine("[_Deleted] [bit] NOT NULL,");
                sb.AppendLine("[_ObjectTypeId] [smallint] NOT NULL,");

                //Add all of the fields to the table
                foreach (DataObjectSchemaField field in dataObjectSchemaMetaData.Fields)
                {
                    if ((byte)field.FieldType < 21)
                        CreateTableField(sb, field);
                }

                sb.AppendLine("[_TimeStamp] [datetime] NOT NULL,");
                sb.AppendLine("CONSTRAINT [PK_" + tableName + "] PRIMARY KEY CLUSTERED ");
                sb.AppendLine("(");
                sb.AppendLine("	[_Id] ASC,");
                sb.AppendLine("	[_RevisionId] ASC");
                sb.AppendLine(")WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]");
                sb.AppendLine(") ON [PRIMARY]");

                ScriptCommon.ExecuteCommand(connection, sb.ToString());

                //Create the table indexes
                CreateTableIndexes(connection, tableName, dataObjectSchemaMetaData);
            }
        }

        private void CreateTableIndexes(DbConnection connection, string tableName, DataObjectSchema dataObjectSchemaMetaData)
        {
            StringBuilder indexBuilder = new StringBuilder();
            indexBuilder.AppendLine(string.Format("CREATE NONCLUSTERED INDEX [IDX_{0}_ID_Revision_Timestamp_Deleted] ON [dbo].[{0}] ", tableName));
            indexBuilder.AppendLine("	(");
            indexBuilder.AppendLine("		[_Id] ASC,");
            indexBuilder.AppendLine("		[_RevisionId] DESC,");
            indexBuilder.AppendLine("		[_TimeStamp] DESC,");
            indexBuilder.AppendLine("		[_Deleted] DESC");
            indexBuilder.AppendLine("	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]");
            indexBuilder.AppendLine();
            ScriptCommon.ExecuteCommand(connection, indexBuilder.ToString());

            foreach (DataObjectSchemaIndex index in dataObjectSchemaMetaData.Indexes)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("CREATE NONCLUSTERED INDEX [{0}] ON [dbo].[{1}]", index.Name, tableName));
                sb.AppendLine("(");

                foreach (DataObjectSchemaFieldIndex fieldIndex in index.Fields)
                {
                    sb.Append(string.Format("[{0}] ", fieldIndex.Field.Name));
                    if (fieldIndex.Ascending)
                        sb.Append("ASC");
                    else
                        sb.Append("DESC");
                    sb.Append(",");
                }
                sb.Length--;
                sb.AppendLine(")WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]");

                //Create the index
                ScriptCommon.ExecuteCommand(connection, sb.ToString());
            }
        }

        private void AlterTable(DbConnection connection, Type objectType, DataObjectSchema dataObjectSchemaDatabase, DataObjectSchema dataObjectSchemaMetaData)
        {
            List<DataObjectSchemaField> newFields = new List<DataObjectSchemaField>();
            List<DataObjectSchemaField> alterFields = new List<DataObjectSchemaField>();
            foreach (DataObjectSchemaField mdField in dataObjectSchemaMetaData.Fields)
            {
                if ((byte)mdField.FieldType < 11)
                {
                    bool containsField = false;
                    foreach (DataObjectSchemaField dbField in dataObjectSchemaDatabase.Fields)
                    {
                        if ((byte)dbField.FieldType < 11)
                        {
                            if (dbField.Name.ToLower() == mdField.Name.ToLower())
                            {
                                containsField = true;
                                if (dbField.FieldType != mdField.FieldType ||
                                    dbField.Length != mdField.Length ||
                                    dbField.Unicode != mdField.Unicode)
                                {
                                    dbField.FieldType = mdField.FieldType;
                                    dbField.FieldType = mdField.FieldType;
                                    dbField.FieldType = mdField.FieldType;
                                    alterFields.Add(dbField);
                                }
                            }
                        }
                    }
                    if (!containsField)
                    {
                        newFields.Add(mdField);
                    }
                }
            }

            string[] tableNames = new string[] { objectType.Name, objectType.Name + "_History", objectType.Name + "_Archive" };
            foreach (string tableName in tableNames)
            {
                if (newFields.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    string alterStatement = "ALTER TABLE [dbo].[" + tableName + "] ADD ";
                    sb.AppendLine(alterStatement);

                    foreach (DataObjectSchemaField field in newFields)
                    {
                        CreateTableField(sb, field);
                    }

                    string sqlTxt = sb.ToString().Substring(0, sb.ToString().LastIndexOf(','));
                    ScriptCommon.ExecuteCommand(connection, sqlTxt);
                }

                //TODO: Implement safe exception friendly field type modification

                AlterTableIndexes(connection, tableName, dataObjectSchemaDatabase, dataObjectSchemaMetaData);
            }
        }

        private void AlterTableIndexes(DbConnection connection, string tableName, DataObjectSchema dataObjectSchemaDatabase, DataObjectSchema dataObjectSchemaMetaData)
        {
            List<DataObjectSchemaIndex> newIndexes = new List<DataObjectSchemaIndex>();
            List<DataObjectSchemaIndex> alterIndexes = new List<DataObjectSchemaIndex>();
            List<DataObjectSchemaIndex> dropIndexes = new List<DataObjectSchemaIndex>();
            foreach (DataObjectSchemaIndex mdIndex in dataObjectSchemaMetaData.Indexes)
            {
                bool containsIndex = false;
                foreach (DataObjectSchemaIndex dbIndex in dataObjectSchemaDatabase.Indexes)
                {
                    if (dbIndex.Name == mdIndex.Name)
                    {
                        containsIndex = true;

                        if (!mdIndex.EqualsIndex(dbIndex))
                            alterIndexes.Add(mdIndex);
                        break;
                    }
                }
                if (!containsIndex)
                    newIndexes.Add(mdIndex);
            }
            foreach (DataObjectSchemaIndex dbIndex in dataObjectSchemaDatabase.Indexes)
            {
                bool containsIndex = false;
                foreach (DataObjectSchemaIndex mdIndex in dataObjectSchemaMetaData.Indexes)
                {

                    if (dbIndex.Name == mdIndex.Name)
                    {
                        containsIndex = true;
                        break;
                    }
                }
                if (!containsIndex)
                    dropIndexes.Add(dbIndex);
            }

            StringBuilder sb = new StringBuilder();
            List<string> droppedIndexNames = new List<string>();
            foreach (DataObjectSchemaIndex index in dropIndexes)
            {
                sb.Length = 0;
                droppedIndexNames.Add(index.Name);
                sb.Append(string.Format("DROP INDEX {0}.{1}", tableName, index.Name));
                ScriptCommon.ExecuteCommand(connection, sb.ToString());
            }
            foreach (DataObjectSchemaIndex index in alterIndexes)
            {
                sb.Length = 0;
                if (droppedIndexNames.Contains(index.Name) == false)
                {
                    sb.Append(string.Format("DROP INDEX {0}.{1}", tableName, index.Name));
                    ScriptCommon.ExecuteCommand(connection, sb.ToString());
                }
            }

            foreach (DataObjectSchemaIndex index in newIndexes)
            {
                sb.Length = 0;
                sb.AppendLine(string.Format("CREATE NONCLUSTERED INDEX [{0}] ON [dbo].[{1}]", index.Name, tableName));
                sb.AppendLine("(");

                foreach (DataObjectSchemaFieldIndex fieldIndex in index.Fields)
                {
                    sb.Append(string.Format("[{0}] ", fieldIndex.Field.Name));
                    if (fieldIndex.Ascending)
                        sb.Append("ASC");
                    else
                        sb.Append("DESC");
                    sb.Append(",");
                }
                sb.Length--;
                sb.AppendLine(")WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]");

                //Create the index
                ScriptCommon.ExecuteCommand(connection, sb.ToString());
            }

            foreach (DataObjectSchemaIndex index in alterIndexes)
            {
                sb.Length = 0;
                sb.AppendLine(string.Format("CREATE NONCLUSTERED INDEX [{0}] ON [dbo].[{1}]", index.Name, tableName));
                sb.AppendLine("(");

                foreach (DataObjectSchemaFieldIndex fieldIndex in index.Fields)
                {
                    sb.Append(string.Format("[{0}] ", fieldIndex.Field.Name));
                    if (fieldIndex.Ascending)
                        sb.Append("ASC");
                    else
                        sb.Append("DESC");
                    sb.Append(",");
                }
                sb.Length--;
                sb.AppendLine(")WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]");

                //Create the index
                ScriptCommon.ExecuteCommand(connection, sb.ToString());
            }
        }

        internal static void CreateTableField(StringBuilder tableBuilder, DataObjectSchemaField field)
        {
            switch (field.FieldType)
            {
                case DataObjectSchemaFieldType.Boolean:
                case DataObjectSchemaFieldType.NullableBoolean:
                    tableBuilder.Append(string.Format("   [{0}] bit NULL", field.Name));
                    break;
                case DataObjectSchemaFieldType.Byte:
                case DataObjectSchemaFieldType.NullableByte:
                    tableBuilder.Append(string.Format("   [{0}] tinyint NULL", field.Name));
                    break;
                case DataObjectSchemaFieldType.Short:
                case DataObjectSchemaFieldType.NullableShort:
                    tableBuilder.Append(string.Format("   [{0}] smallint NULL", field.Name));
                    break;
                case DataObjectSchemaFieldType.Int:
                case DataObjectSchemaFieldType.NullableInt:
                    tableBuilder.Append(string.Format("   [{0}] int NULL", field.Name));
                    break;
                case DataObjectSchemaFieldType.Long:
                case DataObjectSchemaFieldType.NullableLong:
                    tableBuilder.Append(string.Format("   [{0}] bigint NULL", field.Name));
                    break;
                case DataObjectSchemaFieldType.Double:
                case DataObjectSchemaFieldType.NullableDouble:
                    tableBuilder.Append(string.Format("   [{0}] float NULL", field.Name));
                    break;
                case DataObjectSchemaFieldType.Float:
                case DataObjectSchemaFieldType.NullableFloat:
                    tableBuilder.Append(string.Format("   [{0}] real NULL", field.Name));
                    break;
                case DataObjectSchemaFieldType.Decimal:
                case DataObjectSchemaFieldType.NullableDecimal:
                    tableBuilder.Append(string.Format("   [{0}] decimal NULL", field.Name));
                    break;
                case DataObjectSchemaFieldType.DateTime:
                case DataObjectSchemaFieldType.NullableDateTime:
                    tableBuilder.Append(string.Format("   [{0}] datetime NULL", field.Name));
                    break;
                case DataObjectSchemaFieldType.Guid:
                case DataObjectSchemaFieldType.NullableGuid:
                    tableBuilder.Append(string.Format("   [{0}] uniqueidentifier NULL", field.Name));
                    break;
                case DataObjectSchemaFieldType.String:
                    if (field.Length != int.MaxValue)
                    {
                        if (field.Unicode)
                            tableBuilder.Append(string.Format("   [{0}] nvarchar({1}) NULL", field.Name, field.Length));
                        else
                            tableBuilder.Append(string.Format("   [{0}] varchar({1}) NULL", field.Name, field.Length));
                    }
                    else
                    {
                        if (field.Unicode)
                            tableBuilder.Append(string.Format("   [{0}] ntext NULL", field.Name));
                        else
                            tableBuilder.Append(string.Format("   [{0}] text NULL", field.Name));
                    }
                    break;
            }
            tableBuilder.AppendLine(",");
        }
    }
}