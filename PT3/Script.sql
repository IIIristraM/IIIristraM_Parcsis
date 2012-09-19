USE [PT3_DB]
GO

/****** Object:  Table [dbo].[Orders]    Script Date: 09/11/2012 19:13:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Customers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO DBO.Customers(Name) VALUES('Vasia')
INSERT INTO DBO.Customers(Name) VALUES('Petia') 
INSERT INTO DBO.Customers(Name) VALUES('Dima') 
INSERT INTO DBO.Customers(Name) VALUES('Vova') 
INSERT INTO DBO.Customers(Name) VALUES('Sergey') 

CREATE TABLE [dbo].[Orders](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[Date] [datetime] NOT NULL,
	[Sum] [money] NOT NULL,
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_Customers] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([Id])
GO

ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Customers]
GO

INSERT INTO DBO.Orders(CustomerId, [Date], [Sum]) VALUES(1, '2012-09-01', 5) 
INSERT INTO DBO.Orders(CustomerId, [Date], [Sum]) VALUES(1, '2012-09-01', 2) 
INSERT INTO DBO.Orders(CustomerId, [Date], [Sum]) VALUES(1, '2012-09-01', 7) 
INSERT INTO DBO.Orders(CustomerId, [Date], [Sum]) VALUES(1, '2012-09-02', 1) 
INSERT INTO DBO.Orders(CustomerId, [Date], [Sum]) VALUES(1, '2012-05-11', 10)

INSERT INTO DBO.Orders(CustomerId, [Date], [Sum]) VALUES(2, '2012-09-01', 5) 
INSERT INTO DBO.Orders(CustomerId, [Date], [Sum]) VALUES(2, '2012-09-01', 2) 
INSERT INTO DBO.Orders(CustomerId, [Date], [Sum]) VALUES(2, '2012-09-02', 7) 
INSERT INTO DBO.Orders(CustomerId, [Date], [Sum]) VALUES(2, '2012-09-02', 1) 
INSERT INTO DBO.Orders(CustomerId, [Date], [Sum]) VALUES(2, '2012-08-11', 10)

INSERT INTO DBO.Orders(CustomerId, [Date], [Sum]) VALUES(3, '2012-09-01', 5) 
INSERT INTO DBO.Orders(CustomerId, [Date], [Sum]) VALUES(3, '2012-09-01', 2) 
INSERT INTO DBO.Orders(CustomerId, [Date], [Sum]) VALUES(3, '2012-09-01', 7) 
INSERT INTO DBO.Orders(CustomerId, [Date], [Sum]) VALUES(3, '2012-09-02', 1) 
INSERT INTO DBO.Orders(CustomerId, [Date], [Sum]) VALUES(3, '2012-09-02', 10)

INSERT INTO DBO.Orders(CustomerId, [Date], [Sum]) VALUES(4, '2012-09-09', 5) 
INSERT INTO DBO.Orders(CustomerId, [Date], [Sum]) VALUES(4, '2012-09-09', 2) 
INSERT INTO DBO.Orders(CustomerId, [Date], [Sum]) VALUES(4, '2012-09-07', 7) 
INSERT INTO DBO.Orders(CustomerId, [Date], [Sum]) VALUES(4, '2012-09-08', 1) 
INSERT INTO DBO.Orders(CustomerId, [Date], [Sum]) VALUES(4, '2012-09-08', 10)

INSERT INTO DBO.Orders(CustomerId, [Date], [Sum]) VALUES(5, '2012-09-09', 5) 
INSERT INTO DBO.Orders(CustomerId, [Date], [Sum]) VALUES(5, '2012-09-01', 2) 
INSERT INTO DBO.Orders(CustomerId, [Date], [Sum]) VALUES(5, '2012-05-07', 7) 
INSERT INTO DBO.Orders(CustomerId, [Date], [Sum]) VALUES(5, '2012-09-08', 1) 
INSERT INTO DBO.Orders(CustomerId, [Date], [Sum]) VALUES(5, '2012-09-08', 10)

/*----------1----------*/

SELECT Customers.Name, CONVERT(date, Orders.[Date]) as 'Date', COUNT(DISTINCT Orders.Id) as 'Count of orders'
FROM Customers, Orders
WHERE Customers.Id = Orders.CustomerId AND
      YEAR(Orders.[Date]) = YEAR(GETDATE()) AND
      (MONTH(GETDATE()) - MONTH(Orders.[Date])) <= 2
GROUP BY Orders.[Date], Customers.Name

/*----------2----------*/

SELECT Customers.Name, CONVERT(date, Orders.[Date]) as 'Date', MAX(DISTINCT Orders.Sum) as 'Max sum'
FROM Customers, Orders
WHERE Customers.Id = Orders.CustomerId AND
      YEAR(Orders.[Date]) = YEAR(GETDATE()) AND
      MONTH(Orders.[Date]) = MONTH(GETDATE()) AND
      (DAY(GETDATE()) - DAY(Orders.[Date])) <= 7
GROUP BY Orders.[Date], Customers.Name