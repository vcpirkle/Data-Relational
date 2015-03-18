using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataRelational.Attributes;
using System.Data;
using DataRelational.Cache;
using System.Reflection;
using DataRelational.Script;
using DataRelational.Schema;
using System.Data.Common;
using System.Collections;

namespace DataRelational.DataWriters
{
    /// <summary>
    /// A data writer for the T-Sql standard.
    /// </summary>
    class TSqlDataWriter : IDataWriter
    {
        #region IDataWriter Members

        /// <summary>
        /// Begins a batch write process.
        /// </summary>
        /// <param name="builder">The string builder to write to.</param>
        public void BeginWrite(StringBuilder builder)
        {
            builder.AppendLine("DECLARE @WriteTime DateTime");
            builder.AppendLine("DECLARE @RevisionId Int");
            builder.AppendLine("DECLARE @ListId BigInt");
            builder.AppendLine("DECLARE @ListIds Table([_Id] BigInt)");
            builder.AppendLine("DECLARE @WriteTimes Table([_Id] BigInt, [_RevisionId] Int, [_DataObjectTypeId] SmallInt, [WriteTime] DateTime)");
            builder.Append("DECLARE @Relationships TABLE([ParentId] BigInt, [ParentTypeId] SmallInt, [ParentFieldId] Int, ");
            builder.AppendLine("[ChildId] BigInt, [ChildTypeId] SmallInt, [BeginDate] DateTime, [EndDate] DateTime, [Deleted] Bit, [DeleteBehavior] TinyInt)");
        }

        /// <summary>
        /// Writes a data relational object to the batch.
        /// </summary>
        /// <param name="builder">The string builder to write to.</param>
        /// <param name="obj">The data relational object to write.</param>
        /// <param name="objAttribute">The savable object attribute associated with the data relational object.</param>
        public void Write(StringBuilder builder, IDataObject obj, SavableObjectAttribute objAttribute)
        {
            if (objAttribute.SaveBehavior == SaveBehavior.Never)
                return;
            if (objAttribute.SaveBehavior == SaveBehavior.Dirty && obj._Dirty == false)
                return;

            DataObjectReflection objReflection = DataManager.ReflectionCache[obj.GetType()];

            IHistoricalDataObject hObj = obj as IHistoricalDataObject;

            builder.AppendLine("SET @WriteTime = GetUtcDate()");
            if (hObj != null)
                builder.AppendLine(string.Format("SET @RevisionId = {0}", hObj._RevisionId + 1));
            else
                builder.AppendLine(string.Format("SET @RevisionId = (SELECT CASE WHEN Max(_RevisionId) IS NULL THEN 1 ELSE Max(_RevisionId) END FROM [{0}_History] WHERE _Id = {1})", objReflection.Schema.TableName, obj._Id));

            if (obj._Deleted == false)
            {
                if (obj._Id < 1)
                {
                    obj._Id = objReflection.NextIdentifier;
                    WriteInsert(builder, objReflection.Schema.TableName, obj, objReflection);
                    WriteInsert(builder, objReflection.Schema.TableName + "_History", obj, objReflection);
                    WritePrimativeListInsert(builder, obj, objReflection);
                }
                else if (hObj == null)
                {
                    WriteUpdate(builder, objReflection.Schema.TableName, obj, objReflection);
                    WriteUpdate(builder, objReflection.Schema.TableName + "_History", obj, objReflection);
                    WritePrimativeListUpdate(builder, obj, objReflection);
                }
                else
                {
                    WriteUpdate(builder, objReflection.Schema.TableName, obj, objReflection);
                    WriteInsert(builder, objReflection.Schema.TableName + "_History", obj, objReflection);
                    WritePrimativeListInsert(builder, obj, objReflection);
                }
            }
            else
            {
                if (obj._Id > 0)
                {
                    if (objAttribute.DeleteBehavior == DeleteBehavior.Soft)
                    {
                        if (hObj == null)
                        {
                            WriteDeleteRevision(builder, objReflection.Schema.TableName, obj, objReflection);
                            WriteUpdate(builder, objReflection.Schema.TableName + "_History", obj, objReflection);
                            WritePrimativeListUpdate(builder, obj, objReflection);
                        }
                        else
                        {
                            WriteDelete(builder, objReflection.Schema.TableName, obj, objReflection);
                            WriteInsert(builder, objReflection.Schema.TableName + "_History", obj, objReflection);
                            WritePrimativeListInsert(builder, obj, objReflection);
                        }
                    }
                    else if (objAttribute.DeleteBehavior == DeleteBehavior.Hard)
                    {
                        WriteDelete(builder, objReflection.Schema.TableName, obj, objReflection);
                        WriteDelete(builder, objReflection.Schema.TableName + "_History", obj, objReflection);
                        WritePrimativeListDelete(builder, obj, objReflection);
                    }
                    else
                    {
                        if (hObj == null)
                        {
                            WriteDeleteRevision(builder, objReflection.Schema.TableName, obj, objReflection);
                            WriteUpdate(builder, objReflection.Schema.TableName + "_History", obj, objReflection);
                            WritePrimativeListUpdate(builder, obj, objReflection);
                            WriteArchive(builder, objReflection.Schema.TableName + "_History", objReflection.Schema.TableName + "_Archive", obj, objReflection);
                        }
                        else
                        {
                            WriteDelete(builder, objReflection.Schema.TableName, obj, objReflection);
                            WriteInsert(builder, objReflection.Schema.TableName + "_History", obj, objReflection);
                            WritePrimativeListInsert(builder, obj, objReflection);
                            WriteArchive(builder, objReflection.Schema.TableName + "_History", objReflection.Schema.TableName + "_Archive", obj, objReflection);
                        }
                    }
                }
            }
            builder.AppendLine(string.Format("INSERT INTO @WriteTimes SELECT {0}, @RevisionId, {1}, @WriteTime", obj._Id, obj._ObjectType._Id));
            builder.AppendLine();
        }

