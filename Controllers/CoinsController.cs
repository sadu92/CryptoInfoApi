using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class CoinsController : ControllerBase
{
    private readonly CoinService _coinService;

    public CoinsController(CoinService coinService)
    {
        _coinService = coinService;
    }
    [HttpGet(Name = "GetCoin")]
    public async Task<IActionResult> GetCoin(string coinId)
    {
        var coin = await _coinService.GetCoinAsync(coinId.ToLower()); 
        if (coin == null) return NotFound();                          
        return Ok(coin);                                              
    }

}