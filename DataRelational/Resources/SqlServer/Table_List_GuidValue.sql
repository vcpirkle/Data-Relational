CREATE TABLE [dbo].[List_GuidValue](
	[_Id] [bigint] NOT NULL,
	[_Index] [int] NOT NULL,
	[Value] [uniqueidentifier] NULL,
	 CONSTRAINT [PK_List_GuidValue] PRIMARY KEY NONCLUSTERED 
(
	[_Id] ASC,
	[_Index] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[List_GuidValue]  WITH CHECK ADD  CONSTRAINT [FK_List_GuidValue_List_Guid] FOREIGN KEY([_Id])
REFERENCES [dbo].[List_Guid] ([_Id])

ALTER TABLE [dbo].[List_GuidValue] CHECK CONSTRAINT [FK_List_GuidValue_List_Guid]