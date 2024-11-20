using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace UrlShortener.Api.Controllers;

[ApiController]
public class UrlShortenerController : ControllerBase
{
    private readonly IDatabase _urlDatabase;
    private readonly IConfiguration _configuration;
    private const int UrlTimeToLiveDays = 7;

    public UrlShortenerController(IConnectionMultiplexer connectionMultiplexer, IConfiguration configuration)
    {
        _configuration = configuration;
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

        _urlDatabase.StringSet(urlKey, url, TimeSpan.FromDays(UrlTimeToLiveDays));
        
        return urlKey;
    }

    [HttpGet("{id}")]
    public ActionResult GetRedirectUrl(string id)
    {
        var urlValue = _urlDatabase.StringGet(id);
        if (urlValue.IsNullOrEmpty)
            return Redirect(_configuration["Frontend:InvalidUrl"]);
        _urlDatabase.StringSet(id, urlValue, TimeSpan.FromDays(UrlTimeToLiveDays));
        return Redirect(urlValue.ToString());
    }
}