//GenAI
public class Coin
{
    public int Id { get; set; }
    public string CoinId { get; set; }
    public string Name { get; set; }
    public string Symbol { get; set; }
    public decimal CurrentPrice { get; set; }
    public long MarketCap { get; set; }
    public long TotalVolume { get; set; }
    public float PriceChange24h { get; set; }
}