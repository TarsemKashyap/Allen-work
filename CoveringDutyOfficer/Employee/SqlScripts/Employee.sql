

CREATE TABLE [dbo].[Employee](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [varchar](50) NULL,
	[EMAIL] [varchar](50) NULL,
	[NRIC] [varchar](500) NULL,
	[SOEID] [varchar](500) NULL,
	[DIDEXTENSION] [varchar](500) NULL,
	[HANDPHONE] [varchar](500) NULL,
	[FAXNUMBER] [varchar](500) NULL,
	[FIRSTNAME] [varchar](100) NULL,
	[LASTNAME] [varchar](100) NULL,
	[KNOWNAS] [varchar](100) NULL,
	[NAME] [varchar](100) NULL,
	[POSITIONID] [varchar](500) NULL,
	[POSITIONNAME] [varchar](500) NULL,
	[LOCATION] [varchar](2000) NULL,
	[EMPLOYEETYPE] [varchar](100) NULL,
	[ROEMAIL] [varchar](500) NULL,
	[WORKUNITNAME] [varchar](500) NULL,
	[WORKUNITCODE] [varchar](500) NULL,
	[SECTIONNAME] [varchar](500) NULL,
	[SECTIONCODE] [varchar](500) NULL,
	[BRANCHNAME] [varchar](500) NULL,
	[BRANCHCODE] [varchar](500) NULL,
	[DIVISIONNAME] [varchar](500) NULL,
	[DIVISIONCODE] [varchar](500) NULL,
	[CLUSTERNAME] [varchar](500) NULL,
	[CLUSTERCODE] [varchar](500) NULL,
	[BOARDNAME] [varchar](1000) NULL,
	[BOARDCODE] [varchar](2000) NULL,
	[TERMINATIONDATE] [datetime] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL
) ON [PRIMARY]
GO


