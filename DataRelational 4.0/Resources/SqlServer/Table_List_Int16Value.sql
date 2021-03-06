CREATE TABLE [dbo].[List_Int16Value](
	[_Id] [bigint] NOT NULL,
	[_Index] [int] NOT NULL,
	[Value] [smallint] NULL,
	 CONSTRAINT [PK_List_Int16Value] PRIMARY KEY NONCLUSTERED 
(
	[_Id] ASC,
	[_Index] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[List_Int16Value]  WITH CHECK ADD  CONSTRAINT [FK_List_Int16Value_List_Int16] FOREIGN KEY([_Id])
REFERENCES [dbo].[List_Int16] ([_Id])

ALTER TABLE [dbo].[List_Int16Value] CHECK CONSTRAINT [FK_List_Int16Value_List_Int16]