CREATE TABLE [dbo].[List_DateTimeValue](
	[_Id] [bigint] NOT NULL,
	[_Index] [int] NOT NULL,
	[Value] [datetime] NULL,
	 CONSTRAINT [PK_List_DateTimeValue] PRIMARY KEY NONCLUSTERED 
(
	[_Id] ASC,
	[_Index] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[List_DateTimeValue]  WITH CHECK ADD  CONSTRAINT [FK_List_DateTimeValue_List_DateTime] FOREIGN KEY([_Id])
REFERENCES [dbo].[List_DateTime] ([_Id])

ALTER TABLE [dbo].[List_DateTimeValue] CHECK CONSTRAINT [FK_List_DateTimeValue_List_DateTime]