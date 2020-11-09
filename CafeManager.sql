create database CafeTableManager
go
use CafeTableManager
go
create table tAccount
(
	[username]    NVARCHAR (50)  NOT NULL,
    [displayname] NVARCHAR (MAX) NULL,
    [pass]        NVARCHAR (128) not null,
    [roll]        BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([username] ASC)
)
go
create table Catery
(
	 [id]     INT           IDENTITY (1, 1) NOT NULL,
    [CaName] NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
    UNIQUE NONCLUSTERED ([CaName] ASC)
)
go
create table Food
(
	    [id]       INT           IDENTITY (1, 1) NOT NULL,
    [foodname] NVARCHAR (50) DEFAULT (N'Món Chưa Ðặt Tên') NOT NULL,
    [foodCaId] INT           NULL,
    [Price]    INT           DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
    FOREIGN KEY ([foodCaId]) REFERENCES [dbo].[Catery] ([id])
)
go
create table tStatus
(
	 [id]        INT           IDENTITY (1, 1) NOT NULL,
    [satusName] NVARCHAR (50) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
)
go
create table tTable
(
	    [id]        INT           IDENTITY (1, 1) NOT NULL,
    [tableName] NVARCHAR (50) DEFAULT (N'Bàn Chưa Đặt Tên') NULL,
    [idStatus]  INT           DEFAULT ((1)) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
    FOREIGN KEY ([idStatus]) REFERENCES [dbo].[tStatus] ([id])
)
go
create table Bill
(
	    [id]          INT  IDENTITY (-2147483648, 1) NOT NULL,
    [idTable]     INT  NULL,
    [DateCheckIn] DATE DEFAULT (getdate()) NOT NULL,
    [lowPrice]    INT  NULL,
    [totalPrice]  INT  NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
	FOREIGN KEY (idTable) references tTable(id)
)
go
create table tBillInfo
(
	[idFood]    INT NULL,
    [foodcount] INT NULL,
    [idBill]    INT NULL,
    [price]     INT NULL,
    FOREIGN KEY ([idFood]) REFERENCES [dbo].[Food] ([id]),
    FOREIGN KEY ([idBill]) REFERENCES [dbo].[Bill] ([id])
)
go
create table tRAMBill
(
    [id]        INT IDENTITY (-2147483648, 1) NOT NULL,
    [idFood]    INT NULL,
    [idTable]   INT NULL,
    [foodCount] INT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
    FOREIGN KEY ([idTable]) REFERENCES [dbo].[tTable] ([id]) ON DELETE CASCADE,
    FOREIGN KEY ([idFood]) REFERENCES [dbo].[Food] ([id]) ON DELETE CASCADE
)
go

