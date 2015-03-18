using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataRelational.Search;
using System.Data;
using DataRelational.Cache;
using System.Data.Common;
using DataRelational.Script;
using DataRelational.Schema;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DataRelational.DataReaders
{
    /// <summary>
    /// A data reader for the T-Sql standard.
    /// </summary>
    class TSqlDataReader : DataRelational.DataReaders.IDataReader
    {
        #region IDataReader Members

        /// <summary>
        /// Begins a batch read process.
        /// </summary>
        /// <param name="builder">The string build to write to.</param>
        public void BeginRead(StringBuilder builder)
        {
            builder.AppendLine("DECLARE @DataKeys Table([_Id] BigInt, [_RevisionId] Int, [_ObjectTypeId] SmallInt)");
        }

        /// <summary>
        /// Adds a get query to the batch read process.
        /// </summary>
        /// <param name="builder">The string builder to write to.</param>
        /// <param name="context">The context of the search.</param>
        public void AddGetObjectQuery(StringBuilder builder, SearchContext context)
        {
            ComplexSearchElement timeStampElement = context.InitialSearch.Sections.Find("_Timestamp");
            DataObjectReflection objectReflection = null;
            string mainTable = null;

            if (context.RetrievedObjects.Count == 0)
            {
                foreach (DataKey key in context.FirstLevel)
                {
                    builder.AppendLine(string.Format("INSERT INTO @DataKeys([_Id], [_RevisionId], [_ObjectTypeId]) VALUES({0}, {1}, {2})", key.Id, key.RevisionId, key.TypeId));
                }

                objectReflection = GetObjectReflection(context.InitialSearch.SearchObjectType);

                mainTable = GetMainTableNameFromContext(context, objectReflection.Schema.ObjectType);
                builder.Append("SELECT obj.[_Id], obj.[_RevisionId], obj.[_Deleted], obj.[_ObjectTypeId]");
                foreach (DataObjectSchemaField field in objectReflection.Schema.Fields)
                {
                    if ((byte)field.FieldType < 21)
                    {
                        builder.Append(string.Format(",obj.[{0}] ", field.Name));
                    }
                }
                builder.Append(",obj.[_Timestamp]");
                builder.AppendLine();
                builder.AppendLine(string.Format("  FROM [{0}] obj", mainTable));
                builder.AppendLine("  JOIN @DataKeys dkey ON obj.[_Id] = dkey.[_Id] AND obj.[_RevisionId] = dkey.[_RevisionId]");

                if (context.InitialSearch.IncludeArchived)
                {
                    builder.AppendLine("UNION");
                    builder.Append("SELECT obj.[_Id], obj.[_RevisionId], obj.[_Deleted], obj.[_ObjectTypeId]");
                    foreach (DataObjectSchemaField field in objectReflection.Schema.Fields)
                    {
                        if ((byte)field.FieldType < 21)
                        {
                            builder.Append(string.Format(",obj.[{0}] ", field.Name));
                        }
                    }
                    builder.Append(",obj.[_Timestamp]");
                    builder.AppendLine();
                    builder.AppendLine(string.Format("  FROM [{0}] obj", objectReflection.Schema.TableName + "_Archive"));
                    builder.AppendLine("  JOIN @DataKeys dkey ON obj.[_Id] = dkey.[_Id] AND obj.[_RevisionId] = dkey.[_RevisionId]");
                }

                //Write the primative list gets joined to the @DataKeys table
                WriteGetPrimativeLists(builder, objectReflection, "@DataKeys");
            }

            Dictionary<short, List<long>> retrieveHash = GetRetrievedRelationshipsHash(context);
            foreach (short objectTypeId in retrieveHash.Keys)
            {
                builder.AppendLine("DELETE FROM @DataKeys");
                foreach (long objectId in retrieveHash[objectTypeId])
                {
                    builder.AppendLine(string.Format("INSERT INTO @DataKeys([_Id], [_RevisionId], [_ObjectTypeId]) VALUES({0}, NULL, NULL)", objectId));
                }

                objectReflection = GetObjectReflection(objectTypeId);

                mainTable = GetMainTableNameFromContext(context, objectReflection.Schema.ObjectType);

                string objectTable = "@" + mainTable;
                builder.Append(string.Format("DECLARE {0} Table([_Id] BigInt, [_RevisionId] Int, [_Deleted] Bit, [_ObjectTypeId] SmallInt,", objectTable));
                foreach (DataObjectSchemaField field in objectReflection.Schema.Fields)
                {
                    if ((byte)field.FieldType < 21)
                    {
                        DataObjectSchemaFactory.CreateTableField(builder, field);
                    }
                }
                builder.Append("[_Timestamp] DateTime");
                builder.AppendLine(")");

                builder.Append(string.Format("INSERT INTO {0}([_Id], [_RevisionId], [_Deleted], [_ObjectTypeId]", objectTable));
                foreach (DataObjectSchemaField field in objectReflection.Schema.Fields)
                {
                    if ((byte)field.FieldType < 21)
                    {
                        builder.Append(string.Format(",[{0}] ", field.Name));
                    }
                }
                builder.AppendLine(",[_Timestamp])");

                builder.Append("SELECT obj.[_Id], obj.[_RevisionId], obj.[_Deleted], obj.[_ObjectTypeId]");
                foreach (DataObjectSchemaField field in objectReflection.Schema.Fields)
                {
                    if ((byte)field.FieldType < 21)
                    {
                        builder.Append(string.Format(",obj.[{0}] ", field.Name));
                    }
                }
                builder.Append(",obj.[_Timestamp]");
                builder.AppendLine(string.Format("  FROM [{0}] obj", mainTable));
                builder.AppendLine("  JOIN @DataKeys dkey ON obj.[_Id] = dkey.[_Id]");
                builder.Append(string.Format(" WHERE obj._RevisionId = (SELECT Max([_RevisionId]) FROM [{0}] WHERE [_Id] = obj.[_Id] ", mainTable));
                if (timeStampElement != null)
                    builder.Append(string.Format("AND [_Timestamp] < '{0}'", timeStampElement.Value));
                builder.AppendLine(")");

                if (context.InitialSearch.IncludeArchived)
                {
                    builder.AppendLine("UNION");
                    builder.Append("SELECT obj.[_Id], obj.[_RevisionId], obj.[_Deleted], obj.[_ObjectTypeId]");
                    foreach (DataObjectSchemaField field in objectReflection.Schema.Fields)
                    {
                        if ((byte)field.FieldType < 21)
                        {
                            builder.Append(string.Format(",obj.[{0}] ", field.Name));
                        }
                    }
                    builder.Append(",obj.[_Timestamp]");
                    builder.AppendLine(string.Format("  FROM [{0}] obj", objectReflection.Schema.TableName + "_Archive"));
                    builder.AppendLine("  JOIN @DataKeys dkey ON obj.[_Id] = dkey.[_Id]");
                    builder.Append(string.Format(" WHERE obj._RevisionId = (SELECT Max([_RevisionId]) FROM [{0}] WHERE [_Id] = obj.[_Id] ", objectReflection.Schema.TableName + "_Archive"));
                    if (timeStampElement != null)
                        builder.Append(string.Format("AND [_Timestamp] < '{0}'", timeStampElement.Value));
                    builder.AppendLine(")");
                }

                builder.Append("SELECT [_Id], [_RevisionId], [_Deleted], [_ObjectTypeId]");
                foreach (DataObjectSchemaField field in objectReflection.Schema.Fields)
                {
                    if ((byte)field.FieldType < 21)
                    {
                        builder.Append(string.Format(",[{0}] ", field.Name));
                    }
                }
                builder.AppendLine(",[_Timestamp]");
                builder.AppendLine("  FROM " + objectTable);

                //Write the primative list gets joined to the objectTable table
                WriteGetPrimativeLists(builder, objectReflection, objectTable);
            }
        }

        /// <summary>
        /// Ends a batch read process and executes the batch.
        /// </summary>
        /// <param name="builder">The string build to write to.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>A data set containing the queried objects.  The data set will be formatted as Tables: {TypeName1, TypeName2, ... Relationships}</returns>
        public DataSet EndRead(StringBuilder builder, string connectionName)
        {
            DataSet result = new DataSet();
            using (DbConnection connection = DataManager.ScriptManager.GetDatabaseConnection(connectionName))
            {
                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = builder.ToString();
                    cmd.CommandType = System.Data.CommandType.Text;
                    try
                    {
                        DbDataAdapter adapter = DataManager.ScriptManager.GetDataAdapter();
                        adapter.SelectCommand = cmd;
                        DateTime start = DateTime.Now;
                        adapter.Fill(result);
                        DateTime end = DateTime.Now;
                        TimeSpan ts = (end - start);
                        Logging.Information(string.Format("Database read time - {0} seconds {1} milliseconds", ts.Seconds, ts.Milliseconds));
                    }
                    catch (Exception ex)
                    {
                        throw new DataRelationalException("An error occured while attempting to read data.", ex);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the next three levels of relationships.
        /// </summary>
        /// <param name="context">The context of the search.</param>
        /// <param name="connectionName">The name of the connection to read from.</param>
        /// <returns>A search context with the next three levels of relationships.</returns>
        public SearchContext GetRelationships(SearchContext context, string connectionName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("DECLARE @Relationship TABLE");
            sb.AppendLine("(");
            sb.AppendLine("   [ParentId] [bigint],");
            sb.AppendLine("   [ParentTypeId] [smallint],");
            sb.AppendLine("   [ParentFieldId] [int],");
            sb.AppendLine("   [ChildId] [bigint],");
            sb.AppendLine("   [ChildTypeId] [smallint],");
            sb.AppendLine("   [BeginDate] [datetime],");
            sb.AppendLine("   [EndDate] [datetime],");
            sb.AppendLine("   [Level] [int]");
            sb.AppendLine(")");
            sb.AppendLine();

            if (context.FirstLevel.Count == 0)
            {
                //Handle get first level relationships
                if (context.InitialSearch.SearchMode == ComplexSearchMode.GetSingle || context.InitialSearch.SearchMode == ComplexSearchMode.GetMultiple)
                {
                    WriteGetFirstLevelRelationships(sb, context);
                }
                else if (context.InitialSearch.SearchMode == ComplexSearchMode.Search)
                {
                    WriteGetFirstLevelSearchRelationships(sb, context);
                }
                else
                {
                    //TODO: Write a method that builds the first level get object history query
                }
            }
            else
            {
                //Handle get nth level relationships
                if (context.InitialSearch.SearchMode == ComplexSearchMode.GetSingle ||
                        context.InitialSearch.SearchMode == ComplexSearchMode.GetMultiple ||
                        context.InitialSearch.SearchMode == ComplexSearchMode.Search)
                {
                    WriteGetNthLevelRelationships(sb, context);
                }
                else
                {
                    //TODO: Write a method that builds the nth level get object history query
                }
            }

            sb.AppendLine("SELECT DISTINCT * FROM @Relationship");

            using (DbConnection connection = DataManager.ScriptManager.GetDatabaseConnection(connectionName))
            {
                using (DbCommand cmd = connection.CreateCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sb.ToString();
                        DateTime start = DateTime.Now;
                        using (DbDataReader reader = cmd.ExecuteReader())
                        {
                            DateTime end = DateTime.Now;
                            TimeSpan ts = (end - start);
                            Logging.Information(string.Format("Database read time - {0} seconds {1} milliseconds", ts.Seconds, ts.Milliseconds));

                            bool workToDo = true;
                            if (context.loadedDepth == 0)
                            {
                                while (reader.Read())
                                {
                                    DataKey key = new DataKey(reader.GetInt64(0), reader.GetInt32(1), reader.GetInt16(2));
                                    context.FirstLevel.Add(key);
                                }
                                workToDo = reader.NextResult();
                            }
                            if (workToDo)
                            {
                                while (reader.Read())
                                {
                                    Relationship relationship = new Relationship(reader.GetInt64(0), reader.GetInt16(1), reader.GetInt32(2), reader.GetInt64(3), reader.GetInt16(4), reader.GetDateTime(5), reader.GetDateTime(6));
                                    List<Relationship> levelRelationships = null;
                                    if (!context.Relationships.TryGetValue(reader.GetInt32(7), out levelRelationships))
                                    {
                                        levelRelationships = new List<Relationship>();
                                        context.Relationships.Add(reader.GetInt32(7), levelRelationships);
                                    }

                                    bool containsRelationship = false;
                                    foreach (List<Relationship> relationships in context.Relationships.Values)
                                    {
                                        foreach (Relationship check in relationships)
                                        {
                                            if (check.ParentId == relationship.ParentId &&
                                                check.ParentTypeId == relationship.ParentTypeId &&
                                                check.ParentFieldId == relationship.ParentFieldId &&
                                                check.ChildId == relationship.ChildId &&
                                                check.ChildTypeId == relationship.ChildTypeId &&
                                                check.BeginDate == relationship.BeginDate &&
                                                check.EndDate == relationship.EndDate)
                                            {
                                                containsRelationship = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (!containsRelationship)
                                        levelRelationships.Add(relationship);
                                }
                            }
                            reader.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new ScriptException("Could not get the next three levels of relationships.", ex);
                    }
                }
            }
            return context;
        }

        #endregion

        private void WriteGetFirstLevelRelationships(StringBuilder sb, SearchContext context)
        {
            sb.AppendLine("INSERT INTO @Relationship");
            sb.AppendLine("   SELECT [ParentId]");
            sb.AppendLine("         ,[ParentTypeId]");
            sb.AppendLine("         ,[ParentFieldId]");
            sb.AppendLine("         ,[ChildId]");
            sb.AppendLine("         ,[ChildTypeId]");
            sb.AppendLine("         ,[BeginDate]");
            sb.AppendLine("         ,[EndDate]");
            sb.AppendLine("         ,1");
            sb.AppendLine("     FROM [DataObjectRelationship]");
            ComplexSearchElement idElement = context.InitialSearch.Sections.Find("_Id");
            if (idElement == null)
                throw new ScriptException("The condition _Id must always exists if the SearchMode is ComplexSearchMode.GetSingle");
            ComplexSearchElement objectTypeElement = context.InitialSearch.Sections.Find("_ObjectTypeId");
            if (objectTypeElement == null)
                throw new ScriptException("The condition _ObjectTypeId must always exists if the SearchMode is ComplexSearchMode.GetSingle");
            if (context.InitialSearch.SearchMode == ComplexSearchMode.GetSingle)
            {
                sb.Append(string.Format("    WHERE [ParentId] = {0}", idElement.Value));
            }
            else if (context.InitialSearch.SearchMode == ComplexSearchMode.GetMultiple)
            {
                StringBuilder idBuilder = new StringBuilder();
                foreach (long id in idElement.Values)
                {
                    if (idBuilder.Length > 0)
                        idBuilder.Append(",");
                    idBuilder.Append(id);
                }
                if (idBuilder.Length == 0)
                    sb.Append("    WHERE 1=1");
                else
                    sb.Append(string.Format("    WHERE [ParentId] IN ({0})", idBuilder.ToString()));
            }
            sb.AppendLine(string.Format("      AND [ParentTypeId] = {0}", objectTypeElement.Value));
            ComplexSearchElement timeStampElement = context.InitialSearch.Sections.Find("_Timestamp");
            if (timeStampElement != null)
            {
                sb.AppendLine(string.Format("      AND [BeginDate] <= '{0}'", timeStampElement.Value));
                sb.AppendLine(string.Format("      AND [EndDate] > '{0}'", timeStampElement.Value));
            }
            else
            {
                sb.AppendLine("      AND [BeginDate] <= GetUTCDate()");
                sb.AppendLine("      AND [EndDate] > GetUTCDate()");
            }

            sb.AppendLine();
            sb.AppendLine("INSERT INTO @Relationship");
            sb.AppendLine("   SELECT DISTINCT R.[ParentId]");
            sb.AppendLine("         ,R.[ParentTypeId]");
            sb.AppendLine("         ,R.[ParentFieldId]");
            sb.AppendLine("         ,R.[ChildId]");
            sb.AppendLine("         ,R.[ChildTypeId]");
            sb.AppendLine("         ,R.[BeginDate]");
            sb.AppendLine("         ,R.[EndDate]");
            sb.AppendLine("         ,2");
            sb.AppendLine("     FROM [DataObjectRelationship] R");
            sb.AppendLine("     JOIN @Relationship Level1");
            sb.AppendLine("       ON Level1.[ChildId] = R.ParentId");
            sb.AppendLine("      AND Level1.[ChildTypeId] = R.ParentTypeId");
            sb.AppendLine("      AND Level1.Level = 1");
            if (timeStampElement != null)
            {
                sb.AppendLine(string.Format("    WHERE R.[BeginDate] <= '{0}'", timeStampElement.Value));
                sb.AppendLine(string.Format("      AND R.[EndDate] > '{0}'", timeStampElement.Value));
            }
            else
            {
                sb.AppendLine("      AND R.[BeginDate] <= GetUTCDate()");
                sb.AppendLine("      AND R.[EndDate] > GetUTCDate()");
            }
            sb.AppendLine();
            sb.AppendLine("SELECT [_Id], [_RevisionId], [_ObjectTypeId]");

            string mainTable = GetMainTableNameFromContext(context, context.InitialSearch.SearchObjectType);
            
            sb.AppendLine(string.Format("  FROM [{0}] obj", mainTable));
            if (context.InitialSearch.SearchMode == ComplexSearchMode.GetSingle)
            {
                sb.Append(string.Format("    WHERE [_Id] = {0}", idElement.Value));
            }
            else if (context.InitialSearch.SearchMode == ComplexSearchMode.GetMultiple)
            {
                StringBuilder idBuilder = new StringBuilder();
                foreach (long id in idElement.Values)
                {
                    if (idBuilder.Length > 0)
                        idBuilder.Append(",");
                    idBuilder.Append(id);
                }
                if (idBuilder.Length == 0)
                    sb.Append("    WHERE 1=1");
                else
                    sb.Append(string.Format("    WHERE [_Id] IN ({0})", idBuilder.ToString()));
            }
            sb.AppendLine(string.Format("   AND [_RevisionId] = (SELECT Max([_RevisionId]) FROM [{0}] WHERE [_Id] = obj.[_Id]{1})", mainTable, (timeStampElement == null ? "" : " AND [_TimeStamp] < '" + timeStampElement.Value + "'")));
            if (!context.InitialSearch.IncludeDeleted)
                sb.AppendLine(string.Format("   AND [_Deleted] = 0"));

            if (context.InitialSearch.IncludeArchived)
            {
                sb.AppendLine("UNION");
                sb.AppendLine("SELECT [_Id], [_RevisionId], [_ObjectTypeId]");

                string archiveTable = context.InitialSearch.SearchObjectType.Name + "_Archive";

                sb.AppendLine(string.Format("  FROM [{0}] obj", archiveTable));
                if (context.InitialSearch.SearchMode == ComplexSearchMode.GetSingle)
                {
                    sb.Append(string.Format("    WHERE [_Id] = {0}", idElement.Value));
                }
                else if (context.InitialSearch.SearchMode == ComplexSearchMode.GetMultiple)
                {
                    StringBuilder idBuilder = new StringBuilder();
                    foreach (long id in idElement.Values)
                    {
                        if (idBuilder.Length > 0)
                            idBuilder.Append(",");
                        idBuilder.Append(id);
                    }
                    if (idBuilder.Length == 0)
                        sb.Append("    WHERE 1=1");
                    else
                        sb.Append(string.Format("    WHERE [_Id] IN ({0})", idBuilder.ToString()));
                }
                sb.AppendLine(string.Format("   AND [_RevisionId] = (SELECT Max([_RevisionId]) FROM [{0}] WHERE [_Id] = obj.[_Id]{1})", archiveTable, (timeStampElement == null ? "" : " AND [_TimeStamp] < '" + timeStampElement.Value + "'")));
                ComplexSearchElement deletedElement = context.InitialSearch.Sections.Find("_Deleted");
                if(deletedElement != null)
                    sb.AppendLine(string.Format("   AND [_Deleted] = {0}", deletedElement.Value));
                else if (!context.InitialSearch.IncludeDeleted)
                    sb.AppendLine(string.Format("   AND [_Deleted] = 0"));
            }
        }

        private void WriteGetFirstLevelSearchRelationships(StringBuilder sb, SearchContext context)
        {
            ComplexSearchElement objectTypeElement = context.InitialSearch.Sections.Find("_ObjectTypeId");
            if (objectTypeElement == null)
                throw new ScriptException("The condition _ObjectTypeId must always exists if the SearchMode is ComplexSearchMode.Search");

            List<string> joinNames = new List<string>();
            ComplexSearchElement timeStampElement = context.InitialSearch.Sections.Find("_Timestamp");
            string mainTable = GetMainTableNameFromContext(context, context.InitialSearch.SearchObjectType);

            sb.AppendLine("DECLARE @Objects TABLE( [_Id] BigInt, [_RevisionId] Int, [_ObjectTypeId] SmallInt )");
            sb.AppendLine("INSERT INTO @Objects");
            sb.AppendLine("   SELECT obj.[_Id], obj.[_RevisionId], obj.[_ObjectTypeId]");
            sb.AppendLine(string.Format("     FROM [{0}] AS obj", mainTable));
            WriteJoins(sb, context, timeStampElement, ref joinNames);
            WriteWhere(sb, context, timeStampElement, ref joinNames, "obj");

            sb.Append(string.Format("      AND obj._RevisionId = (SELECT Max([_RevisionId]) FROM [{0}] WHERE [_Id] = obj.[_Id] ", mainTable));
            if (timeStampElement != null)
                sb.Append(string.Format("AND [_Timestamp] < '{0}'", timeStampElement.Value));
            sb.AppendLine(")");

            if (context.InitialSearch.IncludeArchived)
            {
                joinNames.Clear();
                string archiveTable = context.InitialSearch.SearchObjectType.Name + "_Archive";
                sb.AppendLine("   UNION");
                sb.AppendLine("   SELECT obj.[_Id], obj.[_RevisionId], obj.[_ObjectTypeId]");
                sb.AppendLine(string.Format("     FROM [{0}] AS obj", archiveTable));
                WriteJoins(sb, context, timeStampElement, ref joinNames);
                WriteWhere(sb, context, timeStampElement, ref joinNames, "obj");

                sb.Append(string.Format("      AND obj._RevisionId = (SELECT Max([_RevisionId]) FROM [{0}] WHERE [_Id] = obj.[_Id] ", archiveTable));
                if (timeStampElement != null)
                    sb.Append(string.Format("AND [_Timestamp] < '{0}'", timeStampElement.Value));
                sb.AppendLine(")");
            }

            sb.AppendLine("INSERT INTO @Relationship");
            sb.AppendLine("   SELECT [ParentId]");
            sb.AppendLine("         ,[ParentTypeId]");
            sb.AppendLine("         ,[ParentFieldId]");
            sb.AppendLine("         ,[ChildId]");
            sb.AppendLine("         ,[ChildTypeId]");
            sb.AppendLine("         ,[BeginDate]");
            sb.AppendLine("         ,[EndDate]");
            sb.AppendLine("         ,1");
            sb.AppendLine("     FROM [DataObjectRelationship] R");
            sb.AppendLine("    WHERE [ParentId] IN (SELECT [_Id] FROM @Objects)");
            sb.AppendLine(string.Format("      AND [ParentTypeId] = {0}", objectTypeElement.Value));
            if (timeStampElement != null)
            {
                sb.AppendLine(string.Format("      AND R.[BeginDate] <= '{0}'", timeStampElement.Value));
                sb.AppendLine(string.Format("      AND R.[EndDate] > '{0}'", timeStampElement.Value));
            }
            else
            {
                sb.AppendLine("      AND R.[BeginDate] <= GetUTCDate()");
                sb.AppendLine("      AND R.[EndDate] > GetUTCDate()");
            }

            sb.AppendLine();
            sb.AppendLine("INSERT INTO @Relationship");
            sb.AppendLine("   SELECT DISTINCT R.[ParentId]");
            sb.AppendLine("         ,R.[ParentTypeId]");
            sb.AppendLine("         ,R.[ParentFieldId]");
            sb.AppendLine("         ,R.[ChildId]");
            sb.AppendLine("         ,R.[ChildTypeId]");
            sb.AppendLine("         ,R.[BeginDate]");
            sb.AppendLine("         ,R.[EndDate]");
            sb.AppendLine("         ,2");
            sb.AppendLine("     FROM [DataObjectRelationship] R");
            sb.AppendLine("     JOIN @Relationship Level1");
            sb.AppendLine("       ON Level1.[ChildId] = R.ParentId");
            sb.AppendLine("      AND Level1.[ChildTypeId] = R.ParentTypeId");
            sb.AppendLine("      AND Level1.Level = 1");
            if (timeStampElement != null)
            {
                sb.AppendLine(string.Format("    WHERE R.[BeginDate] <= '{0}'", timeStampElement.Value));
                sb.AppendLine(string.Format("      AND R.[EndDate] > '{0}'", timeStampElement.Value));
            }
            else
            {
                sb.AppendLine("      AND R.[BeginDate] <= GetUTCDate()");
                sb.AppendLine("      AND R.[EndDate] > GetUTCDate()");
            }

            sb.AppendLine();
            sb.AppendLine("SELECT [_Id], [_RevisionId], [_ObjectTypeId]");
            sb.AppendLine("  FROM @Objects");
        }

        private void WriteGetNthLevelRelationships(StringBuilder sb, SearchContext context)
        {
            //Make sure we actually have more depth to load before wasting database time
            int depthLevel = -1;
            if (context.Relationships.Count > 0 && context.Relationships.ContainsKey(context.loadedDepth + depthLevel))
            {
                List<Relationship> relationships = null;
                if (!context.Relationships.TryGetValue(context.loadedDepth + depthLevel, out relationships))
                {
                    //This should never happen sense we already checked for the key above.
                    throw new DataRelationalException("Requested Nth level relationships for relationships that do not exists in the search context.");
                }

                sb.AppendLine("INSERT INTO @Relationship");
                sb.AppendLine("   SELECT [ParentId]");
                sb.AppendLine("         ,[ParentTypeId]");
                sb.AppendLine("         ,[ParentFieldId]");
                sb.AppendLine("         ,[ChildId]");
                sb.AppendLine("         ,[ChildTypeId]");
                sb.AppendLine("         ,[BeginDate]");
                sb.AppendLine("         ,[EndDate]");
                sb.AppendLine(string.Format("         ,{0}", (context.loadedDepth + depthLevel + 1)));
                sb.AppendLine("     FROM [DataObjectRelationship]");

                StringBuilder parentIdBuilder = new StringBuilder();
                List<long> childIds = new List<long>();
                foreach (Relationship r in relationships)
                {
                    if (childIds.Contains(r.ChildId) == false)
                    {
                        if (parentIdBuilder.Length > 0)
                            parentIdBuilder.Append(",");
                        parentIdBuilder.Append(r.ChildId);
                        childIds.Add(r.ChildId);
                    }
                }
                if (parentIdBuilder.Length == 0)
                    parentIdBuilder.Append("0");

                sb.Append(string.Format("    WHERE [ParentId] IN ({0})", parentIdBuilder.ToString()));
                ComplexSearchElement timeStampElement = context.InitialSearch.Sections.Find("_Timestamp");
                if (timeStampElement != null)
                {
                    sb.AppendLine(string.Format("      AND [BeginDate] <= '{0}'", timeStampElement.Value));
                    sb.AppendLine(string.Format("      AND [EndDate] > '{0}'", timeStampElement.Value));
                }
                else
                {
                    sb.AppendLine("      AND [BeginDate] <= GetUTCDate()");
                    sb.AppendLine("      AND [EndDate] > GetUTCDate()");
                }

                for (depthLevel = depthLevel + 1; depthLevel < 2; depthLevel++)
                {
                    sb.AppendLine();
                    sb.AppendLine("INSERT INTO @Relationship");
                    sb.AppendLine("   SELECT DISTINCT R.[ParentId]");
                    sb.AppendLine("         ,R.[ParentTypeId]");
                    sb.AppendLine("         ,R.[ParentFieldId]");
                    sb.AppendLine("         ,R.[ChildId]");
                    sb.AppendLine("         ,R.[ChildTypeId]");
                    sb.AppendLine("         ,R.[BeginDate]");
                    sb.AppendLine("         ,R.[EndDate]");
                    sb.AppendLine(string.Format("         ,{0}", (context.loadedDepth + depthLevel + 1)));
                    sb.AppendLine("     FROM [DataObjectRelationship] R");
                    sb.AppendLine("     JOIN @Relationship Level1");
                    sb.AppendLine("       ON Level1.[ChildId] = R.ParentId");
                    sb.AppendLine("      AND Level1.[ChildTypeId] = R.ParentTypeId");
                    sb.AppendLine(string.Format("      AND Level1.Level = {0}", (context.loadedDepth + depthLevel)));
                    if (timeStampElement != null)
                    {
                        sb.AppendLine(string.Format("    WHERE R.[BeginDate] <= '{0}'", timeStampElement.Value));
                        sb.AppendLine(string.Format("      AND R.[EndDate] > '{0}'", timeStampElement.Value));
                    }
                    else
                    {
                        sb.AppendLine("      AND R.[BeginDate] <= GetUTCDate()");
                        sb.AppendLine("      AND R.[EndDate] > GetUTCDate()");
                    }
                }
            }
        }

        private void WriteGetPrimativeLists(StringBuilder sb, DataObjectReflection objectReflection, string joinTable)
        {
            bool lBool = false, lByte = false, lShort = false, lInt = false, lLong = false, lDouble = false, lFloat = false, lDecimal = false, lDateTime = false, lGuid = false, lString = false;

            foreach (DataObjectSchemaField field in objectReflection.Schema.Fields)
            {
                if (field.FieldType == DataObjectSchemaFieldType.PrimativeArray || field.FieldType == DataObjectSchemaFieldType.PrimativeList)
                {
                    Type listType = null;
                    foreach (FieldInfo fld in objectReflection.DataFields)
                    {
                        if (fld.Name.ToLower() == field.Name.ToLower())
                        {
                            if (ScriptCommon.GetArrayType(fld.FieldType) != null && (ScriptCommon.PrimativeTypes.Contains(ScriptCommon.GetArrayType(fld.FieldType)) || ScriptCommon.GetArrayType(fld.FieldType).IsEnum))
                            {
                                listType = ScriptCommon.GetArrayType(fld.FieldType);
                                break;
                            }
                            else if (fld.FieldType.IsGenericType && fld.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                            {
                                if (ScriptCommon.PrimativeTypes.Contains(fld.FieldType.GetGenericArguments()[0]) || fld.FieldType.GetGenericArguments()[0].IsEnum)
                                {
                                    listType = fld.FieldType.GetGenericArguments()[0];
                                    break;
                                }
                            }
                        }
                    }
                    if (listType == null)
                    {
                        foreach (PropertyInfo prop in objectReflection.DataProperties)
                        {
                            if (prop.Name.ToLower() == field.Name.ToLower())
                            {
                                if (ScriptCommon.GetArrayType(prop.PropertyType) != null && (ScriptCommon.PrimativeTypes.Contains(ScriptCommon.GetArrayType(prop.PropertyType)) || ScriptCommon.GetArrayType(prop.PropertyType).IsEnum))
                                {
                                    listType = ScriptCommon.GetArrayType(prop.PropertyType);
                                    break;
                                }
                                else if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                                {
                                    if (ScriptCommon.PrimativeTypes.Contains(prop.PropertyType.GetGenericArguments()[0]) || prop.PropertyType.GetGenericArguments()[0].IsEnum)
                                    {
                                        listType = prop.PropertyType.GetGenericArguments()[0];
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (listType != null)
                    {
                        switch(listType.FullName)
                        {
                            case "System.Boolean":
                                if (!lBool)
                                {
                                    WriteGetPrimativeList(sb, "List_Boolean", joinTable);
                                    lBool = true;
                                }
                                break;
                            case "System.Byte":
                                if (!lByte)
                                {
                                    WriteGetPrimativeList(sb, "List_Byte", joinTable);
                                    lByte = true;
                                }
                                break;
                            case "System.Int16":
                                if (!lShort)
                                {
                                    WriteGetPrimativeList(sb, "List_Int16", joinTable);
                                    lShort = true;
                                }
                                break;
                            case "System.Int32":
                                if (!lInt)
                                {
                                    WriteGetPrimativeList(sb, "List_Int32", joinTable);
                                    lInt = true;
                                }
                                break;
                            case "System.Int64":
                                if (!lLong)
                                {
                                    WriteGetPrimativeList(sb, "List_Int64", joinTable);
                                    lLong = true;
                                }
                                break;
                            case "System.Double":
                                if (!lDouble)
                                {
                                    WriteGetPrimativeList(sb, "List_Double", joinTable);
                                    lDouble = true;
                                }
                                break;
                            case "System.Single":
                                if (!lFloat)
                                {
                                    WriteGetPrimativeList(sb, "List_Single", joinTable);
                                    lFloat = true;
                                }
                                break;
                            case "System.Decimal":
                                if (!lDecimal)
                                {
                                    WriteGetPrimativeList(sb, "List_Decimal", joinTable);
                                    lDecimal = true;
                                }
                                break;
                            case "System.DateTime":
                                if (!lDateTime)
                                {
                                    WriteGetPrimativeList(sb, "List_DateTime", joinTable);
                                    lDateTime = true;
                                }
                                break;
                            case "System.Guid":
                                if (!lGuid)
                                {
                                    WriteGetPrimativeList(sb, "List_Guid", joinTable);
                                    lGuid = true;
                                }
                                break;
                            case "System.String":
                                if (!lString)
                                {
                                    WriteGetPrimativeList(sb, "List_String", joinTable);
                                    lString = true;
                                }
                                break;
                        }
                    }
                }
            }
        }

        private void WriteGetPrimativeList(StringBuilder sb, string listTable, string joinTable)
        {
            sb.AppendLine("SELECT l.[_Id] AS [_ListId], [ObjectId], [ObjectRevisionId], [_DataObjectTypeId], [_DataObjectFieldId], [TypeName], [_Index], [Value]");
            sb.AppendLine(string.Format("  FROM [{0}] l", listTable));
            sb.AppendLine(string.Format("  JOIN {0} dkeys", joinTable));
            sb.AppendLine("    ON l.[_DataObjectTypeId] = dkeys.[_ObjectTypeId]");
            sb.AppendLine("   AND l.[ObjectId] = dkeys.[_Id]");
            sb.AppendLine("   AND l.[ObjectRevisionId] = dkeys.[_RevisionId]");
            sb.AppendLine(string.Format("  JOIN [{0}Value] lv", listTable));
            sb.AppendLine("    ON l.[_Id] = lv.[_Id]");
            sb.AppendLine("  JOIN [DataObjectType] dt");
            sb.AppendLine("    ON l.[_DataObjectTypeId] = dt.[_Id]");
            sb.AppendLine(" ORDER BY [_ListId] ASC, [_Index] ASC");
        }

        private void WriteJoins(StringBuilder sb, SearchContext context, ComplexSearchElement timeStampElement, ref List<string> joinNames)
        {
            DataObjectReflection objectReflection = null;
            foreach (ComplexSearchSection section in context.InitialSearch.Sections)
            {
                foreach (ComplexSearchElement condition in section.Conditions)
                {
                    if (condition.FieldName.Contains("."))
                    {
                        //If the condtion contains a '.' we need to verify the string and add the joins
                        string[] parts = condition.FieldName.Split(".".ToCharArray());
                        objectReflection = GetObjectReflection(context.InitialSearch.SearchObjectType);

                        for (int i = 0; i < parts.Length; i++)
                        {
                            if (i < parts.Length - 1) // Should be a property name that is tied to a data relationship
                            {
                                DataObjectSchemaField field = null;
                                foreach (DataObjectSchemaField f in objectReflection.Schema.Fields)
                                {
                                    if ((byte)f.FieldType > 23 && f.Name.ToLower() == parts[i].ToLower())
                                    {
                                        field = f;
                                        break;
                                    }
                                }
                                if (field == null)
                                {
                                    throw new DataRelationalException(string.Format("Could not find the data relationship '{0}' on the object type '{1}'.", parts[i], objectReflection.Schema.TypeName));
                                }
                                Type fieldType = ScriptCommon.GetRelationshipType(field.Name, objectReflection);
                                if (fieldType == null)
                                {
                                    throw new DataRelationalException(string.Format("Could not find the data relationship '{0}' on the object type '{1}'.", parts[i], objectReflection.Schema.TypeName));
                                }
                                string joinName = "obj" + (joinNames.Count + 1).ToString();
                                joinNames.Add(joinName);
                                if (i == 0)
                                {
                                    WriteJoin(sb, context, timeStampElement, objectReflection.Schema._Id, field._Id, fieldType, "R" + joinNames.Count.ToString(), "obj", joinName);
                                }
                                else
                                {
                                    WriteJoin(sb, context, timeStampElement, objectReflection.Schema._Id, field._Id, fieldType, "R" + joinNames.Count.ToString(), joinNames[joinNames.Count - 2], joinName);
                                }

                                objectReflection = GetObjectReflection(fieldType);
                            }
                            else //Should be a property name that is a data field
                            {
                                bool foundField = false;
                                foreach (DataObjectSchemaField field in objectReflection.Schema.Fields)
                                {
                                    if (field.Name.ToLower() == parts[i].ToLower())
                                    {
                                        foundField = true;
                                        break;
                                    }
                                }
                                if (!foundField)
                                {
                                    throw new DataRelationalException(string.Format("Could not find the data field '{0}' on the object type '{1}'.", parts[i], objectReflection.Schema.TypeName));
                                }
                            }
                        }
                    }
                }
            }
        }

        private void WriteJoin(StringBuilder sb, SearchContext context, ComplexSearchElement timeStampElement, short parentObjectTypeId, int fieldId, Type fieldType, string relAlias, string parentObjAlias, string childObjAlias)
        {
            sb.AppendLine(string.Format("     JOIN [DataObjectRelationship] AS {0}", relAlias));
            sb.AppendLine(string.Format("       ON {0}.[ParentTypeId] = {1}", relAlias, parentObjectTypeId));
            sb.AppendLine(string.Format("      AND {0}.[ParentFieldId] = {1}", relAlias, fieldId));
            sb.AppendLine(string.Format("      AND {0}.[ParentId] = {1}.[_Id]", relAlias, parentObjAlias));
            if (timeStampElement != null)
            {
                sb.AppendLine(string.Format("      AND {0}.[BeginDate] <= '{1}'", relAlias, timeStampElement.Value));
                sb.AppendLine(string.Format("      AND {0}.[EndDate] > '{1}'", relAlias, timeStampElement.Value));
            }
            else
            {
                sb.AppendLine(string.Format("      AND {0}.[BeginDate] <= GetUTCDate()", relAlias));
                sb.AppendLine(string.Format("      AND {0}.[EndDate] > GetUTCDate()", relAlias));
            }

            sb.AppendLine(string.Format("     JOIN [{0}] AS {1}", GetMainTableNameFromContext(context, fieldType), childObjAlias));
            sb.AppendLine(string.Format("       ON {0}.[ChildId] = {1}.[_Id]", relAlias, childObjAlias));
            sb.AppendLine(string.Format("      AND {0}.[ParentFieldId] = {1}", relAlias, fieldId));
            if (context.InitialSearch.IncludeHistory && timeStampElement != null)
            {
                sb.AppendLine(string.Format("      AND {0}.[_RevisionId] = (SELECT MAX(_RevisionId) FROM [{1}] WHERE _Id = {0}._Id AND _TimeStamp < '{2}')", childObjAlias, GetMainTableNameFromContext(context, fieldType), timeStampElement.Value));
            }
        }

        private void WriteWhere(StringBuilder sb, SearchContext context, ComplexSearchElement timeStampElement, ref List<string> joinNames, string parentObjAlias)
        {
            StringBuilder sb2 = new StringBuilder();
            int joinIndex = 0;
            for (int i = 0; i < context.InitialSearch.Sections.Count; i++)
            {
                ComplexSearchSection section = context.InitialSearch.Sections[i];
                if (section.Conditions.Count == 1 && (section.Conditions[0].FieldName == "_ObjectTypeId" || section.Conditions[0].FieldName == "_Timestamp"))
                {
                    continue;
                }
                if (sb2.Length == 0)
                {
                    sb2.Append("    WHERE (");
                    WriteWhereSection(sb2, section, ref joinNames, false, 0, ref joinIndex, parentObjAlias);
                }
                else
                {
                    WriteWhereSection(sb2, section, ref joinNames, true, 0, ref joinIndex, parentObjAlias);
                }
            }
            sb.Append(sb2.ToString());
        }

        private void WriteWhereSection(StringBuilder sb, ComplexSearchSection section, ref List<string> joinNames, bool includeType, int level, ref int joinIndex, string parentObjAlias)
        {
            if (includeType)
            {
                if (section.SearchType == ComplexSearchType.And)
                {
                    if (level == 0)
                        sb.Append("      ");
                    sb.Append("AND (");
                }
                else
                {
                    if (level == 0)
                        sb.Append("       ");
                    sb.Append("OR (");
                }
            }
            bool writeSearchType = false;
            foreach (ComplexSearchElement condition in section.Conditions)
            {
                if (condition.FieldName.Contains(".") == false)
                {
                    sb.Append(condition.ToString(writeSearchType, parentObjAlias));
                    writeSearchType = true;
                }
                else
                {
                    string[] parts = condition.FieldName.Split(".".ToCharArray());
                    joinIndex += parts.Length - 2;
                    sb.Append(condition.ToString(writeSearchType, joinNames[joinIndex]));
                    joinIndex++;
                    writeSearchType = true;
                }
            }
            foreach(ComplexSearchSection childSection in section.Sections)
            {
                WriteWhereSection(sb, childSection, ref joinNames, true, level + 1, ref joinIndex, parentObjAlias);
            }
            sb.AppendLine(")");
        }

        private Dictionary<short, List<long>> GetRetrievedRelationshipsHash(SearchContext context)
        {
            Dictionary<short, List<long>> retrieveHash = new Dictionary<short, List<long>>();
            foreach (List<Relationship> relationships in context.Relationships.Values)
            {
                foreach (Relationship relationship in relationships)
                {
                    bool retrievedObject = false;
                    Dictionary<long, IDataObject> childHash = null;
                    if (context.RetrievedObjects.TryGetValue(relationship.ChildTypeId, out childHash))
                    {
                        if (childHash.ContainsKey(relationship.ChildId))
                        {
                            retrievedObject = true;
                        }
                    }
                    if (!retrievedObject)
                    {
                        List<long> keys = null;
                        if (!retrieveHash.TryGetValue(relationship.ChildTypeId, out keys))
                        {
                            keys = new List<long>();
                            retrieveHash.Add(relationship.ChildTypeId, keys);
                        }
                        if (!keys.Contains(relationship.ChildId))
                            keys.Add(relationship.ChildId);
                    }
                }
            }
            return retrieveHash;
        }

        private string GetMainTableNameFromContext(SearchContext context, Type objectType)
        {
            ComplexSearchElement timeStampElement = context.InitialSearch.Sections.Find("_Timestamp");
            ComplexSearchElement deletedElement = context.InitialSearch.Sections.Find("_Deleted");

            if (context.InitialSearch.IncludeDeleted || timeStampElement != null)
            {
                //In this instance there are deleted records and potential time dependent historical records so the main table must be the history table
                return objectType.Name + "_History";
            }

            //By default the main table should be the 'Head' table with only live data
            return objectType.Name;
        }

        private DataObjectReflection GetObjectReflection(Type type)
        {
            DataObjectReflection objectReflection = null;
            if (!DataManager.ReflectionCache.Contains(type))
            {
                DataObjectSchema dataObjectSchema = DataManager.ScriptManager.ScriptObjectSchema(type);
                objectReflection = new DataObjectReflection(dataObjectSchema);
                DataManager.ReflectionCache.Add(objectReflection);
            }
            else
            {
                objectReflection = DataManager.ReflectionCache[type];
            }
            return objectReflection;
        }

        private DataObjectReflection GetObjectReflection(short objectTypeId)
        {
            DataObjectReflection objectReflection = null;
            if (!DataManager.ReflectionCache.Contains(objectTypeId))
            {
                //If the reflection cache has never seen the object type, we need to make sure we have a valid place to store the object.
                //The script manager will adjust the database footprint for the object if necessary.
                DataObjectSchema dataObjectSchema = DataManager.ScriptManager.GetObjectSchema(objectTypeId);
                objectReflection = new DataObjectReflection(dataObjectSchema);
                DataManager.ReflectionCache.Add(objectReflection);
            }
            else
            {
                objectReflection = DataManager.ReflectionCache[objectTypeId];
            }
            return objectReflection;
        }
    }
}
