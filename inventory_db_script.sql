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

create procedure usp_AddCategory(@CategoryName nvarchar(50),@CategoryId int null)
as
begin
insert into Category(CreateDate,UpdateDate,IsDeleted,CategoryName,CategoryId)
values(getdate(),getdate(),0,@CategoryName,@CategoryId);

select c.*,parent.CategoryName as ParentCategoryName
from category c left join category parent
on c.CategoryId=parent.Id where c.Id=scope_identity()
end


create procedure usp_UpdateCategory(@Id int,@CategoryName nvarchar(50),@CategoryId int null)
as
begin
Update Category
          set
          UpdateDate=getdate(),
          CategoryName=@CategoryName,
          CategoryId=@CategoryId where Id=@Id

select c.*,parent.CategoryName as ParentCategoryName
from category c left join category parent
on c.CategoryId=parent.Id where c.Id=@Id
end

create proc usp_getCategories  @searchTerm nvarchar(50)=''
as 
begin
select c.*,parent.CategoryName as ParentCategoryName
from category c left join category parent
on c.CategoryId=parent.Id where (@searchTerm ='' or c.CategoryName like '' + @searchTerm + '%')  and  c.IsDeleted=0 
end

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

create proc usp_updateProduct 
@Id int,
@ProductName nvarchar(50),
@CategoryId int,
@Price decimal(18,2)
as
begin
update Product set
         UpdateDate=getdate(), 
         ProductName=@ProductName,
         CategoryId=@CategoryId,
         Price=@Price
         where Id=@Id

select p.*,c.CategoryName from Product p join Category c on p.CategoryId = c.Id
where p.IsDeleted=0 and c.IsDeleted=0 and p.Id=@Id
end
