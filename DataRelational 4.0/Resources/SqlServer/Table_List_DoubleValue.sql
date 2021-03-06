CREATE TABLE [dbo].[List_DoubleValue](
	[_Id] [bigint] NOT NULL,
	[_Index] [int] NOT NULL,
	[Value] [float] NULL,
	 CONSTRAINT [PK_List_DoubleValue] PRIMARY KEY NONCLUSTERED 
(
	[_Id] ASC,
	[_Index] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[List_DoubleValue]  WITH CHECK ADD  CONSTRAINT [FK_List_DoubleValue_List_Double] FOREIGN KEY([_Id])
REFERENCES [dbo].[List_Double] ([_Id])

ALTER TABLE [dbo].[List_DoubleValue] CHECK CONSTRAINT [FK_List_DoubleValue_List_Double]