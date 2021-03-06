CREATE TABLE [dbo].[List_DecimalValue](
	[_Id] [bigint] NOT NULL,
	[_Index] [int] NOT NULL,
	[Value] [decimal] NULL,
	 CONSTRAINT [PK_List_DecimalValue] PRIMARY KEY NONCLUSTERED 
(
	[_Id] ASC,
	[_Index] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[List_DecimalValue]  WITH CHECK ADD  CONSTRAINT [FK_List_DecimalValue_List_Decimal] FOREIGN KEY([_Id])
REFERENCES [dbo].[List_Decimal] ([_Id])

ALTER TABLE [dbo].[List_DecimalValue] CHECK CONSTRAINT [FK_List_DecimalValue_List_Decimal]