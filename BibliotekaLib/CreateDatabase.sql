USE master;
GO

CREATE DATABASE Biblioteka;
GO

USE Biblioteka;
GO

CREATE TABLE dbo.Shelfs (
    ShelfID INT PRIMARY KEY IDENTITY(1, 1),
    ShelfName NVARCHAR(64) UNIQUE
);
GO

CREATE TABLE dbo.Generes (
    GenereID INT PRIMARY KEY IDENTITY(1, 1),
    GenereName NVARCHAR(32) UNIQUE
);
GO

CREATE TABLE dbo.Books (
    BookID INT PRIMARY KEY IDENTITY(1, 1),
    Title NVARCHAR(128) NOT NULL,
    PurchaseDate DATE NOT NULL,
    GenereID INT NOT NULL,
    ReadCount INT NOT NULL DEFAULT 0,
    ShelfID INT NOT NULL,

    CONSTRAINT CHK_Books_PurchaseDate CHECK (PurchaseDate <= GETDATE()),

    CONSTRAINT CHK_Books_ReadCount CHECK (ReadCount >= 0),

    CONSTRAINT FK_Books_Generes FOREIGN KEY (GenereID)
    REFERENCES dbo.Generes(GenereID),

    CONSTRAINT FK_Books_Shelfs FOREIGN KEY (ShelfID)
    REFERENCES dbo.Shelfs(ShelfID)
);
GO

CREATE TABLE dbo.Authors (
    AuthorID INT PRIMARY KEY IDENTITY(1, 1),
    FirstName NVARCHAR(32) NOT NULL,
    LastName NVARCHAR(32),

    UNIQUE(FirstName, LastName)
);
GO

CREATE TABLE dbo.BookAuthors (
    BookID INT NOT NULL,
    AuthorID INT NOT NULL,

    PRIMARY KEY(BookID, AuthorID),

    CONSTRAINT FK_BookAuthors_Books FOREIGN KEY (BookID)
    REFERENCES dbo.Books(BookID),

    CONSTRAINT FK_BookAuthors_Authors FOREIGN KEY (AuthorID)
    REFERENCES dbo.Authors(AuthorID)
);
GO