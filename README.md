CryptoInfoApi (.NET Core Web API)

A self-contained .NET Core Web API project that fetches cryptocurrency data from the public CoinGecko API and stores it in a SQL Server database (without using an ORM). If data exists in the DB, it returns it; otherwise, it fetches from the API, stores it, and returns it.

📦 Features

Fetch real-time cryptocurrency data (e.g., Bitcoin)

Store data in SQL Server using raw SQL (no ORM)

RESTful API endpoints

Smart cache: avoids repeated API calls

Includes error handling

⚙️ Tech Stack

.NET Core Web API

SQL Server

CoinGecko API

No ORM (uses Microsoft.Data.SqlClient)

🚀 How to Run

1. Clone the Repository

git clone https://github.com/sadu92/CryptoInfoApi.git

2. Create SQL Database and Table

Create a database named CoinMarket, then run the following SQL to create the table:

CREATE TABLE Coins (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CoinId NVARCHAR(100),
    Name NVARCHAR(100),
    Symbol NVARCHAR(20),
    CurrentPrice DECIMAL(18,8),
    MarketCap BIGINT,
    TotalVolume BIGINT,
    PriceChange24h FLOAT
);

3. Set Connection String

Edit appsettings.json:

"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=CoinMarket;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trusted_Connection=True;TrustServerCertificate=True;;Application Intent=ReadWrite;Multi Subnet Failover=False;"
}

4. Build and Run

dotnet restore
dotnet build
dotnet run

5. Test the API

Use a tool like Postman or browser:

GET https://localhost:5001/api/coins/bitcoin

📚 API Endpoints

Endpoint

Description

GET /api/coins/{coinId}

Get coin data by CoinGecko ID

📄 Notes

The project uses raw SQL with SqlConnection and SqlCommand.

API fallback logic checks the local DB before calling CoinGecko.

No 3rd party ORM or heavy frameworks used.


