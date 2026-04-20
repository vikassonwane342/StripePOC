
//CREATE TABLE Orders (
//    Id INT IDENTITY PRIMARY KEY,
//    OrderId UNIQUEIDENTIFIER,
//    Amount DECIMAL(10,2),
//    Status VARCHAR(50),   --Created,Success,Failed
//    CreatedAt DATETIME DEFAULT GETDATE()
//);

//CREATE TABLE StripePayments (
//    Id INT IDENTITY PRIMARY KEY,
//    OrderId UNIQUEIDENTIFIER,
//    PaymentIntentId VARCHAR(100),
//    Status VARCHAR(50),
//    CreatedAt DATETIME DEFAULT GETDATE()
//);