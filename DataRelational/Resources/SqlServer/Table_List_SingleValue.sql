CREATE TABLE [dbo].[List_SingleValue](
	[_Id] [bigint] NOT NULL,
	[_Index] [int] NOT NULL,
	[Value] [real] NULL,
	 CONSTRAINT [PK_List_SingleValue] PRIMARY KEY NONCLUSTERED 
(
	[_Id] ASC,
	[_Index] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[List_SingleValue]  WITH CHECK ADD  CONSTRAINT [FK_List_SingleValue_List_Single] FOREIGN KEY([_Id])
REFERENCES [dbo].[List_Single] ([_Id])

ALTER TABLE [dbo].[List_SingleValue] CHECK CONSTRAINT [FK_List_SingleValue_List_Single]