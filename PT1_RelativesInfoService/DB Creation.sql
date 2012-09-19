USE [PT1_DB]
GO

/****** Object:  Table [dbo].[Persones]    Script Date: 09/11/2012 10:27:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Persones](
	[PersonID] [int] IDENTITY(1,1) NOT NULL,
	[PassportNumber] [nvarchar](50) NOT NULL UNIQUE,
	[SecondName] [nvarchar](50) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[ThirdName] [nvarchar](50) NOT NULL,
	[Sex] [nvarchar](10) NOT NULL,
	[DateOfBirth] [date] NULL,
	[Address] [nvarchar](500) NULL,
    CONSTRAINT [PK_Persones] PRIMARY KEY CLUSTERED ([PersonID] ASC)
    WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) 
ON [PRIMARY]

GO

/****** Object:  Table [dbo].[Relationships]    Script Date: 09/11/2012 10:31:36 ******/

CREATE TABLE [dbo].[Relationships](
	[RelationshipID] [int] IDENTITY(1,1) NOT NULL,
	[FirstPersonID] [int] NOT NULL,
	[SecondPersonID] [int] NOT NULL,
	[State] [nvarchar](50) NOT NULL,
    CONSTRAINT [PK_Relationships] PRIMARY KEY CLUSTERED([RelationshipID] ASC)
    WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) 
ON [PRIMARY]

GO

ALTER TABLE [dbo].[Relationships]  WITH CHECK ADD  CONSTRAINT [FK_Relationships_Persones_FP] FOREIGN KEY([FirstPersonID])
REFERENCES [dbo].[Persones] ([PersonID])
GO

ALTER TABLE [dbo].[Relationships] CHECK CONSTRAINT [FK_Relationships_Persones_FP]
GO

ALTER TABLE [dbo].[Relationships]  WITH CHECK ADD  CONSTRAINT [FK_Relationships_Persones_SP] FOREIGN KEY([SecondPersonID])
REFERENCES [dbo].[Persones] ([PersonID])
GO

ALTER TABLE [dbo].[Relationships] CHECK CONSTRAINT [FK_Relationships_Persones_SP]
GO

INSERT INTO [dbo].[Persones]([PassportNumber],[SecondName],[FirstName],[ThirdName],[Sex],[DateOfBirth],[Address])
VALUES('1111654321', 'Glazachev', 'Konstantin', 'Igorevich', 'male', '1990-07-18', 'Samara')
