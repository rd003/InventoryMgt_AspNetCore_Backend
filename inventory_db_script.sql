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
Price decimal not null
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
[Description] nvarchar(100) not null
)