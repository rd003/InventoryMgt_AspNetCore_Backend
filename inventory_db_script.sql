create database InventoryMgt

use InventoryMgt

create table Category(
Id int primary key identity,
CreateDate  datetime not null,
UpdateDate  datetime not null,
IsDeleted bit,
CategoryName nvarchar(50) not null,
CategoryId int null
)

create table Product(
Id int primary key identity,
CreateDate  datetime not null,
UpdateDate  datetime not null,
IsDeleted bit,
ProductName nvarchar(50) not null,
CategoryId int not null references Category(Id),
Price decimal(18,2) not null
)

create table Stock(
Id int primary key identity,
CreateDate  datetime not null,
UpdateDate  datetime not null,
IsDeleted bit,
ProductId int references Product(Id) unique, 
Quantity float not null
)

create table Purchase(
Id int primary key identity,
CreateDate  datetime not null,
UpdateDate  datetime not null,
IsDeleted bit,
ProductId int references Product(Id), 
PurchaseDate datetime not null,
Quantity float not null,
Price decimal(18,2) not null
[Description] nvarchar(100)
)

create table Sale(
Id int primary key identity,
CreateDate  datetime not null,
UpdateDate  datetime not null,
IsDeleted bit,
ProductId int not null references Product(Id),
SellingDate datetime not null,
Quantity float, 
Price decimal(18,2) not null
[Description] nvarchar(100) not null
)

--stored procedures

create procedure Usp_AddProduct
(
  @ProductName nvarchar(50), @CategoryId int, @Price decimal(18,2)
)
as
begin
declare @lastId int
insert into Product (CreateDate,UpdateDate,IsDeleted,ProductName,CategoryId,Price)
values(getdate(),getdate(),0,@ProductName,@CategoryId,@Price)
select @lastId=SCOPE_IDENTITY()

select p.*,c.CategoryName from Product p join Category c on p.CategoryId = c.Id
where p.IsDeleted=0 and c.IsDeleted=0 and p.Id=@lastId
