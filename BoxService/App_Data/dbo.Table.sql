CREATE TABLE [dbo].[FoodItems] (
    [ID]    INT             IDENTITY (1, 1) NOT NULL,
    [Name]  NVARCHAR (MAX)  NULL,
    [Genre] NVARCHAR (MAX)  NULL,
    [Price] DECIMAL (18, 2) NOT NULL,
    CONSTRAINT [PK_dbo.FoodItems] PRIMARY KEY CLUSTERED ([ID] ASC)
);