CREATE FUNCTION GetNonSymString (@S NVARCHAR(MAX) )
RETURNS NVARCHAR(MAX)
AS
BEGIN    
    IF (@S IS NULL OR @S = '')  RETURN ''
   
    DECLARE @RT NVARCHAR(MAX)
    DECLARE @SIGN_CHARS NCHAR(256)
    DECLARE @UNSIGN_CHARS NCHAR (256)
 
    SET @SIGN_CHARS = N'ăâđêôơưàảãạáằẳẵặắầẩẫậấèẻẽẹéềểễệếìỉĩịíòỏõọóồổỗộốờởỡợớùủũụúừửữựứỳỷỹỵýĂÂĐÊÔƠƯÀẢÃẠÁẰẲẴẶẮẦẨẪẬẤÈẺẼẸÉỀỂỄỆẾÌỈĨỊÍÒỎÕỌÓỒỔỖỘỐỜỞỠỢỚÙỦŨỤÚỪỬỮỰỨỲỶỸỴÝ' + NCHAR(272) + NCHAR(208)
    SET @UNSIGN_CHARS = N'aadeoouaaaaaaaaaaaaaaaeeeeeeeeeeiiiiiooooooooooooooouuuuuuuuuuyyyyyAADEOOUAAAAAAAAAAAAAAAEEEEEEEEEEIIIIIOOOOOOOOOOOOOOOUUUUUUUUUUYYYYYDD'
 
    DECLARE @COUNTER int
    DECLARE @COUNTER1 int
   
    SET @COUNTER = 1
    WHILE (@COUNTER <= LEN(@S))
    BEGIN  
        SET @COUNTER1 = 1
        WHILE (@COUNTER1 <= LEN(@SIGN_CHARS) + 1)
        BEGIN
            IF UNICODE(SUBSTRING(@SIGN_CHARS, @COUNTER1,1)) = UNICODE(SUBSTRING(@S,@COUNTER ,1))
            BEGIN          
                IF @COUNTER = 1
                    SET @S = SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@S, @COUNTER+1,LEN(@S)-1)      
                ELSE
                    SET @S = SUBSTRING(@S, 1, @COUNTER-1) +SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@S, @COUNTER+1,LEN(@S)- @COUNTER)
                BREAK
            END
            SET @COUNTER1 = @COUNTER1 +1
        END
        SET @COUNTER = @COUNTER +1
    END
    -- SET @inputVar = replace(@inputVar,' ','-')
    RETURN @S
END
go

create proc pDeleteCatergry
@IDCatergory int
as
begin
	delete Catery where Catery.id = @IDCatergory
end
go
CREATE proc pDeleteFood
@FoodID int
as
begin
	delete Food where Food.id = @FoodID
	declare @maxID int
	select @maxID = MAX([id]) from Food
	if @maxID is null
	begin
		set @maxID = 0
	end
	dbcc checkident ('Food', RESEED, @maxID)
	
end
go
create proc pDeleteTable
@TableID int
as
begin
	delete tTable where tTable.id = @TableID

	declare @maxID int
	select @maxID = MAX([id]) from tTable
	if @maxID is null
	begin
		set @maxID = 0
	end
	dbcc checkident ('tTable', RESEED, @maxID)
end
go
create proc pDeleteUser
@Username nvarchar(max)
as
begin
	delete tAccount where username = @Username
end
go
CREATE proc pGetBillByDate
@DateFrom date, @DateTo date
as
begin
		select Bill.id as [Mã Hóa Đơn], tTable.tableName as [Bàn Sở Hữu], DateCheckIn as [Ngày Thanh Toán], lowPrice as [Giảm Giá], totalPrice as [Tổng Tiền] from Bill, tTable where tTable.id = idTable and DateCheckIn >= @DateFrom and DateCheckIn <= @DateTo
end
go
create proc pInsertCatergry
@CaName nvarchar(max)
as
begin
	insert Catery(CaName) values(@CaName)
end
go
CREATE proc pInsertFood
@FoodName nvarchar(max), @CaID int , @Price int
as
begin
	insert Food (foodname, foodCaId, Price) values(@FoodName, @CaID, @Price)
end
go
create proc pInsertUser
@Username nvarchar(max), @Hash nvarchar(max), @Roll bit
as
begin
	insert tAccount (username,displayname, pass, roll ) values(@Username, @Username, @Hash, @Roll)
end
go
CREATE proc pLogin 
@Username nvarchar(max)
as
begin
	select * from tAccount where username = @Username
end
go
CREATE proc pPayBill
@TotalPrice int, @LowPrice int, @TableID int
as
begin
	insert Bill (idTable, lowPrice, totalPrice ) values (@TableID, @LowPrice, @TotalPrice)
	declare @IDBill int
	select @IDBill = MAX(id) from Bill
		INSERT tBillInfo(idFood,idBill, foodcount, price) select tRAMBill.idFood,@IDBill, tRAMBill.foodCount, Food.Price from tRAMBill, Food where tRAMBill.idTable = @TableID and tRAMBill.idFood = Food.id

	delete tRAMBill where tRAMBill.idTable = @TableID
	
	update tTable set idStatus = 1 where tTable.id = @TableID
	declare @maxID int
	select @maxID = MAX([id]) from tRAMBill
	if @maxID is null
	begin
		set @maxID = 0
	end
	dbcc checkident ('tRAMBill', RESEED, @maxID)
