﻿CREATE TABLE [dbo].[Table]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [start_date] DATETIME NULL, 
    [end_date] DATETIME NULL DEFAULT 0
)
