CREATE TABLE [dbo].[DataObjectRelationship](
	[ParentId] [bigint] NOT NULL,
	[ParentTypeId] [smallint] NOT NULL,
	[ParentFieldId] [int] NOT NULL,
	[ChildId] [bigint] NOT NULL,
	[ChildTypeId] [smallint] NOT NULL,
	[BeginDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[DataObjectRelationship]  WITH CHECK ADD  CONSTRAINT [FK_DataObjectRelationship_DataObjectField] FOREIGN KEY([ParentFieldId])
REFERENCES [dbo].[DataObjectField] ([_Id])
GO
ALTER TABLE [dbo].[DataObjectRelationship] CHECK CONSTRAINT [FK_DataObjectRelationship_DataObjectField]
GO
ALTER TABLE [dbo].[DataObjectRelationship]  WITH CHECK ADD  CONSTRAINT [FK_DataObjectRelationship_DataObjectType] FOREIGN KEY([ParentTypeId])
REFERENCES [dbo].[DataObjectType] ([_Id])
GO
ALTER TABLE [dbo].[DataObjectRelationship] CHECK CONSTRAINT [FK_DataObjectRelationship_DataObjectType]
GO
ALTER TABLE [dbo].[DataObjectRelationship]  WITH CHECK ADD  CONSTRAINT [FK_DataObjectRelationship_DataObjectType1] FOREIGN KEY([ChildTypeId])
REFERENCES [dbo].[DataObjectType] ([_Id])
GO
ALTER TABLE [dbo].[DataObjectRelationship] CHECK CONSTRAINT [FK_DataObjectRelationship_DataObjectType1]
GO
CREATE NONCLUSTERED INDEX [IDX_DataObjectRelationship] ON [dbo].[DataObjectRelationship] 
(
	[ParentId] ASC,
	[ParentTypeId] ASC,
	[ChildId] ASC,
	[ChildTypeId] ASC,
	[BeginDate] DESC,
	[EndDate] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]