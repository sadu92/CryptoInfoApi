using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CoinsController : ControllerBase
{
    private readonly CoinService _coinService;

    public CoinsController(CoinService coinService)
    {
        _coinService = coinService;
    }

    [HttpGet("{coinId}")]
    public async Task<IActionResult> GetCoin(string coinId)
    {
        var coin = await _coinService.GetCoinAsync(coinId.ToLower()); 
        if (coin == null) return NotFound();                          
        return Ok(coin);                                              
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllCoins()
    {
        var coins = await _coinService.GetAllCoinsAsync();
        return Ok(coins);
    }


}