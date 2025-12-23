USE master;
GO

PRINT N'Creating Sales...';  
GO  

CREATE DATABASE [Sales_DEABB979];
GO  

USE [Sales_DEABB979];
GO

PRINT N'Creating Sales.Customer...';  
GO  

CREATE TABLE [dbo].[Customer] (  
    [CustomerID]   INT           IDENTITY (1, 1) NOT NULL,  
    [CustomerName] NVARCHAR (40) NOT NULL,  
);  
GO  

PRINT N'Creating Sales.Orders...';  
GO  

CREATE TABLE [dbo].[Orders] (  
    [CustomerID] INT      NOT NULL,  
    [OrderID]    INT      IDENTITY (1, 1) NOT NULL,  
    [OrderDate]  DATETIME NOT NULL,  
    [Status]     CHAR (1) NOT NULL,  
    [Amount]     INT      NOT NULL  
);  
GO  

ALTER TABLE [dbo].[Customer]  
    ADD CONSTRAINT [PK_Customer_CustID] PRIMARY KEY CLUSTERED ([CustomerID] ASC);  
GO  

PRINT N'Creating Sales.PK_Orders_OrderID...';  
GO  

ALTER TABLE [dbo].[Orders]  
    ADD CONSTRAINT [PK_Orders_OrderID] PRIMARY KEY CLUSTERED ([OrderID] ASC);  
GO  

PRINT N'Creating Sales.FK_Orders_Customer_CustID...';  
GO  

ALTER TABLE [dbo].[Orders]
    ADD CONSTRAINT [FK_Orders_Customer_CustID] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer] ([CustomerID]);  
GO