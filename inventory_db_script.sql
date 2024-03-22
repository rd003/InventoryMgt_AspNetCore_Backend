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
CreateDate  datetime not null default getdate(),
UpdateDate  datetime not null default getdate(),
IsDeleted bit default 0,
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

-- usp: get products

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

-- purchase stored procedures

-- usp: add purchase

create procedure dbo.usp_AddPurchase
 @ProductId int , @PurchaseDate datetime ,
 @Quantity float,@Price decimal(18,2) ,@Description nvarchar(100)
as begin
begin Transaction;
 begin try
  declare @createdPurchaseId int;

  insert into Purchase(CreateDate,UpdateDate,IsDeleted,ProductId,PurchaseDate,Quantity,Price,[Description])
  values
  (getdate(),getdate(),0,@ProductId,@PurchaseDate,@Quantity,@Price,@Description);

  set @createdPurchaseId=SCOPE_IDENTITY();

  -- managing stock
  if exists(select 1 from Stock where ProductId=@ProductId)
  begin
    update Stock set Quantity=Quantity+@Quantity where ProductId=@ProductId;
  end
  else
  begin
    insert into Stock(ProductId,Quantity) values (@ProductId,@Quantity)
  end
  
  COMMIT TRANSACTION;
  --  returning created purchase entry

   select purchase.*,product.ProductName from
   Purchase purchase join Product product
   on purchase.ProductId = product.Id
   where purchase.IsDeleted=0 and product.IsDeleted=0 and purchase.Id=@createdPurchaseId
 end try

 begin catch
   ROLLBACK TRANSACTION;
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    DECLARE @ErrorState INT = ERROR_STATE();
    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
 end catch
end

-- usp: update purchase

create procedure dbo.usp_UpdatePurchase
 @Id int,
 @ProductId int, @PurchaseDate datetime,
 @Quantity float,@Price decimal(18,2),@Description nvarchar(100)
as begin
begin Transaction;
 begin try
  declare @previousProductId int,@previousQuantity int
  select @previousProductId=ProductId, @previousQuantity=Quantity from Purchase where Id=@Id

  update  Purchase set UpdateDate=getdate(),
    ProductId=@ProductId,PurchaseDate=@PurchaseDate,Quantity=@Quantity,Price=@Price,[Description]=@Description
	where Id=@Id
  -- managing stock
 
  -- if we are having the same product
  if(@previousProductId=@ProductId)
   begin
    update Stock set Quantity=(Quantity-@previousQuantity)+@Quantity where ProductId=@ProductId;
   end

   -- if we are having the different product
   else
   begin
    --decrease the quantity of previous product
    update Stock set Quantity=Quantity-@previousQuantity where ProductId=@previousProductId;

	-- increasing quantity of new product
	if exists(select 1 from Stock where productId=@ProductId)
	 begin
	  update Stock set Quantity=Quantity+@Quantity where ProductId=@ProductId;
	 end
	else
	 begin
      insert into Stock(ProductId,Quantity) values (@ProductId,@Quantity)
     end
   end

  COMMIT TRANSACTION;

  --  returning updated purchase entry
   select purchase.*,product.ProductName from
   Purchase purchase join Product product
   on purchase.ProductId = product.Id
   where purchase.IsDeleted=0 and product.IsDeleted=0 and purchase.Id=@Id
 end try

 begin catch
   ROLLBACK TRANSACTION;
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    DECLARE @ErrorState INT = ERROR_STATE();
    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
 end catch
end

-- usp: get purchase by id
create procedure dbo.usp_GetPurchaseById @Id int
as
begin
 select purchase.*,product.ProductName from
   Purchase purchase join Product product
   on purchase.ProductId = product.Id
   where purchase.IsDeleted=0 and product.IsDeleted=0 and purchase.Id=@Id
end

-- usp: delete product

