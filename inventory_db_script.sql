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

--product's stored procedures

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


create procedure usp_getProducts 
  @page int=1,
  @limit int=4,
  @searchTerm nvarchar(50) = null,
  @sortColumn nvarchar(20)='Id',
 @sortDirection nvarchar(5)='asc' 
as 
begin
select p.*, c.CategoryName from Product p join Category c
         on p.CategoryId=c.Id where (@searchTerm is null or p.ProductName like '%'+@searchTerm+'%' or c.CategoryName like '%'+@searchTerm+'%') 
		 and p.IsDeleted=0 and c.IsDeleted=0 
order by
case when @sortColumn='Id' and @sortDirection='asc' then p.Id end,
case when @sortColumn='Id' and @sortDirection='desc' then p.Id end desc,
case when @sortColumn='ProductName' and @sortDirection='asc' then p.ProductName end,
case when @sortColumn='ProductName' and @sortDirection='desc' then p.ProductName end desc,
case when @sortColumn='Price' and @sortDirection='asc' then p.Price end,
case when @sortColumn='Price' and @sortDirection='desc' then p.Price end desc,
case when @sortColumn='CreateDate' and @sortDirection='asc' then p.CreateDate end,
case when @sortColumn='CreateDate' and @sortDirection='desc' then p.CreateDate end desc,
case when @sortColumn='UpdateDate' and @sortDirection='asc' then p.UpdateDate end,
case when @sortColumn='UpdateDate' and @sortDirection='desc' then p.UpdateDate end desc,
case when @sortColumn='CategoryName' and @sortDirection='asc' then c.CategoryName end,
case when @sortColumn='CategoryName' and @sortDirection='desc' then c.CategoryName end desc

OFFSET(@page-1)*@limit ROWS
FETCH NEXT @limit ROWS ONLY;

select Count(p.Id) as TotalRecords,CAST(CEILING((count(p.Id)*1.0)/@limit)as int) as TotalPages
 from Product p join Category c
         on p.CategoryId=c.Id where (@searchTerm is null or p.ProductName like '%'+@searchTerm+'%' or c.CategoryName like '%'+@searchTerm+'%') 
		 and p.IsDeleted=0 and c.IsDeleted=0 
end