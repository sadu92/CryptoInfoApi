using System.Text.Json;
using Microsoft.Data.SqlClient; 

public class CoinService
{
    private readonly string _connectionString;
    private readonly HttpClient _httpClient;

    public CoinService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection");
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; CryptoInfoApp/1.0)");
    }

    public async Task<Coin?> GetCoinAsync(string coinId)
    {
        var coin = await GetCoinFromDbAsync(coinId);
        if (coin != null) return coin;

        var url = $"https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&ids={coinId}";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<List<JsonElement>>(json);

        if (data == null || data.Count == 0) return null;

        var item = data[0];
        var newCoin = new Coin
        {
            CoinId = item.GetProperty("id").GetString(),
            Name = item.GetProperty("name").GetString(),
            Symbol = item.GetProperty("symbol").GetString(),
            CurrentPrice = item.GetProperty("current_price").GetDecimal(),
            MarketCap = item.GetProperty("market_cap").GetInt64(),
            TotalVolume = item.GetProperty("total_volume").GetInt64(),
            PriceChange24h = item.GetProperty("price_change_percentage_24h").GetSingle()
        };

        await SaveCoinToDbAsync(newCoin);
        return newCoin;
    }

    private async Task<Coin?> GetCoinFromDbAsync(string coinId)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var cmd = new SqlCommand("SELECT TOP 1 * FROM Coins WHERE CoinId = @CoinId", conn);
        cmd.Parameters.AddWithValue("@CoinId", coinId);

        using var reader = await cmd.ExecuteReaderAsync();
        if (!reader.Read()) return null;

        return new Coin
        {
            Id = (int)reader["Id"],
            CoinId = reader["CoinId"].ToString(),
            Name = reader["Name"].ToString(),
            Symbol = reader["Symbol"].ToString(),
            CurrentPrice = (decimal)reader["CurrentPrice"],
            MarketCap = (long)reader["MarketCap"],
            TotalVolume = (long)reader["TotalVolume"],
            PriceChange24h = (float)(double)reader["PriceChange24h"]
        };
    }

    private async Task SaveCoinToDbAsync(Coin coin)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var cmd = new SqlCommand(@"
            INSERT INTO Coins (CoinId, Name, Symbol, CurrentPrice, MarketCap, TotalVolume, PriceChange24h)
            VALUES (@CoinId, @Name, @Symbol, @CurrentPrice, @MarketCap, @TotalVolume, @PriceChange24h)", conn);

        cmd.Parameters.AddWithValue("@CoinId", coin.CoinId);
        cmd.Parameters.AddWithValue("@Name", coin.Name);
        cmd.Parameters.AddWithValue("@Symbol", coin.Symbol);
        cmd.Parameters.AddWithValue("@CurrentPrice", coin.CurrentPrice);
        cmd.Parameters.AddWithValue("@MarketCap", coin.MarketCap);
        cmd.Parameters.AddWithValue("@TotalVolume", coin.TotalVolume);
        cmd.Parameters.AddWithValue("@PriceChange24h", coin.PriceChange24h);

        await cmd.ExecuteNonQueryAsync();
    }
    public async Task<List<Coin>> GetAllCoinsAsync()
    {
        var coins = new List<Coin>();

        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var cmd = new SqlCommand("SELECT * FROM Coins", conn);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            coins.Add(new Coin
            {
                Id = (int)reader["Id"],
                CoinId = reader["CoinId"].ToString(),
                Name = reader["Name"].ToString(),
                Symbol = reader["Symbol"].ToString(),
                CurrentPrice = (decimal)reader["CurrentPrice"],
                MarketCap = (long)reader["MarketCap"],
                TotalVolume = (long)reader["TotalVolume"],
                PriceChange24h = (float)(double)reader["PriceChange24h"]
            });
        }

        return coins;
    }

}