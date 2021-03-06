CREATE TABLE [dbo].[List_StringValue](
	[_Id] [bigint] NOT NULL,
	[_Index] [int] NOT NULL,
	[Value] [ntext] NULL,
	 CONSTRAINT [PK_List_StringValue] PRIMARY KEY NONCLUSTERED 
(
	[_Id] ASC,
	[_Index] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[List_StringValue]  WITH CHECK ADD  CONSTRAINT [FK_List_StringValue_List_String] FOREIGN KEY([_Id])
REFERENCES [dbo].[List_String] ([_Id])

ALTER TABLE [dbo].[List_StringValue] CHECK CONSTRAINT [FK_List_StringValue_List_String]