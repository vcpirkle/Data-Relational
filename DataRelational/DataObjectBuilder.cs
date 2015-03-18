using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataRelational.Search;
using System.Data;
using DataRelational.Cache;
using System.Runtime.Serialization;
using DataRelational.Schema;
using System.Reflection;
using System.Collections;
using DataRelational.Script;
using System.Runtime.CompilerServices;

namespace DataRelational
{
    /// <summary>
    /// Used to build <see cref="DataRelational.IDataObject"/> based on a search context and a data set.
    /// </summary>
    static class DataObjectBuilder
    {
        /// <summary>
        /// Converts data dataset to a list of data objects and updates the search context with built objects.
        /// </summary>
        /// <param name="context">The context of the search.</param>
        /// <param name="requestingRelationship">The data relationship that is requesting the next level be loaded.</param>
        /// <param name="dataSet">The dataset containing new data objects.</param>
        public static IDataRelationship BuildObjects(SearchContext context, IDataRelationship requestingRelationship, DataSet dataSet)
        {
            DateTime start = DateTime.Now;
            foreach (DataTable table in dataSet.Tables)
            {
                if (!table.Columns.Contains("_ListId")) //A table of objects
                {
                    DataObjectReflection objReflection = null;
                    Dictionary<long, IDataObject> objectHash = null;
                    foreach (DataRow objRow in table.Rows)
                    {
                        if (objReflection == null)
                        {
                            short objectTypeId = (short)objRow["_ObjectTypeId"];
                            if (!DataManager.ReflectionCache.Contains(objectTypeId))
                            {
                                //Shit!  We have to lookup the object type by name.  This will be a rare occurance but will happen on first load scenarios.
                                Type objType = ScriptCommon.GetType((string)objRow["TypeName"]);
                                if (objType == null)
                                    throw new DataRelationalException(string.Format("Could not resolve the type {0}.", objRow["TypeName"]));

                                DataObjectSchema dataObjectSchema = DataManager.ScriptManager.ScriptObjectSchema(objType);
                                objReflection = new DataObjectReflection(dataObjectSchema);
                                DataManager.ReflectionCache.Add(objReflection);
                            }
                            else
                            {
                                objReflection = DataManager.ReflectionCache[objectTypeId];
                            }
                        }
                        if (objectHash == null)
                        {
                            if (!context.RetrievedObjects.TryGetValue(objReflection.Schema._Id, out objectHash))
                            {
                                objectHash = new Dictionary<long, IDataObject>();
                                context.RetrievedObjects.Add(objReflection.Schema._Id, objectHash);
                            }
                        }

                        IDataObject obj = null;
                        if (!objectHash.TryGetValue((long)objRow["_Id"], out obj))
                        {
                            obj = BuildObject(objRow);
                            objectHash.Add(obj._Id,obj);
                        }
                    }
                }
                else //A table of list items
                {
                    DataObjectReflection objReflection = null;
                    IDataObject currentObject = null;
                    IList currentList = null;
                    long currentListId = -1;
                    int currentFieldId = -1;
                    Type listType = null;

                    foreach (DataRow objRow in table.Rows)
                    {
                        if ((long)objRow["_ListId"] != currentListId)
                        {
                            if (currentList != null)
                            {
                                //Dump this list in the current object
                                SetPrimativeList(currentObject, objReflection, currentFieldId, currentList);
                            }

                            //Reset and move on to the next list
                            currentListId = -1;
                            currentFieldId = -1;
                            currentList = null;
                            currentObject = null;

                            objReflection = DataManager.ReflectionCache[(short)objRow["_DataObjectTypeId"]];
                            Dictionary<long, IDataObject> objectHash = null;
                            if (context.RetrievedObjects.TryGetValue(objReflection.Schema._Id, out objectHash))
                            {
                                if (objectHash.TryGetValue((long)objRow["ObjectId"], out currentObject))
                                {
                                    //Yays!  We found the object this list goes to.
                                    currentFieldId = (int)objRow["_DataObjectFieldId"];
                                    currentList = BuildPrimativeList(currentObject, objReflection, currentFieldId, out listType);
                                }
                            }
                            if (currentList != null)
                            {
                                currentListId = (long)objRow["_ListId"];
                            }
                        }

                        if (currentList != null)
                        {
                            currentList.Add(objRow["Value"]);
                        }
                    }

                    if (currentList != null)
                    {
                        //Dump this list in the current object
                        SetPrimativeList(currentObject, objReflection, currentFieldId, currentList);
                    }
                }
            }

            //Apply Relationships
            foreach (List<Relationship> relationships in context.Relationships.Values)
            {
                foreach (Relationship relationship in relationships)
                {
                    if (!ContainsRelationships(context.appliedRelationships, relationship))
                    {
                        IDataRelationship check = ApplyRelationship(context, requestingRelationship, relationship);
                        if (check != null && requestingRelationship != null && check.GetInstanceIdentifier() == requestingRelationship.GetInstanceIdentifier())
                            requestingRelationship = check;

                        context.appliedRelationships.Add(relationship);
                    }
                }
            }

            context.ApplySearchContext();
            if (context.FirstLevel.Count > 0)
            {
                List<Guid> instanceIdentifiers = new List<Guid>();
                DataObjectReflection objReflection = DataManager.ReflectionCache[context.FirstLevel[0].TypeId];
                foreach (DataKey dKey in context.FirstLevel)
                {
                    Dictionary<long, IDataObject> objectHash = null;
                    if (context.RetrievedObjects.TryGetValue(dKey.TypeId, out objectHash))
                    {
                        IDataObject obj = null;
                        if (objectHash.TryGetValue(dKey.Id, out obj))
                        {
                            context.ApplyLoadedDepthLevel(obj, ScriptCommon.GetChildRelationships(obj, objReflection), 2, instanceIdentifiers);
                        }
                    }
                }
            }

            DateTime end = DateTime.Now;
            TimeSpan ts = (end - start);
            Logging.Information(string.Format("Data object build time - {0} seconds {1} milliseconds", ts.Seconds, ts.Milliseconds));
            return requestingRelationship;
        }

