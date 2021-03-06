CREATE TABLE [dbo].[DataObjectIndex](
	[_Id] [int] NOT NULL,
	[_DataObjectTypeId] [smallint] NOT NULL,
	[_DataObjectFieldId] [int] NOT NULL,
	[IndexName] [varchar](255) NOT NULL,
	[Ascending] [bit] NOT NULL,
 CONSTRAINT [PK_DataObjectIndex] PRIMARY KEY CLUSTERED 
(
	[_Id] ASC,
	[_DataObjectFieldId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[DataObjectIndex]  WITH CHECK ADD  CONSTRAINT [FK_DataObjectIndex_DataObjectField] FOREIGN KEY([_DataObjectFieldId])
REFERENCES [dbo].[DataObjectField] ([_Id])
GO
ALTER TABLE [dbo].[DataObjectIndex] CHECK CONSTRAINT [FK_DataObjectIndex_DataObjectField]
GO
ALTER TABLE [dbo].[DataObjectIndex]  WITH CHECK ADD  CONSTRAINT [FK_DataObjectIndex_DataObjectType] FOREIGN KEY([_DataObjectTypeId])
REFERENCES [dbo].[DataObjectType] ([_Id])
GO
ALTER TABLE [dbo].[DataObjectIndex] CHECK CONSTRAINT [FK_DataObjectIndex_DataObjectType]