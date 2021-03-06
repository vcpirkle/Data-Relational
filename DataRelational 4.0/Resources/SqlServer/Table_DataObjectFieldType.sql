CREATE TABLE [dbo].[DataObjectFieldType](
	[_Id] [tinyint] NOT NULL,
	[Name] [varchar](255) NOT NULL,
 CONSTRAINT [PK_DataObjectFieldType] PRIMARY KEY CLUSTERED 
(
	[_Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
INSERT INTO DataObjectFieldType(_Id, Name) VALUES(0, 'Boolean')
INSERT INTO DataObjectFieldType(_Id, Name) VALUES(1, 'Byte')
INSERT INTO DataObjectFieldType(_Id, Name) VALUES(2, 'Short')
INSERT INTO DataObjectFieldType(_Id, Name) VALUES(3, 'Int')
INSERT INTO DataObjectFieldType(_Id, Name) VALUES(4, 'Long')
INSERT INTO DataObjectFieldType(_Id, Name) VALUES(5, 'Double')
INSERT INTO DataObjectFieldType(_Id, Name) VALUES(6, 'Float')
INSERT INTO DataObjectFieldType(_Id, Name) VALUES(7, 'Decimal')
INSERT INTO DataObjectFieldType(_Id, Name) VALUES(8, 'DateTime')
INSERT INTO DataObjectFieldType(_Id, Name) VALUES(9, 'Guid')
INSERT INTO DataObjectFieldType(_Id, Name) VALUES(10, 'String')
INSERT INTO DataObjectFieldType(_Id, Name) VALUES(11, 'NullableBoolean')
INSERT INTO DataObjectFieldType(_Id, Name) VALUES(12, 'NullableByte')
INSERT INTO DataObjectFieldType(_Id, Name) VALUES(13, 'NullableShort')
INSERT INTO DataObjectFieldType(_Id, Name) VALUES(14, 'NullableInt')
INSERT INTO DataObjectFieldType(_Id, Name) VALUES(15, 'NullableLong')
INSERT INTO DataObjectFieldType(_Id, Name) VALUES(16, 'NullableDouble')
INSERT INTO DataObjectFieldType(_Id, Name) VALUES(17, 'NullableFloat')
INSERT INTO DataObjectFieldType(_Id, Name) VALUES(18, 'NullableDecimal')
INSERT INTO DataObjectFieldType(_Id, Name) VALUES(19, 'NullableDateTime')
INSERT INTO DataObjectFieldType(_Id, Name) VALUES(20, 'NullableGuid')
INSERT INTO DataObjectFieldType(_Id, Name) VALUES(21, 'PrimativeArray')
INSERT INTO DataObjectFieldType(_Id, Name) VALUES(22, 'PrimativeList')
INSERT INTO DataObjectFieldType(_Id, Name) VALUES(23, 'PrimativeDictionary')
INSERT INTO DataObjectFieldType(_Id, Name) VALUES(24, 'IDataObject')
INSERT INTO DataObjectFieldType(_Id, Name) VALUES(25, 'IDataObjectArray')
INSERT INTO DataObjectFieldType(_Id, Name) VALUES(26, 'IDataObjectList')
INSERT INTO DataObjectFieldType(_Id, Name) VALUES(27, 'IDataObjectDictionary')