        private static bool ContainsRelationships(List<Relationship> relationships, Relationship relationship)
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
                    return true;
                }
            }
            return false;
        }

        private static IDataObject BuildObject(DataRow objRow)
        {
            DataObjectReflection objReflection = null;
            if (!DataManager.ReflectionCache.Contains(Convert.ToInt16(objRow["_ObjectTypeId"])))
            {
                //If the reflection cache has never seen the object type, we need to make sure we have a valid place to store the object.
                //The script manager will adjust the database footprint for the object if necessary.
                DataObjectSchema dataObjectSchema = DataManager.ScriptManager.GetObjectSchema(Convert.ToInt16(objRow["_ObjectTypeId"]));
                objReflection = new DataObjectReflection(dataObjectSchema);
                DataManager.ReflectionCache.Add(objReflection);
            }
            else
            {
                objReflection = DataManager.ReflectionCache[Convert.ToInt16(objRow["_ObjectTypeId"])];
            }

            IDataObject obj = (IDataObject)FormatterServices.GetUninitializedObject(objReflection.Schema.ObjectType);
            IHistoricalDataObject hObj = obj as IHistoricalDataObject;

            obj._Id = (long)objRow["_Id"];
            if (hObj != null) { hObj._RevisionId = (int)objRow["_RevisionId"]; }
            obj._Deleted = (bool)objRow["_Deleted"];
            obj._ObjectType = objReflection.Schema;
            obj._Timestamp = (DateTime)objRow["_TimeStamp"];
            obj.SetInstanceIdentifier(Guid.NewGuid());
            foreach (DataObjectSchemaField field in objReflection.Schema.Fields)
            {
                if ((byte)field.FieldType < 21)
                {
                    if (!objRow.IsNull(field.Name))
                    {
                        object fieldValue = objRow[field.Name];
                        bool foundField = false;
                        foreach (FieldInfo fld in objReflection.DataFields)
                        {
                            if (fld.Name.ToLower() == field.Name.ToLower())
                            {
                                fld.SetValue(obj, fieldValue);
                                foundField = true;
                                break;
                            }
                        }
                        if (!foundField)
                        {
                            foreach (PropertyInfo prop in objReflection.DataProperties)
                            {
                                if (prop.Name.ToLower() == field.Name.ToLower())
                                {
                                    prop.SetValue(obj, fieldValue, null);
                                    foundField = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            obj.OnInitializing(true);
            return obj;
        }

        private static IList BuildPrimativeList(IDataObject obj, DataObjectReflection objReflection, int fieldId, out Type listType)
        {
            listType = null;
            IList rVal = null;
            foreach (DataObjectSchemaField field in objReflection.Schema.Fields)
            {
                if (field._Id == fieldId)
                {
                    bool foundField = false;
                    foreach (FieldInfo fld in objReflection.DataFields)
                    {
                        if (fld.Name.ToLower() == field.Name.ToLower())
                        {
                            if (field.FieldType == DataObjectSchemaFieldType.PrimativeArray)
                            {
                                listType = ScriptCommon.GetArrayType(fld.FieldType);
                            }
                            else if (field.FieldType == DataObjectSchemaFieldType.PrimativeList)
                            {
                                listType = fld.FieldType.GetGenericArguments()[0];
                            }
                            foundField = true;
                            break;
                        }
                    }
                    if (!foundField)
                    {
                        foreach (PropertyInfo prop in objReflection.DataProperties)
                        {
                            if (prop.Name.ToLower() == field.Name.ToLower())
                            {
                                if (field.FieldType == DataObjectSchemaFieldType.PrimativeArray)
                                {
                                    listType = ScriptCommon.GetArrayType(prop.PropertyType);
                                }
                                else if (field.FieldType == DataObjectSchemaFieldType.PrimativeList)
                                {
                                    listType = prop.PropertyType.GetGenericArguments()[0];
                                }
                                foundField = true;
                                break;
                            }
                        }
                    }
                    break;
                }
            }
            if (listType != null)
            {
                Type lType = typeof(List<>);
                Type[] typeArgs = { listType };
                Type constructed = lType.MakeGenericType(typeArgs);
                rVal = (IList)Activator.CreateInstance(constructed);
            }
            return rVal;
        }

        private static void SetPrimativeList(IDataObject obj, DataObjectReflection objReflection, int fieldId, IList list)
        {
            foreach (DataObjectSchemaField field in objReflection.Schema.Fields)
            {
                if (field._Id == fieldId)
                {
                    bool foundField = false;
                    foreach (FieldInfo fld in objReflection.DataFields)
                    {
                        if (fld.Name.ToLower() == field.Name.ToLower())
                        {
                            if (field.FieldType == DataObjectSchemaFieldType.PrimativeArray)
                            {
                                fld.SetValue(obj, list);
                            }
                            else if (field.FieldType == DataObjectSchemaFieldType.PrimativeList)
                            {
                                fld.SetValue(obj, list);
                            }
                            foundField = true;
                            break;
                        }
                    }
                    if (!foundField)
                    {
                        foreach (PropertyInfo prop in objReflection.DataProperties)
                        {
                            if (prop.Name.ToLower() == field.Name.ToLower())
                            {
                                if (field.FieldType == DataObjectSchemaFieldType.PrimativeArray)
                                {
                                    prop.SetValue(obj, list, null);
                                }
                                else if (field.FieldType == DataObjectSchemaFieldType.PrimativeList)
                                {
                                    prop.SetValue(obj, list, null);
                                }
                                foundField = true;
                                break;
                            }
                        }
                    }
                    break;
                }
            }
        }

        private static IDataRelationship ApplyRelationship(SearchContext context, IDataRelationship requestingRelationship, Relationship relationship)
        {
            IDataObject parent = null;
            IDataObject child = null;

            //Try to find the parent and child object for this relationship
            if (TryGetParentChild(relationship, context.RetrievedObjects, out parent, out child))
            {
                //If both objects are found we need to set the field, set the property, or add to the list
                DataObjectReflection objReflection = DataManager.ReflectionCache[parent.GetType()];

                foreach (DataObjectSchemaField field in objReflection.Schema.Fields)
                {
                    if (field._Id == relationship.ParentFieldId)
                    {
                        bool appliedRelationship = false;
                        foreach (FieldInfo fld in objReflection.RelationshipFields)
                        {
                            if (fld.Name.ToLower() == field.Name.ToLower())
                            {
                                if (fld.FieldType.IsGenericType && fld.FieldType.GetGenericTypeDefinition() == typeof(DataRelationship<>))
                                {
                                    if (fld.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Length == 0)
                                    {
                                        bool isRequestingRelationship = false;
                                        Type underlyingType = DataRelationship.GetUnderlyingType(fld.FieldType);
                                        if (underlyingType.GetInterface("DataRelational.IDataObject", false) != null)
                                        {
                                            IDataRelationship dRelationship = (IDataRelationship)fld.GetValue(parent);
                                            if (dRelationship == null || dRelationship.GetHasValue() == false)
                                            {
                                                if (dRelationship != null && requestingRelationship != null && requestingRelationship.GetInstanceIdentifier() == dRelationship.GetInstanceIdentifier())
                                                    isRequestingRelationship = true;

                                                Type dRelationType = typeof(DataRelationship<>);

                                                Type[] typeArgs = new Type[] { underlyingType };
                                                Type constructed = dRelationType.MakeGenericType(typeArgs);
                                                dRelationship = (IDataRelationship)Activator.CreateInstance(constructed, new object[] { child });
                                            }
                                            dRelationship.SetContext(context);
                                            fld.SetValue(parent, dRelationship);
                                            if (isRequestingRelationship)
                                            {
                                                requestingRelationship.SetContext(context);
                                                requestingRelationship.SetValue(child);
                                                return requestingRelationship;
                                            }

                                            appliedRelationship = true;
                                            break;
                                        }
                                        else if (ScriptCommon.GetArrayType(underlyingType) != null && ScriptCommon.GetArrayType(underlyingType).GetInterface("DataRelational.IDataObject", false) != null)
                                        {
                                            IDataRelationship dRelationshipArray = (IDataRelationship)fld.GetValue(parent);
                                            if (dRelationshipArray == null || dRelationshipArray.GetHasValue() == false)
                                            {
                                                if (dRelationshipArray != null && requestingRelationship != null && requestingRelationship.GetInstanceIdentifier() == dRelationshipArray.GetInstanceIdentifier())
                                                    isRequestingRelationship = true;

                                                Type dRelationType = typeof(DataRelationship<>);

                                                Array empty = Array.CreateInstance(ScriptCommon.GetArrayType(underlyingType), 0);

                                                Type[] typeArgs = new Type[] { empty.GetType() };
                                                Type constructed = dRelationType.MakeGenericType(typeArgs);
                                                dRelationshipArray = (IDataRelationship)Activator.CreateInstance(constructed, new object[] { empty });  
                                            }
                                            Array previousArray = (Array)dRelationshipArray.GetValue();
                                            Array newArray = Array.CreateInstance(ScriptCommon.GetArrayType(underlyingType), previousArray.Length + 1);
                                            Array.Copy(previousArray, 0, newArray, 0, previousArray.Length);
                                            newArray.SetValue(child, newArray.GetUpperBound(0));
                                            dRelationshipArray.SetValue(newArray);
                                            dRelationshipArray.SetContext(context);
                                            fld.SetValue(parent, dRelationshipArray);
                                            if (isRequestingRelationship)
                                            {
                                                requestingRelationship.SetContext(context);
                                                requestingRelationship.SetValue(newArray);
                                                return requestingRelationship;
                                            }

                                            appliedRelationship = true;
                                            break;
                                        }
                                        else if (underlyingType.IsGenericType && underlyingType.GetGenericTypeDefinition() == typeof(List<>))
                                        {
                                            IDataRelationship dRelationshipList = (IDataRelationship)fld.GetValue(parent);
                                            if (dRelationshipList == null || dRelationshipList.GetHasValue() == false)
                                            {
                                                if (dRelationshipList != null && requestingRelationship != null && requestingRelationship.GetInstanceIdentifier() == dRelationshipList.GetInstanceIdentifier())
                                                    isRequestingRelationship = true;

                                                Type dRelationType = typeof(DataRelationship<>);

                                                Type lType = typeof(List<>);
                                                Type[] typeArgs = { underlyingType.GetGenericArguments()[0] };
                                                Type constructed = lType.MakeGenericType(typeArgs);
                                                IList newList = (IList)Activator.CreateInstance(constructed);

                                                typeArgs = new Type[] { constructed };
                                                constructed = dRelationType.MakeGenericType(typeArgs);
                                                dRelationshipList = (IDataRelationship)Activator.CreateInstance(constructed, new object[] { newList });
                                            }
                                            IList list = (IList)dRelationshipList.GetValue();
                                            list.Add(child);
                                            dRelationshipList.SetValue(list);
                                            dRelationshipList.SetContext(context);
                                            fld.SetValue(parent, dRelationshipList);
                                            if (isRequestingRelationship)
                                            {
                                                requestingRelationship.SetContext(context);
                                                requestingRelationship.SetValue(list);
                                                return requestingRelationship;
                                            }

                                            appliedRelationship = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        if (!appliedRelationship)
                        {
                            foreach (PropertyInfo prop in objReflection.RelationshipProperties)
                            {
                                if (prop.Name.ToLower() == field.Name.ToLower())
                                {
                                    if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(DataRelationship<>))
                                    {
                                        bool isRequestingRelationship = false;
                                        Type underlyingType = DataRelationship.GetUnderlyingType(prop.PropertyType);
                                        if (underlyingType.GetInterface("DataRelational.IDataObject", false) != null)
                                        {
                                            IDataRelationship dRelationship = (IDataRelationship)prop.GetValue(parent, null);
                                            if (dRelationship == null || dRelationship.GetHasValue() == false)
                                            {
                                                if (dRelationship != null && requestingRelationship != null && requestingRelationship.GetInstanceIdentifier() == dRelationship.GetInstanceIdentifier())
                                                    isRequestingRelationship = true;

                                                Type dRelationType = typeof(DataRelationship<>);

                                                Type[] typeArgs = new Type[] { underlyingType };
                                                Type constructed = dRelationType.MakeGenericType(typeArgs);
                                                dRelationship = (IDataRelationship)Activator.CreateInstance(constructed, new object[]{child});
                                            }
                                            dRelationship.SetContext(context);
                                            prop.SetValue(parent, dRelationship, null);
                                            if (isRequestingRelationship)
                                            {
                                                requestingRelationship.SetContext(context);
                                                requestingRelationship.SetValue(child);
                                                return requestingRelationship;
                                            }

                                            appliedRelationship = true;
                                            break;
                                        }
                                        else if (ScriptCommon.GetArrayType(underlyingType) != null && ScriptCommon.GetArrayType(underlyingType).GetInterface("DataRelational.IDataObject", false) != null)
                                        {
                                            IDataRelationship dRelationshipArray = (IDataRelationship)prop.GetValue(parent, null);
                                            if (dRelationshipArray == null || dRelationshipArray.GetHasValue() == false)
                                            {
                                                if (dRelationshipArray != null && requestingRelationship != null && requestingRelationship.GetInstanceIdentifier() == dRelationshipArray.GetInstanceIdentifier())
                                                    isRequestingRelationship = true;

                                                Type dRelationType = typeof(DataRelationship<>);

                                                Array empty = Array.CreateInstance(ScriptCommon.GetArrayType(underlyingType), 0);

                                                Type[] typeArgs = new Type[] { empty.GetType() };
                                                Type constructed = dRelationType.MakeGenericType(typeArgs);
                                                dRelationshipArray = (IDataRelationship)Activator.CreateInstance(constructed, new object[] { empty });
                                            }
                                            Array previousArray = (Array)dRelationshipArray.GetValue();
                                            Array newArray = Array.CreateInstance(ScriptCommon.GetArrayType(underlyingType), previousArray.Length + 1);
                                            Array.Copy(previousArray, 0, newArray, 0, previousArray.Length);
                                            newArray.SetValue(child, newArray.GetUpperBound(0));
                                            dRelationshipArray.SetValue(newArray);
                                            dRelationshipArray.SetContext(context);
                                            prop.SetValue(parent, dRelationshipArray, null);
                                            if (isRequestingRelationship)
                                            {
                                                requestingRelationship.SetContext(context);
                                                requestingRelationship.SetValue(newArray);
                                                return requestingRelationship;
                                            }

                                            appliedRelationship = true;
                                            break;
                                        }
                                        else if (underlyingType.IsGenericType && underlyingType.GetGenericTypeDefinition() == typeof(List<>))
                                        {
                                            IDataRelationship dRelationshipList = (IDataRelationship)prop.GetValue(parent, null);
                                            if (dRelationshipList == null || dRelationshipList.GetHasValue() == false)
                                            {
                                                if (dRelationshipList != null && requestingRelationship != null && requestingRelationship.GetInstanceIdentifier() == dRelationshipList.GetInstanceIdentifier())
                                                    isRequestingRelationship = true;

                                                Type dRelationType = typeof(DataRelationship<>);

                                                Type lType = typeof(List<>);
                                                Type[] typeArgs = { underlyingType.GetGenericArguments()[0] };
                                                Type constructed = lType.MakeGenericType(typeArgs);
                                                IList newList = (IList)Activator.CreateInstance(constructed);

                                                typeArgs = new Type[] { constructed };
                                                constructed = dRelationType.MakeGenericType(typeArgs);
                                                dRelationshipList = (IDataRelationship)Activator.CreateInstance(constructed, new object[] { newList });
                                            }
                                            IList list = (IList)dRelationshipList.GetValue();
                                            list.Add(child);
                                            dRelationshipList.SetValue(list);
                                            dRelationshipList.SetContext(context);
                                            prop.SetValue(parent, dRelationshipList, null);
                                            if (isRequestingRelationship)
                                            {
                                                requestingRelationship.SetContext(context);
                                                requestingRelationship.SetValue(list);
                                                return requestingRelationship;
                                            }

                                            appliedRelationship = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        private static bool TryGetParentChild(Relationship relationship, Dictionary<short, Dictionary<long, IDataObject>> objDictionary, out IDataObject parent, out IDataObject child)
        {
            IDataObject pObj = null, cObj = null;
            Dictionary<long, IDataObject> pDictionary = null, cDictionary = null;
            if (objDictionary.TryGetValue(relationship.ParentTypeId, out pDictionary) && objDictionary.TryGetValue(relationship.ChildTypeId, out cDictionary))
            {
                pDictionary.TryGetValue(relationship.ParentId, out pObj);
                cDictionary.TryGetValue(relationship.ChildId, out cObj);

                if (pObj != null && cObj != null)
                {
                    parent = pObj;
                    child = cObj;
                    return true;
                }
            }

            parent = null;
            child = null;
            return false;
        }
    }
}