create procedure dbo.usp_DeletePurchase @Id int
as
begin
 begin transaction;
 begin try
  declare @productId int,@quantity int;

  select @productId=productId,@quantity=Quantity from Purchase where Id=@Id;

  update Purchase set IsDeleted=1 where Id=@Id;

  if exists(select 1 from stock where productId=@productId)
   begin
    update Stock set Quantity=Quantity-@quantity where ProductId=@productId;
   end
  commit transaction;
 end try
 begin catch
  rollback transaction;
  DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    DECLARE @ErrorState INT = ERROR_STATE();
    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
 end catch
end


-- usp: get purchases

create procedure dbo.usp_getPurchases
@dateFrom datetime = null,
@dateTo datetime = null,
@productName nvarchar(100) = null,
@page int =1,@limit int=4,@sortDirection nvarchar(4)='asc',@sortColumn nvarchar(50)='Id'
as
begin

select purchase.*,product.ProductName from
   Purchase purchase join Product product
   on purchase.ProductId = product.Id
   where purchase.IsDeleted=0 and product.IsDeleted=0
   AND
(
    -- Condition 1: No filter columns passed
    (
        @dateFrom IS NULL AND @dateTo IS NULL AND @productName IS NULL
    )
    OR
    -- Condition 2: User passes productName only
    (
        @dateFrom IS NULL AND @dateTo IS NULL AND @productName IS NOT NULL
        AND product.ProductName like '%'+@productName+'%'
    )
    OR
    -- Condition 3: User passes dateFrom, dateTo only
    (
        @dateFrom IS NOT NULL AND @dateTo IS NOT NULL AND @productName IS NULL
        AND purchase.PurchaseDate >= @dateFrom AND purchase.PurchaseDate <=@dateTo
    )
    OR
    -- Condition 4: User passes dateFrom, dateTo and productName together
    (
        @dateFrom IS NOT NULL AND @dateTo IS NOT NULL AND @productName IS NOT NULL
        AND purchase.PurchaseDate >= @dateFrom AND purchase.PurchaseDate <=@dateTo
        AND product.ProductName like '%'+@productName+'%'
    )
)
order by 
case when @sortColumn='Id' and @sortDirection='asc' then purchase.Id end,
case when @sortColumn='Id' and @sortDirection='desc' then purchase.Id end desc,
case when @sortColumn='ProductName' and @sortDirection='asc' then product.ProductName end,
case when @sortColumn='ProductName' and @sortDirection='desc' then product.ProductName end desc,
case when @sortColumn='Price' and @sortDirection='asc' then purchase.Price end,
case when @sortColumn='Price' and @sortDirection='desc' then purchase.Price end desc,
case when @sortColumn='PurchaseDate' and @sortDirection='asc' then purchase.PurchaseDate end,
case when @sortColumn='PurchaseDate' and @sortDirection='desc' then purchase.PurchaseDate end desc

OFFSET(@page-1)*@limit ROWS
fetch next @limit rows only;

-- second result set returns totalRecords with filter in table

select Count(purchase.Id) as TotalRecords,CAST(CEILING((count(purchase.Id)*1.0)/@limit)as int) as TotalPages from
   Purchase purchase join Product product
   on purchase.ProductId = product.Id
   where purchase.IsDeleted=0 and product.IsDeleted=0
   AND
(
    -- Condition 1: No filter columns passed
    (
        @dateFrom IS NULL AND @dateTo IS NULL AND @productName IS NULL
    )
    OR
    -- Condition 2: User passes productName only
    (
        @dateFrom IS NULL AND @dateTo IS NULL AND @productName IS NOT NULL
        AND product.ProductName like '%'+@productName+'%'
    )
    OR
    -- Condition 3: User passes dateFrom, dateTo only
    (
        @dateFrom IS NOT NULL AND @dateTo IS NOT NULL AND @productName IS NULL
        AND purchase.PurchaseDate >= @dateFrom AND purchase.PurchaseDate <=@dateTo
    )
    OR
    -- Condition 4: User passes dateFrom, dateTo and productName together
    (
        @dateFrom IS NOT NULL AND @dateTo IS NOT NULL AND @productName IS NOT NULL
        AND purchase.PurchaseDate >= @dateFrom AND purchase.PurchaseDate <=@dateTo
        AND product.ProductName like '%'+@productName+'%'
    )
)

end