end
go
CREATE proc pSearchAcc
@KeyWord nvarchar(100)
as
begin
	select tAccount.username as [Tên Đăng Nhập],tAccount.roll as [Quyền] from tAccount where tAccount.username like '%'+@KeyWord+'%'
end
go
CREATE proc pSreachFood
@KeyWord nvarchar(100)
as
begin
	select Food.id as [ID], Food.foodname as [Tên Món],Catery.CaName as [Danh Mục], Food.Price as [Giá] from Food, Catery where Food.foodCaId  = Catery.id and dbo.GetNonSymString(Food.foodname) like '%'+ dbo.GetNonSymString(@KeyWord) +'%' or @KeyWord = str(Food.id)
end
go
create proc pUpdateCatergry
@IDCatergory int, @CaName nvarchar(max)
as
begin
	update Catery set CaName = @CaName where Catery.id = @IDCatergory
end
go
create proc pUpdateDisplayName
@Username nvarchar(max), @Displayname nvarchar(max)
as
begin
	update tAccount set displayname = @Displayname where username = @Username
end
go
create proc pUpdateFood
@FoodID int, @FoodName nvarchar(max), @CaID int, @Price int
as
begin
	update Food set foodname = @FoodName, foodCaId = @CaID, Price = @Price where Food.id = @FoodID
end
go

CREATE proc pUpdateMenuFoodProvider
@FoodID int, @TableID int
as
begin
	declare @FoodCount int
	select @FoodCount = tRAMBill.foodCount from tRAMBill where tRAMBill.idFood = @FoodID and tRAMBill.idTable = @TableID
	if @FoodCount >0
	begin
		update tRAMBill set foodCount = foodCount+1 where tRAMBill.idFood = @FoodID and tRAMBill.idTable = @TableID	
	end
	if @FoodCount is null
	begin
		insert tRAMBill (idFood, idTable , foodCount) values (@FoodID, @TableID, 1)
		declare @IdStatus int
	select @IdStatus = tTable.idStatus from tTable where tTable.id = @TableID
	update tTable set idStatus = 2 where tTable.id = @TableID
	end
end
go
create proc pUpdatePassword
@Username nvarchar(max), @Hash nvarchar(128)
as
begin
	update tAccount set pass = @Hash where username = @Username
end
go
create proc pUpdateTable
@TableID int, @TableName nvarchar(max)
as
begin
	update tTable set tableName = @TableName where id = @TableID
end
go
CREATE proc pUpdateUser
@Username nvarchar(max), @Roll bit
as
begin
	update tAccount set roll = @Roll where username = @Username
end
go
create proc pUpdateSubtractFoodMenuProvider
@FoodID int, @TableID int
as
begin
	declare @FoodCount int
	select @FoodCount = tRAMBill.foodCount from tRAMBill where tRAMBill.idFood = @FoodID and tRAMBill.idTable = @TableID
	if @FoodCount >0
	begin
		update tRAMBill set foodCount = foodCount-1 where tRAMBill.idFood = @FoodID and tRAMBill.idTable = @TableID	
		set @FoodCount = @FoodCount -1
	end
	if @FoodCount <=0
	begin
		delete tRAMBill where tRAMBill.idFood = @FoodID and tRAMBill.idTable = @TableID			
	end
	declare @TableMenuCount int
	select @TableMenuCount = COUNT (*) from tRAMBill where tRAMBill.idTable = @TableID
	if @TableMenuCount = 0
	begin
		update tTable set idStatus = 1 where tTable.id = @TableID
	end
end
go