        /// <summary>
        /// Writes the parent child relationships between a parent and child object to the batch.
        /// </summary>
        /// <param name="builder">The string builder to write to.</param>
        /// <param name="obj">The data relational object to write relationshps for.</param>
        /// <param name="objAttribute">The savable object attribute associated with the data relational parent object.</param>
        /// <param name="writtenRelationships">The list or relationships written in this save context.</param>
        public void WriteRelationships(StringBuilder builder, IDataObject obj, SavableObjectAttribute objAttribute, List<Relationship> writtenRelationships)
        {
            DataObjectReflection objectReflection = DataManager.ReflectionCache[obj.GetType()];
            if (obj._Deleted == true && objAttribute.DeleteBehavior == DeleteBehavior.Hard)
            {
                builder.Append("INSERT INTO @Relationships([ParentId], [ParentTypeId], [ParentFieldId], ");
                builder.Append("[ChildId], [ChildTypeId], [BeginDate], [EndDate], [Deleted], [DeleteBehavior])");
                builder.AppendLine(string.Format("VALUES({0}, {1}, NULL, NULL, NULL, NULL, NULL, 1, {2})", obj._Id, obj._ObjectType._Id, (byte)objAttribute.DeleteBehavior));
            }
            else
            {
                List<FieldChildObject> children = ScriptCommon.GetChildren(obj, objectReflection);
                foreach (DataObjectSchemaField field in objectReflection.Schema.Fields)
                {
                    if (field.FieldType == DataObjectSchemaFieldType.IDataObject ||
                        field.FieldType == DataObjectSchemaFieldType.IDataObjectArray ||
                        field.FieldType == DataObjectSchemaFieldType.IDataObjectList)
                    {
                        foreach (FieldChildObject fieldChild in children)
                        {
                            if (fieldChild.ChildObject._ObjectType != null)
                            {
                                if (field.Name.ToLower() == fieldChild.FieldName.ToLower())
                                {
                                    if (!ContainsRelationship(obj._Id, obj._ObjectType._Id, field._Id, fieldChild.ChildObject._Id, fieldChild.ChildObject._ObjectType._Id, writtenRelationships))
                                    {
                                        writtenRelationships.Add(new Relationship(obj._Id, obj._ObjectType._Id, field._Id, fieldChild.ChildObject._Id, fieldChild.ChildObject._ObjectType._Id, DateTime.Now, DataManager.MaxDate));
                                        builder.Append("INSERT INTO @Relationships([ParentId], [ParentTypeId], [ParentFieldId], ");
                                        builder.Append("[ChildId], [ChildTypeId], [BeginDate], [EndDate], [Deleted], [DeleteBehavior])");
                                        builder.AppendLine(string.Format("VALUES({0}, {1}, {2}, {3}, {4}, {5}, '{6}', {7}, {8})", obj._Id, obj._ObjectType._Id, field._Id, fieldChild.ChildObject._Id, fieldChild.ChildObject._ObjectType._Id, "@WriteTime", DataManager.MaxDate, obj._Deleted ? 1 : 0, (byte)objAttribute.DeleteBehavior));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            builder.AppendLine();
        }

        private bool ContainsRelationship(long parentId, short parentObjectTypeId, int parentFieldId, long childId, short childObjectTypeId, List<Relationship> relationships)
        {
            foreach (Relationship r in relationships)
            {
                if (r.ParentId == parentId &&
                    r.ParentTypeId == parentObjectTypeId &&
                    r.ParentFieldId == parentFieldId &&
                    r.ChildId == childId &&
                    r.ChildTypeId == childObjectTypeId)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Ends a batch write process and executes the batch.
        /// </summary>
        /// <param name="builder">The string builder to write to.</param>
        /// <param name="connectionName">The name of the connection to write to.</param>
        /// <returns>A data table containing the columns {_Id, _RevisionId, _DataObjectTypeId, and WriteTime}</returns>
        public DataTable EndWrite(StringBuilder builder, string connectionName)
        {
            builder.AppendLine("UPDATE R");
            builder.AppendLine("   SET [EndDate] = @WriteTime");
            builder.AppendLine("  FROM [DataObjectRelationship] R");
            builder.AppendLine("  JOIN @Relationships newR");
            builder.AppendLine("    ON R.ParentId = newR.ParentId");
            builder.AppendLine("   AND R.ParentTypeId = newR.ParentTypeId");
            builder.AppendLine("   AND R.ParentFieldId = newR.ParentFieldId");
            builder.AppendLine("   AND R.ChildId <> newR.ChildId");
            builder.AppendLine(string.Format(" WHERE R.EndDate = '{0}'", DataManager.MaxDate));
            builder.AppendLine("   AND R.ChildId NOT IN (SELECT ChildId FROM @Relationships WHERE ParentId = R.ParentId AND ParentTypeId = R.ParentTypeId AND ParentFieldId = R.ParentFieldId)");
            builder.AppendLine();
            builder.AppendLine("INSERT INTO [DataObjectRelationship]([ParentId], [ParentTypeId], [ParentFieldId], [ChildId], [ChildTypeId], [BeginDate], [EndDate])");
            builder.AppendLine("SELECT [ParentId], [ParentTypeId], [ParentFieldId], [ChildId], [ChildTypeId], [BeginDate], [EndDate]");
            builder.AppendLine("  FROM @Relationships newR");
            builder.AppendLine(" WHERE newR.ParentFieldId IS NOT NULL");
            builder.AppendLine(string.Format("   AND (SELECT Count(1) FROM [DataObjectRelationship] R WHERE R.ParentId = newR.ParentId AND R.ParentTypeId = newR.ParentTypeId AND R.ParentFieldId = newR.ParentFieldId AND R.ChildId = newR.ChildId AND R.EndDate = '{0}') = 0", DataManager.MaxDate));
            builder.AppendLine();
            builder.AppendLine("DELETE R");
            builder.AppendLine("  FROM [DataObjectRelationship] R");
            builder.AppendLine("  JOIN @Relationships newR");
            builder.AppendLine("    ON R.ParentId = newR.ParentId");
            builder.AppendLine("   AND R.ParentTypeId = newR.ParentTypeId");
            builder.AppendLine(string.Format(" WHERE newR.DeleteBehavior = {0}", (byte)DeleteBehavior.Hard));
            builder.AppendLine("   AND newR.Deleted = 1");

            builder.Append("SELECT [_Id], [_RevisionId], [_DataObjectTypeId], [WriteTime] FROM @WriteTimes");
            using (DbConnection connection = DataManager.ScriptManager.GetDatabaseConnection(connectionName))
            {
                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.Transaction = connection.BeginTransaction();
                    cmd.CommandText = builder.ToString();
                    cmd.CommandType = System.Data.CommandType.Text;
                    try
                    {
                        DataTable writeTimes = new DataTable("WriteTimes");
                        writeTimes.Columns.Add("_Id", typeof(long));
                        writeTimes.Columns.Add("_RevisionId", typeof(int));
                        writeTimes.Columns.Add("_DataObjectTypeId", typeof(short));
                        writeTimes.Columns.Add("WriteTime", typeof(DateTime));

                        DateTime start = DateTime.Now;
                        using (DbDataReader reader = cmd.ExecuteReader())
                        {
                            DateTime end = DateTime.Now;
                            TimeSpan ts = (end - start);
                            Logging.Information(string.Format("Database write time - {0} seconds {1} milliseconds", ts.Seconds, ts.Milliseconds));

                            while (reader.Read())
                            {
                                writeTimes.Rows.Add(reader[0], reader[1], reader[2], reader[3]);
                            }
                            reader.Close();
                        }
                        cmd.Transaction.Commit();

                        return writeTimes;
                    }
                    catch (Exception ex)
                    {
                        cmd.Transaction.Rollback();
                        throw new DataRelationalException("An error occured while attempting to write data.", ex);
                    }
                }
            }
        }

        #endregion

        #region Methods

        private void WriteInsert(StringBuilder sb, string tableName, IDataObject obj, DataObjectReflection objReflection)
        {
            IHistoricalDataObject hObj = obj as IHistoricalDataObject;
            sb.Append("INSERT INTO [" + tableName + "]");
            sb.Append("([_Id]");
            sb.Append(", [_RevisionId]");
            sb.Append(", [_Deleted]");
            sb.Append(", [_ObjectTypeId]");

            foreach (DataObjectSchemaField field in objReflection.Schema.Fields)
            {
                if ((byte)field.FieldType < 21)
                    sb.Append(", [" + field.Name + "]");
            }

            sb.Append(" , [_TimeStamp]");
            sb.AppendLine(")");
            sb.Append("VALUES");
            sb.Append("(");
            sb.Append(string.Format("{0}", obj._Id));
            sb.Append(", @RevisionId");
            sb.Append(string.Format(", {0}", (obj._Deleted ? 1 : 0)));
            sb.Append(string.Format(", {0}", obj._ObjectType._Id));

            foreach (DataObjectSchemaField field in objReflection.Schema.Fields)
            {
                if ((byte)field.FieldType < 21)
                {
                    bool foundField = false;
                    foreach (FieldInfo df in objReflection.DataFields)
                    {
                        if (df.Name.ToLower() == field.Name.ToLower())
                        {
                            foundField = true;
                            sb.Append(", ");
                            WriteFieldValue(sb, field.FieldType, df.GetValue(obj), df, null, obj);
                            break;
                        }
                    }
                    if (!foundField)
                    {
                        foreach (PropertyInfo pf in objReflection.DataProperties)
                        {
                            if (pf.Name.ToLower() == field.Name.ToLower())
                            {
                                foundField = true;
                                sb.Append(", ");
                                WriteFieldValue(sb, field.FieldType, pf.GetValue(obj, null), null, pf, obj);
                                break;
                            }
                        }
                    }
                    if (!foundField)
                    {
                        sb.Append(string.Format(", (SELECT [{0}] FROM [{1}] WHERE _Id = {2} AND _RevisionId = {3})",
                            field.Name,
                            tableName,
                            obj._Id,
                            (hObj != null ? hObj._RevisionId : 1)));
                    }
                }
            }
            sb.Append(" , @WriteTime");
            sb.AppendLine(")");
        }

        private void WriteUpdate(StringBuilder sb, string tableName, IDataObject obj, DataObjectReflection objReflection)
        {
            IHistoricalDataObject hObj = obj as IHistoricalDataObject;
            sb.Append("UPDATE [" + tableName + "] ");
            sb.Append("SET [_RevisionId] = @RevisionId");
            sb.Append(string.Format(", [_Deleted] = {0}", (obj._Deleted ? 1 : 0)));

            foreach (DataObjectSchemaField field in objReflection.Schema.Fields)
            {
                if ((byte)field.FieldType < 21)
                {
                    bool foundField = false;
                    foreach (FieldInfo df in objReflection.DataFields)
                    {
                        if (df.Name.ToLower() == field.Name.ToLower())
                        {
                            foundField = true;
                            sb.Append(", [" + field.Name + "] = ");
                            WriteFieldValue(sb, field.FieldType, df.GetValue(obj), df, null, obj);
                            break;
                        }
                    }
                    if (!foundField)
                    {
                        foreach (PropertyInfo pf in objReflection.DataProperties)
                        {
                            if (pf.Name.ToLower() == field.Name.ToLower())
                            {
                                foundField = true;
                                sb.Append(", [" + field.Name + "] = ");
                                WriteFieldValue(sb, field.FieldType, pf.GetValue(obj, null), null, pf, obj);
                                break;
                            }
                        }
                    }
                }
            }

            sb.AppendLine(", [_TimeStamp] = @WriteTime");
            sb.Append(string.Format("Where _Id = {0} ", obj._Id));
            if (hObj != null)
                sb.AppendLine(string.Format("AND _RevisionId = {0}", hObj._RevisionId));
            else
                sb.AppendLine("AND _RevisionId = @RevisionId");
        }

        private void WriteDelete(StringBuilder sb, string tableName, IDataObject obj, DataObjectReflection objReflection)
        {
            sb.Append("DELETE FROM [" + tableName + "] ");
            sb.AppendLine(string.Format("WHERE [_Id] = {0}", obj._Id));
        }

        private void WriteDeleteRevision(StringBuilder sb, string tableName, IDataObject obj, DataObjectReflection objReflection)
        {
            sb.Append("DELETE FROM [" + tableName + "] ");
            sb.AppendLine(string.Format("WHERE [_Id] = {0} AND [_RevisionId] = @RevisionId", obj._Id));
        }

        private void WriteArchive(StringBuilder sb, string tableName, string archiveTableName, IDataObject obj, DataObjectReflection objReflection)
        {
            IHistoricalDataObject hObj = obj as IHistoricalDataObject;
            sb.Append("INSERT INTO [" + archiveTableName + "]");
            sb.Append("([_Id]");
            sb.Append(", [_RevisionId]");
            sb.Append(", [_Deleted]");
            sb.Append(", [_ObjectTypeId]");

            foreach (DataObjectSchemaField field in objReflection.Schema.Fields)
            {
                if ((byte)field.FieldType < 21)
                    sb.Append(", [" + field.Name + "]");
            }

            sb.Append(" , [_TimeStamp]");
            sb.AppendLine(")");
            sb.Append("SELECT [_Id]");
            sb.Append(", [_RevisionId]");
            sb.Append(", [_Deleted]");
            sb.Append(", [_ObjectTypeId]");

            foreach (DataObjectSchemaField field in objReflection.Schema.Fields)
            {
                if ((byte)field.FieldType < 21)
                    sb.Append(", [" + field.Name + "]");
            }
            sb.AppendLine(", @WriteTime");
            sb.AppendLine("  FROM [" + tableName + "]");
            sb.AppendLine(string.Format(" WHERE [_Id] = {0}", obj._Id));
            sb.AppendLine("DELETE FROM [" + tableName + "]");
            sb.AppendLine(string.Format(" WHERE [_Id] = {0}", obj._Id));
        }

        private void WritePrimativeListInsert(StringBuilder sb, IDataObject obj, DataObjectReflection objReflection)
        {
            foreach (DataObjectSchemaField field in objReflection.Schema.Fields)
            {
                if (field.FieldType == DataObjectSchemaFieldType.PrimativeList ||
                    field.FieldType == DataObjectSchemaFieldType.PrimativeArray)
                {
                    bool foundField = false;
                    foreach (FieldInfo df in objReflection.DataFields)
                    {
                        if (df.Name.ToLower() == field.Name.ToLower())
                        {
                            IList list = (IList)df.GetValue(obj);
                            if (list != null)
                            {
                                string tableName = GetListTableName(df.FieldType);
                                sb.AppendLine(string.Format("INSERT INTO [List_{0}] ([_DataObjectTypeId], [ObjectId], [ObjectRevisionId], [_DataObjectFieldId])", tableName));
                                sb.AppendLine(string.Format("VALUES({0}, {1}, @RevisionId, {2})", obj._ObjectType._Id, obj._Id, field._Id));
                                sb.AppendLine("SELECT @ListId = SCOPE_IDENTITY()");
                                int index = 0;
                                foreach (object item in list)
                                {
                                    if (item != null)
                                    {
                                        if (tableName == "DateTime" || tableName == "Guid" || tableName == "String")
                                        {
                                            sb.Append(string.Format("INSERT INTO [List_{0}Value] ([_Id], [_Index], [Value]) ", tableName));
                                            sb.AppendLine(string.Format("VALUES (@ListId, {0}, '{1}')", index++, item.ToString().Replace("'", "''")));
                                        }
                                        else
                                        {
                                            sb.Append(string.Format("INSERT INTO [List_{0}Value] ([_Id], [_Index], [Value]) ", tableName));
                                            if (item.GetType().IsEnum)
                                            {
                                                sb.AppendLine(string.Format("VALUES (@ListId, {0}, {1})", index++, GetEnumValue(item)));
                                            }
                                            else
                                            {
                                                sb.AppendLine(string.Format("VALUES (@ListId, {0}, {1})", index++, item.ToString().Replace("'", "''")));
                                            }
                                        }
                                    }
                                }
                            }

                            foundField = true;
                            break;
                        }
                    }
                    if (!foundField)
                    {
                        foreach (PropertyInfo dp in objReflection.DataProperties)
                        {
                            if (dp.Name == field.Name)
                            {
                                IList list = (IList)dp.GetValue(obj, null);
                                if (list != null)
                                {
                                    string tableName = GetListTableName(dp.PropertyType);
                                    sb.AppendLine(string.Format("INSERT INTO [List_{0}] ([_DataObjectTypeId], [ObjectId], [ObjectRevisionId], [_DataObjectFieldId])", tableName));
                                    sb.AppendLine(string.Format("VALUES({0}, {1}, @RevisionId, {2})", obj._ObjectType._Id, obj._Id, field._Id));
                                    sb.AppendLine("SELECT @ListId = SCOPE_IDENTITY()");
                                    int index = 0;
                                    foreach (object item in list)
                                    {
                                        if (item != null)
                                        {
                                            if (tableName == "DateTime" || tableName == "Guid" || tableName == "String")
                                            {
                                                sb.Append(string.Format("INSERT INTO [List_{0}Value] ([_Id], [_Index], [Value]) ", tableName));
                                                sb.AppendLine(string.Format("VALUES (@ListId, {0}, '{1}')", index++, item.ToString().Replace("'", "''")));
                                            }
                                            else
                                            {
                                                sb.Append(string.Format("INSERT INTO [List_{0}Value] ([_Id], [_Index], [Value]) ", tableName));
                                                if (item.GetType().IsEnum)
                                                {
                                                    sb.AppendLine(string.Format("VALUES (@ListId, {0}, {1})", index++, GetEnumValue(item)));
                                                }
                                                else
                                                {
                                                    sb.AppendLine(string.Format("VALUES (@ListId, {0}, {1})", index++, item.ToString().Replace("'", "''")));
                                                }
                                            }
                                        }
                                    }
                                }

                                foundField = true;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void WritePrimativeListUpdate(StringBuilder sb, IDataObject obj, DataObjectReflection objReflection)
        {
            foreach (DataObjectSchemaField field in objReflection.Schema.Fields)
            {
                if (field.FieldType == DataObjectSchemaFieldType.PrimativeList ||
                    field.FieldType == DataObjectSchemaFieldType.PrimativeArray)
                {
                    bool foundField = false;
                    foreach (FieldInfo df in objReflection.DataFields)
                    {
                        if (df.Name.ToLower() == field.Name.ToLower())
                        {
                            IList list = (IList)df.GetValue(obj);
                            string tableName = GetListTableName(df.FieldType);
                            if (list != null)
                            {
                                sb.AppendLine("SET @ListId = NULL");
                                sb.AppendLine(string.Format("SELECT @ListId = [_Id] FROM [List_{0}] WHERE [_DataObjectTypeId] = {1} AND [ObjectId] = {2} AND [ObjectRevisionId] = {3} AND [_DataObjectFieldId] = {4}",
                                    tableName, obj._ObjectType._Id, obj._Id, "@RevisionId", field._Id));
                                sb.AppendLine("IF @ListId IS NOT NULL");
                                sb.AppendLine(string.Format("   DELETE FROM [List_{0}Value] WHERE [_Id] = @ListId", tableName));
                                sb.AppendLine("ELSE");
                                sb.AppendLine("BEGIN");
                                sb.AppendLine(string.Format("   INSERT INTO [List_{0}] ([_DataObjectTypeId], [ObjectId], [ObjectRevisionId], [_DataObjectFieldId])", tableName));
                                sb.AppendLine(string.Format("   VALUES({0}, {1}, @RevisionId, {2})", obj._ObjectType._Id, obj._Id, field._Id));
                                sb.AppendLine("   SELECT @ListId = SCOPE_IDENTITY()");
                                sb.AppendLine("END");
                                int index = 0;
                                foreach (object item in list)
                                {
                                    if (item != null)
                                    {
                                        if (tableName == "DateTime" || tableName == "Guid" || tableName == "String")
                                        {
                                            sb.Append(string.Format("INSERT INTO [List_{0}Value] ([_Id], [_Index], [Value]) ", tableName));
                                            sb.AppendLine(string.Format("VALUES (@ListId, {0}, '{1}')", index++, item.ToString().Replace("'", "''")));
                                        }
                                        else
                                        {
                                            sb.Append(string.Format("INSERT INTO [List_{0}Value] ([_Id], [_Index], [Value]) ", tableName));
                                            if (item.GetType().IsEnum)
                                            {
                                                sb.AppendLine(string.Format("VALUES (@ListId, {0}, {1})", index++, GetEnumValue(item)));
                                            }
                                            else
                                            {
                                                sb.AppendLine(string.Format("VALUES (@ListId, {0}, {1})", index++, item.ToString().Replace("'", "''")));
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                sb.AppendLine("SET @ListId = NULL");
                                sb.AppendLine(string.Format("SELECT @ListId = [_Id] FROM [List_{0}] WHERE [_DataObjectTypeId] = {1} AND [ObjectId] = {2} AND [ObjectRevisionId] = {3} AND [_DataObjectFieldId] = {4}",
                                                                    tableName, obj._ObjectType._Id, obj._Id, "@RevisionId", field._Id));
                                sb.AppendLine("IF @ListId IS NOT NULL");
                                sb.AppendLine("BEGIN");
                                sb.AppendLine(string.Format("   DELETE FROM [List_{0}Value] WHERE [_Id] = @ListId", tableName));
                                sb.AppendLine(string.Format("   DELETE FROM [List_{0}] WHERE [_Id] = @ListId", tableName));
                                sb.AppendLine("END");
                            }

                            foundField = true;
                            break;
                        }
                    }
                    if (!foundField)
                    {
                        foreach (PropertyInfo dp in objReflection.DataProperties)
                        {
                            if (dp.Name == field.Name)
                            {
                                IList list = (IList)dp.GetValue(obj, null);
                                string tableName = GetListTableName(dp.PropertyType);
                                if (list != null)
                                {
                                    sb.AppendLine("SET @ListId = NULL");
                                    sb.AppendLine(string.Format("SELECT @ListId = [_Id] FROM [List_{0}] WHERE [_DataObjectTypeId] = {1} AND [ObjectId] = {2} AND [ObjectRevisionId] = {3} AND [_DataObjectFieldId] = {4}",
                                        tableName, obj._ObjectType._Id, obj._Id, "@RevisionId", field._Id));
                                    sb.AppendLine("IF @ListId IS NOT NULL");
                                    sb.AppendLine(string.Format("   DELETE FROM [List_{0}Value] WHERE [_Id] = @ListId", tableName));
                                    sb.AppendLine("ELSE");
                                    sb.AppendLine("BEGIN");
                                    sb.AppendLine(string.Format("   INSERT INTO [List_{0}] ([_DataObjectTypeId], [ObjectId], [ObjectRevisionId], [_DataObjectFieldId])", tableName));
                                    sb.AppendLine(string.Format("   VALUES({0}, {1}, @RevisionId, {2})", obj._ObjectType._Id, obj._Id, field._Id));
                                    sb.AppendLine("   SELECT @ListId = SCOPE_IDENTITY()");
                                    sb.AppendLine("END");
                                    int index = 0;
                                    foreach (object item in list)
                                    {
                                        if (item != null)
                                        {
                                            if (tableName == "DateTime" || tableName == "Guid" || tableName == "String")
                                            {
                                                sb.Append(string.Format("INSERT INTO [List_{0}Value] ([_Id], [_Index], [Value]) ", tableName));
                                                sb.AppendLine(string.Format("VALUES (@ListId, {0}, '{1}')", index++, item.ToString().Replace("'", "''")));
                                            }
                                            else
                                            {
                                                sb.Append(string.Format("INSERT INTO [List_{0}Value] ([_Id], [_Index], [Value]) ", tableName));
                                                if (item.GetType().IsEnum)
                                                {
                                                    sb.AppendLine(string.Format("VALUES (@ListId, {0}, {1})", index++, GetEnumValue(item)));
                                                }
                                                else
                                                {
                                                    sb.AppendLine(string.Format("VALUES (@ListId, {0}, {1})", index++, item.ToString().Replace("'", "''")));
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    sb.AppendLine("SET @ListId = NULL");
                                    sb.AppendLine(string.Format("SELECT @ListId = [_Id] FROM [List_{0}] WHERE [_DataObjectTypeId] = {1} AND [ObjectId] = {2} AND [ObjectRevisionId] = {3} AND [_DataObjectFieldId] = {4}",
                                                                        tableName, obj._ObjectType._Id, obj._Id, "@RevisionId", field._Id));
                                    sb.AppendLine("IF @ListId IS NOT NULL");
                                    sb.AppendLine("BEGIN");
                                    sb.AppendLine(string.Format("   DELETE FROM [List_{0}Value] WHERE [_Id] = @ListId", tableName));
                                    sb.AppendLine(string.Format("   DELETE FROM [List_{0}] WHERE [_Id] = @ListId", tableName));
                                    sb.AppendLine("END");
                                }

                                foundField = true;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void WritePrimativeListDeleteRevision(StringBuilder sb, IDataObject obj, DataObjectReflection objReflection)
        {
            foreach (DataObjectSchemaField field in objReflection.Schema.Fields)
            {
                if (field.FieldType == DataObjectSchemaFieldType.PrimativeList ||
                    field.FieldType == DataObjectSchemaFieldType.PrimativeArray)
                {
                    bool foundField = false;
                    foreach (FieldInfo df in objReflection.DataFields)
                    {
                        if (df.Name.ToLower() == field.Name.ToLower())
                        {
                            string tableName = GetListTableName(df.FieldType);
                            sb.AppendLine(string.Format("INSERT INTO @ListIds SELECT [_Id] FROM [List_{0}] WHERE [_DataObjectTypeId] = {1} AND [ObjectId] = {2} AND [ObjectRevisionId] = {3} AND [_DataObjectFieldId] = {4}",
                                tableName, obj._ObjectType._Id, obj._Id, "@RevisionId", field._Id));
                            sb.AppendLine(string.Format("DELETE FROM [List_{0}Value] WHERE [_Id] IN (SELECT [_Id] FROM @ListIds)", tableName));
                            sb.AppendLine(string.Format("DELETE FROM [List_{0}] WHERE [_Id] IN (SELECT [_Id] FROM @ListIds)", tableName));
                            sb.AppendLine("DELETE FROM @ListIds");

                            foundField = true;
                            break;
                        }
                    }
                    if (!foundField)
                    {
                        foreach (PropertyInfo dp in objReflection.DataProperties)
                        {
                            if (dp.Name == field.Name)
                            {
                                string tableName = GetListTableName(dp.PropertyType);
                                sb.AppendLine(string.Format("INSERT INTO @ListIds SELECT [_Id] FROM [List_{0}] WHERE [_DataObjectTypeId] = {1} AND [ObjectId] = {2} AND [ObjectRevisionId] = {3} AND [_DataObjectFieldId] = {4}",
                                    tableName, obj._ObjectType._Id, obj._Id, "@RevisionId", field._Id));
                                sb.AppendLine(string.Format("DELETE FROM [List_{0}Value] WHERE [_Id] IN (SELECT [_Id] FROM @ListIds)", tableName));
                                sb.AppendLine(string.Format("DELETE FROM [List_{0}] WHERE [_Id] IN (SELECT [_Id] FROM @ListIds)", tableName));
                                sb.AppendLine("DELETE FROM @ListIds");

                                foundField = true;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void WritePrimativeListDelete(StringBuilder sb, IDataObject obj, DataObjectReflection objReflection)
        {
            foreach (DataObjectSchemaField field in objReflection.Schema.Fields)
            {
                if (field.FieldType == DataObjectSchemaFieldType.PrimativeList ||
                    field.FieldType == DataObjectSchemaFieldType.PrimativeArray)
                {
                    bool foundField = false;
                    foreach (FieldInfo df in objReflection.DataFields)
                    {
                        if (df.Name.ToLower() == field.Name.ToLower())
                        {
                            string tableName = GetListTableName(df.FieldType);
                            sb.AppendLine(string.Format("INSERT INTO @ListIds SELECT [_Id] FROM [List_{0}] WHERE [_DataObjectTypeId] = {1} AND [ObjectId] = {2} AND [_DataObjectFieldId] = {3}",
                                tableName, obj._ObjectType._Id, obj._Id, field._Id));
                            sb.AppendLine(string.Format("DELETE FROM [List_{0}Value] WHERE [_Id] IN (SELECT [_Id] FROM @ListIds)", tableName));
                            sb.AppendLine(string.Format("DELETE FROM [List_{0}] WHERE [_Id] IN (SELECT [_Id] FROM @ListIds)", tableName));
                            sb.AppendLine("DELETE FROM @ListIds");

                            foundField = true;
                            break;
                        }
                    }
                    if (!foundField)
                    {
                        foreach (PropertyInfo dp in objReflection.DataProperties)
                        {
                            if (dp.Name == field.Name)
                            {
                                string tableName = GetListTableName(dp.PropertyType);
                                sb.AppendLine(string.Format("INSERT INTO @ListIds SELECT [_Id] FROM [List_{0}] WHERE [_DataObjectTypeId] = {1} AND [ObjectId] = {2} AND [_DataObjectFieldId] = {3}",
                                    tableName, obj._ObjectType._Id, obj._Id, field._Id));
                                sb.AppendLine(string.Format("DELETE FROM [List_{0}Value] WHERE [_Id] IN (SELECT [_Id] FROM @ListIds)", tableName));
                                sb.AppendLine(string.Format("DELETE FROM [List_{0}] WHERE [_Id] IN (SELECT [_Id] FROM @ListIds)", tableName));
                                sb.AppendLine("DELETE FROM @ListIds");

                                foundField = true;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void WriteFieldValue(StringBuilder sb, DataObjectSchemaFieldType fieldType, object value, FieldInfo fInfo, PropertyInfo pInfo, IDataObject parent)
        {
            if (fieldType == DataObjectSchemaFieldType.DateTime ||
                fieldType == DataObjectSchemaFieldType.Guid ||
                fieldType == DataObjectSchemaFieldType.NullableDateTime ||
                fieldType == DataObjectSchemaFieldType.NullableGuid ||
                fieldType == DataObjectSchemaFieldType.String)
            {
                if (value == null)
                    sb.Append("NULL");
                else
                {
                    if (fieldType == DataObjectSchemaFieldType.DateTime)
                    {
                        if ((DateTime)value <= DataManager.MinDate)
                        {
                            sb.Append("NULL");
                        }
                        else
                        {
                            sb.Append("'");
                            sb.Append(value.ToString().Replace("'", "''"));
                            sb.Append("'");
                        }
                    }
                    else if (fieldType == DataObjectSchemaFieldType.NullableDateTime)
                    {
                        if (((DateTime?)value).HasValue == false || ((DateTime?)value).Value <= DataManager.MinDate)
                        {
                            sb.Append("NULL");
                        }
                        else
                        {
                            sb.Append("'");
                            sb.Append(value.ToString().Replace("'", "''"));
                            sb.Append("'");
                        }
                    }
                    else
                    {
                        sb.Append("'");
                        sb.Append(value.ToString().Replace("'", "''"));
                        sb.Append("'");
                    }
                }
            }
            else
            {
                if (pInfo != null)
                {
                    if (pInfo.PropertyType.IsEnum)
                        sb.Append(GetEnumValue(pInfo, parent).ToString());
                    else
                        sb.Append(value.ToString().Replace("True", "1").Replace("False", "0"));
                }
                else
                {
                    if (fInfo.FieldType.IsEnum)
                        sb.Append(GetEnumValue(fInfo, parent).ToString());
                    else
                        sb.Append(value.ToString().Replace("True", "1").Replace("False", "0"));
                }
            }
        }

        private string GetListTableName(Type type)
        {
            string table = null;
            if (type.IsGenericType)
            {
                Type listType = type.GetGenericArguments()[0];
                if (listType.IsEnum)
                    table = Enum.GetUnderlyingType(listType).Name;
                else
                    table = listType.Name;
            }
            else if (type.IsArray)
            {
                Type arrayType = ScriptCommon.GetArrayType(type);
                if (arrayType.IsEnum)
                    table = Enum.GetUnderlyingType(arrayType).Name;
                else
                    table = arrayType.Name;
            }
            return table;
        }

        private object GetEnumValue(object enumValue)
        {
            switch (Enum.GetUnderlyingType(enumValue.GetType()).FullName)
            {
                case "System.Byte":
                    return (byte)Enum.Parse(enumValue.GetType(), enumValue.ToString());
                case "System.Int16":
                    return (short)Enum.Parse(enumValue.GetType(), enumValue.ToString());
                case "System.Int32":
                    return (int)Enum.Parse(enumValue.GetType(), enumValue.ToString());
                default:
                    return (long)Enum.Parse(enumValue.GetType(), enumValue.ToString());
            }
        }

        private object GetEnumValue(PropertyInfo enumProperty, IDataObject obj)
        {
            switch (Enum.GetUnderlyingType(enumProperty.PropertyType).FullName)
            {
                case "System.Byte":
                    return (byte)Enum.Parse(enumProperty.PropertyType, enumProperty.GetValue(obj, null).ToString());
                case "System.Int16":
                    return (short)Enum.Parse(enumProperty.PropertyType, enumProperty.GetValue(obj, null).ToString());
                case "System.Int32":
                    return (int)Enum.Parse(enumProperty.PropertyType, enumProperty.GetValue(obj, null).ToString());
                default:
                    return (long)Enum.Parse(enumProperty.PropertyType, enumProperty.GetValue(obj, null).ToString());
            }
        }

        private object GetEnumValue(FieldInfo enumField, IDataObject obj)
        {
            switch (Enum.GetUnderlyingType(enumField.FieldType).FullName)
            {
                case "System.Byte":
                    return (byte)Enum.Parse(enumField.FieldType, enumField.GetValue(obj).ToString());
                case "System.Int16":
                    return (short)Enum.Parse(enumField.FieldType, enumField.GetValue(obj).ToString());
                case "System.Int32":
                    return (int)Enum.Parse(enumField.FieldType, enumField.GetValue(obj).ToString());
                default:
                    return (long)Enum.Parse(enumField.FieldType, enumField.GetValue(obj).ToString());
            }
        }

        #endregion Methods
    }
}
