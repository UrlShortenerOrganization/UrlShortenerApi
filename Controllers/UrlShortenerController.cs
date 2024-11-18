using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace UrlShortener.Api.Controllers;

[ApiController]
public class UrlShortenerController : ControllerBase
{
    private readonly IDatabase _urlDatabase;

    public UrlShortenerController(IConnectionMultiplexer connectionMultiplexer)
    {
        _urlDatabase = connectionMultiplexer.GetDatabase();
    }

    [HttpPost("generate")]
    public ActionResult<string> GenerateUrl(string url)
    {
        var salt = 0;
        string urlKey;

        do
        {
            urlKey = HashService.Get8LengthHash(url + salt++);
        } while (!_urlDatabase.StringGet(urlKey).IsNullOrEmpty);

        _urlDatabase.StringSet(urlKey, url);
        
        return urlKey;
    }

    [HttpGet("{url}")]
    public ActionResult GetRedirectUrl(string url)
    {
        var urlValue = _urlDatabase.StringGet(url);
        if (urlValue.IsNullOrEmpty)
            return BadRequest("Url not found");
        return Redirect(urlValue.ToString());
    }
}