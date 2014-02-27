﻿CREATE TABLE [dbo].[Users] (
    [ID]         UNIQUEIDENTIFIER NOT NULL,
    [Identifier] NVARCHAR (255)   NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK2C1C7FE57C0F2309] FOREIGN KEY ([ID]) REFERENCES [dbo].[Profiles] ([ID]),
    UNIQUE NONCLUSTERED ([Identifier] ASC)
);
