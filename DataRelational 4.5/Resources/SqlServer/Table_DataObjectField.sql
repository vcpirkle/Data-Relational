CREATE TABLE [dbo].[DataObjectField](
	[_Id] [int] IDENTITY(1,1) NOT NULL,
	[_DataObjectTypeId] [smallint] NOT NULL,
	[FieldName] [varchar](255) NOT NULL,
	[_DataObjectFieldTypeId] [tinyint] NOT NULL,
	[FieldLength] [int] NOT NULL,
	[Unicode] [bit] NOT NULL,
 CONSTRAINT [PK_DataObjectField] PRIMARY KEY CLUSTERED 
(
	[_Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[DataObjectField]  WITH CHECK ADD  CONSTRAINT [FK_DataObjectField_DataObjectFieldType] FOREIGN KEY([_DataObjectFieldTypeId])
REFERENCES [dbo].[DataObjectFieldType] ([_Id])
GO
ALTER TABLE [dbo].[DataObjectField] CHECK CONSTRAINT [FK_DataObjectField_DataObjectFieldType]
GO
ALTER TABLE [dbo].[DataObjectField]  WITH CHECK ADD  CONSTRAINT [FK_DataObjectField_DataObjectType] FOREIGN KEY([_DataObjectTypeId])
REFERENCES [dbo].[DataObjectType] ([_Id])
GO
ALTER TABLE [dbo].[DataObjectField] CHECK CONSTRAINT [FK_DataObjectField_DataObjectType]