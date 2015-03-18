using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational.Schema
{
    /// <summary>
    /// An enum that describes a field type
    /// </summary>
    enum DataObjectSchemaFieldType : byte
    {
        /// <summary>
        /// A boolean field - TSql: bit
        /// </summary>
        Boolean = 0,
        /// <summary>
        /// A byte field - TSql: tinyint
        /// </summary>
        Byte = 1,
        /// <summary>
        /// A short field - TSql: smallint
        /// </summary>
        Short = 2,
        /// <summary>
        /// A int field - TSql: int
        /// </summary>
        Int = 3,
        /// <summary>
        /// A long field - TSql: bigint
        /// </summary>
        Long = 4,
        /// <summary>
        /// A double field - TSql: float
        /// </summary>
        Double = 5,
        /// <summary>
        /// A float field - TSql: real
        /// </summary>
        Float = 6,
        /// <summary>
        /// A decimal field - TSql: decimal
        /// </summary>
        Decimal = 7,
        /// <summary>
        /// A datetime field - TSql: datetime
        /// </summary>
        DateTime = 8,
        /// <summary>
        /// A guid field - TSql: uniqueidentifier
        /// </summary>
        Guid = 9,
        /// <summary>
        /// A string field - TSql: varchar, nvarchar, text, ntext
        /// </summary>
        String = 10,
        /// <summary>
        /// A nullable boolean field - TSql: bit
        /// </summary>
        NullableBoolean = 11,
        /// <summary>
        /// A nullable byte field - TSql: tinyint
        /// </summary>
        NullableByte = 12,
        /// <summary>
        /// A nullable short field - TSql: smallint
        /// </summary>
        NullableShort = 13,
        /// <summary>
        /// A nullable int field - TSql: int
        /// </summary>
        NullableInt = 14,
        /// <summary>
        /// A nullable long field - TSql: bigint
        /// </summary>
        NullableLong = 15,
        /// <summary>
        /// A nullable double field - TSql: float
        /// </summary>
        NullableDouble = 16,
        /// <summary>
        /// A nullable float field - TSql: real
        /// </summary>
        NullableFloat = 17,
        /// <summary>
        /// A nullable decimal field - TSql: decimal
        /// </summary>
        NullableDecimal = 18,
        /// <summary>
        /// A nullable datetime field - TSql: datetime
        /// </summary>
        NullableDateTime = 19,
        /// <summary>
        /// A nullable guid field - TSql: uniqueidentifier
        /// </summary>
        NullableGuid = 20,
        /// <summary>
        /// An Array T where T is a primative type
        /// </summary>
        PrimativeArray = 21,
        /// <summary>
        /// A List T where T is a primative type
        /// </summary>
        PrimativeList = 22,
        /// <summary>
        /// A dictionary field with a key value pair consisting of primative types
        /// </summary>
        PrimativeDictionary = 23,
        /// <summary>
        /// An IDataObject field - TSql: a one to one relationship between parent and child objects
        /// </summary>
        IDataObject = 24,
        /// <summary>
        /// An IDataObject array - TSql: a one to many relationship between parent and child objects
        /// </summary>
        IDataObjectArray = 25,
        /// <summary>
        /// An IDataObject collection - TSql: a one to many relationship between parent and child objects
        /// </summary>
        IDataObjectList = 26,
        /// <summary>
        /// An IDataObject dictionary field with a key value pari consisting of a mixture of IDataObjects or primative types
        /// </summary>
        IDataObjectDictionary = 27
    }
}
