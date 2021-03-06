CREATE TABLE [dbo].[List_BooleanValue](
	[_Id] [bigint] NOT NULL,
	[_Index] [int] NOT NULL,
	[Value] [bit] NULL,
 CONSTRAINT [PK_List_BooleanValue] PRIMARY KEY NONCLUSTERED 
(
	[_Id] ASC,
	[_Index] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[List_BooleanValue]  WITH CHECK ADD  CONSTRAINT [FK_List_BooleanValue_List_Boolean] FOREIGN KEY([_Id])
REFERENCES [dbo].[List_Boolean] ([_Id])

ALTER TABLE [dbo].[List_BooleanValue] CHECK CONSTRAINT [FK_List_BooleanValue_List_Boolean]