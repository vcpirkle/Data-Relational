CREATE TABLE [dbo].[List_Double](
	[_Id] [bigint] IDENTITY(1,1) NOT NULL,
	[_DataObjectTypeId] [smallint] NOT NULL,
	[ObjectId] [bigint] NOT NULL,
	[ObjectRevisionId] [int] NOT NULL,
	[_DataObjectFieldId] [int] NOT NULL,
 CONSTRAINT [PK_List_Double] PRIMARY KEY CLUSTERED 
(
	[_Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[List_Double]  WITH CHECK ADD  CONSTRAINT [FK_List_Double_DataObjectField] FOREIGN KEY([_DataObjectFieldId])
REFERENCES [dbo].[DataObjectField] ([_Id])
GO
ALTER TABLE [dbo].[List_Double] CHECK CONSTRAINT [FK_List_Double_DataObjectField]
GO
ALTER TABLE [dbo].[List_Double]  WITH CHECK ADD  CONSTRAINT [FK_List_Double_DataObjectType] FOREIGN KEY([_DataObjectTypeId])
REFERENCES [dbo].[DataObjectType] ([_Id])
GO
ALTER TABLE [dbo].[List_Double] CHECK CONSTRAINT [FK_List_Double_DataObjectType]