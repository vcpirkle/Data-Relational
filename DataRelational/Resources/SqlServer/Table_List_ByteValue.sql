CREATE TABLE [dbo].[List_ByteValue](
	[_Id] [bigint] NOT NULL,
	[_Index] [int] NOT NULL,
	[Value] [tinyint] NULL,
	 CONSTRAINT [PK_List_ByteValue] PRIMARY KEY NONCLUSTERED 
(
	[_Id] ASC,
	[_Index] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[List_ByteValue]  WITH CHECK ADD  CONSTRAINT [FK_List_ByteValue_List_Byte] FOREIGN KEY([_Id])
REFERENCES [dbo].[List_Byte] ([_Id])

ALTER TABLE [dbo].[List_ByteValue] CHECK CONSTRAINT [FK_List_ByteValue_List_Byte]