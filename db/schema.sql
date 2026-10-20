
USE master;
GO

IF EXISTS (SELECT 1 FROM sys.databases WHERE name = N'DB_Markov_Matveev')
BEGIN
    ALTER DATABASE DB_Markov_Matveev SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE DB_Markov_Matveev;
END
GO

CREATE DATABASE DB_Markov_Matveev;
GO

USE DB_Markov_Matveev;
GO


CREATE TABLE Users (
    ID       INT IDENTITY(1,1) PRIMARY KEY,
    Login    NVARCHAR(50)  NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,   -- SHA1-хэш пароля
    Role     NVARCHAR(20)  NOT NULL
        CONSTRAINT CK_Users_Role CHECK (Role IN (N'User', N'Admin')),
    FIO      NVARCHAR(150) NOT NULL,
    Photo    NVARCHAR(260) NULL
);
GO


CREATE TABLE ClientTypes (
    ClientTypeId INT IDENTITY(1,1) PRIMARY KEY,
    Name         NVARCHAR(20) NOT NULL UNIQUE
);
GO

CREATE TABLE Cities (
    CityId INT IDENTITY(1,1) PRIMARY KEY,
    Name   NVARCHAR(100) NOT NULL UNIQUE,
    Region NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE OrderStatuses (
    OrderStatusId INT IDENTITY(1,1) PRIMARY KEY,
    Name          NVARCHAR(30) NOT NULL UNIQUE
);
GO

CREATE TABLE CargoTypes (
    CargoTypeId INT IDENTITY(1,1) PRIMARY KEY,
    Name        NVARCHAR(30) NOT NULL UNIQUE,
    Description NVARCHAR(200) NULL
);
GO


CREATE TABLE Clients (
    ClientId     INT IDENTITY(1,1) PRIMARY KEY,
    ClientTypeId INT NOT NULL,
    CityId       INT NOT NULL,
    Name         NVARCHAR(150) NOT NULL,
    Phone        NVARCHAR(20) NOT NULL,
    Email        NVARCHAR(100) NULL,
    Address      NVARCHAR(200) NOT NULL,
    CONSTRAINT FK_Clients_ClientTypes FOREIGN KEY (ClientTypeId) REFERENCES ClientTypes(ClientTypeId),
    CONSTRAINT FK_Clients_Cities FOREIGN KEY (CityId) REFERENCES Cities(CityId)
);
GO


CREATE TABLE Warehouses (
    WarehouseId INT IDENTITY(1,1) PRIMARY KEY,
    CityId      INT NOT NULL,
    Name        NVARCHAR(100) NOT NULL,
    Address     NVARCHAR(200) NOT NULL,
    Capacity    DECIMAL(10,2) NOT NULL CONSTRAINT CK_Warehouses_Capacity CHECK (Capacity > 0),
    CONSTRAINT FK_Warehouses_Cities FOREIGN KEY (CityId) REFERENCES Cities(CityId)
);
GO


CREATE TABLE Orders (
    OrderId         INT IDENTITY(1,1) PRIMARY KEY,
    ClientId        INT NOT NULL,
    WarehouseId     INT NULL,
    OrderStatusId   INT NOT NULL,
    PickupAddress   NVARCHAR(200) NOT NULL,
    DeliveryAddress NVARCHAR(200) NOT NULL,
    OrderDate       DATE NOT NULL CONSTRAINT DF_Orders_OrderDate DEFAULT (CAST(GETDATE() AS DATE)),
    PlannedDate     DATE NULL,
    ActualDate      DATE NULL,
    Cost            DECIMAL(10,2) NOT NULL CONSTRAINT CK_Orders_Cost CHECK (Cost >= 0),
    CONSTRAINT FK_Orders_Clients FOREIGN KEY (ClientId) REFERENCES Clients(ClientId) ON DELETE CASCADE,
    CONSTRAINT FK_Orders_Warehouses FOREIGN KEY (WarehouseId) REFERENCES Warehouses(WarehouseId) ON DELETE SET NULL,
    CONSTRAINT FK_Orders_OrderStatuses FOREIGN KEY (OrderStatusId) REFERENCES OrderStatuses(OrderStatusId)
);
GO


CREATE TABLE Cargo (
    CargoId     INT IDENTITY(1,1) PRIMARY KEY,
    OrderId     INT NOT NULL,
    CargoTypeId INT NOT NULL,
    Name        NVARCHAR(100) NOT NULL,
    WeightKg    DECIMAL(8,2) NOT NULL CONSTRAINT CK_Cargo_Weight CHECK (WeightKg > 0),
    VolumeM3    DECIMAL(8,3) NOT NULL CONSTRAINT CK_Cargo_Volume CHECK (VolumeM3 > 0),
    CONSTRAINT FK_Cargo_Orders FOREIGN KEY (OrderId) REFERENCES Orders(OrderId) ON DELETE CASCADE,
    CONSTRAINT FK_Cargo_CargoTypes FOREIGN KEY (CargoTypeId) REFERENCES CargoTypes(CargoTypeId)
);
GO




INSERT INTO Users (Login, Password, Role, FIO, Photo) VALUES
(N'admin', N'F865B53623B121FD34EE5426C792E5C33AF8C227', N'Admin', N'Марков Андрей Владимирович', NULL),
(N'user',  N'95C946BF622EF93B0A211CD0FD028DFDFCF7E39E', N'User',  N'Иванов Сергей Петрович', NULL);
GO

INSERT INTO ClientTypes (Name) VALUES (N'Физлицо'), (N'Юрлицо');
GO

INSERT INTO Cities (Name, Region) VALUES
(N'Москва', N'Московская область'),
(N'Санкт-Петербург', N'Ленинградская область'),
(N'Екатеринбург', N'Свердловская область'),
(N'Новосибирск', N'Новосибирская область');
GO

INSERT INTO OrderStatuses (Name) VALUES
(N'Оформлен'), (N'В обработке'), (N'В пути'), (N'Доставлен'), (N'Отменён');
GO

INSERT INTO CargoTypes (Name, Description) VALUES
(N'Обычный', N'Грузы без особых условий перевозки'),
(N'Хрупкий', N'Требует аккуратной погрузки и фиксации'),
(N'Опасный', N'Требует специального разрешения и маркировки'),
(N'Скоропортящийся', N'Требует температурного режима и сжатых сроков');
GO

INSERT INTO Clients (ClientTypeId, CityId, Name, Phone, Email, Address) VALUES
(1, 1, N'Иванов Сергей Петрович',  N'+7 900 111-22-33', N'ivanov@mail.ru',     N'г. Москва, ул. Ленина, д. 5'),
(2, 1, N'ООО "ТехноСнаб"',         N'+7 495 222-33-44', N'info@technosnab.ru', N'г. Москва, Складской пр-д, д. 12'),
(1, 2, N'Петрова Анна Викторовна', N'+7 900 333-44-55', N'petrova@mail.ru',    N'г. Санкт-Петербург, Невский пр., д. 30'),
(2, 2, N'ИП Кузнецов А.А.',        N'+7 812 444-55-66', N'kuznetsov@shop.ru',  N'г. Санкт-Петербург, ул. Гороховая, д. 8'),
(2, 3, N'ООО "УралТранс"',         N'+7 343 555-66-77', N'info@uraltrans.ru',  N'г. Екатеринбург, ул. Промышленная, д. 2');
GO

INSERT INTO Warehouses (CityId, Name, Address, Capacity) VALUES
(1, N'Склад "Москва-Центральный"',    N'г. Москва, Складской пр-д, д. 1',             5000.00),
(2, N'Склад "Санкт-Петербург-Север"', N'г. Санкт-Петербург, Индустриальная ул., д. 4', 3200.00),
(3, N'Склад "Екатеринбург"',          N'г. Екатеринбург, ул. Промышленная, д. 10',     2100.00);
GO


INSERT INTO Orders (ClientId, WarehouseId, OrderStatusId, PickupAddress, DeliveryAddress, OrderDate, PlannedDate, ActualDate, Cost) VALUES
(1, 1, 4, N'г. Москва, ул. Ленина, д. 5',           N'г. Санкт-Петербург, Невский пр., д. 30',      '2026-09-01', '2026-09-05', '2026-09-05', 4500.00),
(2, 1, 3, N'г. Москва, Складской пр-д, д. 12',      N'г. Екатеринбург, ул. Ленина, д. 20',          '2026-09-10', '2026-09-15', NULL,         12300.00),
(3, 2, 1, N'г. Санкт-Петербург, Невский пр., д. 30', N'г. Москва, ул. Ленина, д. 5',                '2026-09-20', '2026-09-24', NULL,         4500.00),
(4, 2, 2, N'г. Санкт-Петербург, ул. Гороховая, д. 8', N'г. Екатеринбург, ул. Промышленная, д. 10',  '2026-09-25', NULL,         NULL,         8900.00),
(5, 3, 4, N'г. Екатеринбург, ул. Промышленная, д. 2', N'г. Москва, Складской пр-д, д. 12',          '2026-08-15', '2026-08-20', '2026-08-19', 15700.00),
(1, 1, 5, N'г. Москва, ул. Ленина, д. 5',           N'г. Новосибирск, Красный пр-т, д. 1',          '2026-07-01', '2026-07-10', NULL,         21000.00);
GO


INSERT INTO Cargo (OrderId, CargoTypeId, Name, WeightKg, VolumeM3) VALUES
(1, 1, N'Документы',          2.50,   0.010),
(2, 2, N'Бытовая техника',    120.00, 1.800),
(2, 2, N'Комплектующие ПК',   45.00,  0.600),
(3, 1, N'Одежда',             30.00,  0.400),
(4, 1, N'Стройматериалы',     500.00, 3.200),
(5, 4, N'Продукты питания',   350.00, 2.100),
(6, 3, N'Химические реагенты', 80.00, 0.500);
GO
