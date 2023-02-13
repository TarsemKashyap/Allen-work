
Create table CoveringDuty
(
Id int not null identity constraint IX_CoveringDuty_ID Primary key,
UserId varchar(50) Index IX_CoveringDuty_UserId CLUSTERED,
SOEID varchar(20),
UserName varchar(200),
DelegateUserId varchar(500),
DelegateSOEID varchar(500),
DelegateUserName varchar(500),
BeginDate date,
EndDate date,
Designation varchar(200),
StartOnMyBehalf varchar(1000),
InboxMyBehalf varchar(2000),
CreatedDate datetime,
ModifiedDate datetime,
)
go