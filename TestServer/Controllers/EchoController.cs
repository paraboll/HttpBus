using Microsoft.AspNetCore.Mvc;

namespace TestServer.Controllers;

[ApiController]
[Route("api/v1/")]
public class EchoController : ControllerBase
{
    private readonly ILogger<EchoController> _logger;

    public EchoController(ILogger<EchoController> logger)
    {
        _logger = logger;
    }

    [HttpGet("test")]
    public async Task<IActionResult> EchoTest()
    {
        _logger.LogWarning($"message: test");
        return Ok("Ok");
    }

    [HttpPost("test1")]
    public async Task<IActionResult> EchoTest1([FromBody] MessageRequest message)
    {
        _logger.LogWarning($"message1: {message.Message}");
        return Ok();
    }

    [HttpPost("test2")]
    public async Task<IActionResult> EchoTest2([FromBody] MessageRequest message)
    {
        _logger.LogWarning($"message2: {message.Message}");
        return Ok();
    }
}

public class MessageRequest
{
    public string Message { get; set; }
}