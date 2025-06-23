using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Text.Json;
using System.Text.Json.Nodes;

[ApiController]
[Route("webhook")]
public class WebhookController : ControllerBase
{

    private readonly IConfiguration _configuration;

    private readonly ILogger<WebhookController> _logger;

    public WebhookController(IConfiguration configuration, ILogger<WebhookController> logger)
    {

        _configuration = configuration;

        _logger = logger;

    }

    // Step 1: Verification GET request
    [HttpGet]
    public IActionResult VerifyWebhook([FromQuery] string hub_mode, [FromQuery] string hub_verify_token, [FromQuery] string hub_challenge)
    {
        var verifyToken = _configuration["WhatsApp:VerifyToken"];
        
        if (string.IsNullOrEmpty(verifyToken))
        {
            
            _logger.LogError("WhatsApp verify token is not configured");

            return BadRequest("Verify token not configured");

        }

        if (hub_mode == "subscribe" && hub_verify_token == verifyToken)
        {
            return Ok(hub_challenge);
        }

        return Unauthorized("Verification token mismatch");

    }
[HttpPost]
public IActionResult ReceiveWebhook([FromBody] JsonElement body)
{
    _logger.LogInformation("Incoming webhook: {Body}", JsonSerializer.Serialize(body));

    try
    {
        // Get "entry" array
        JsonElement entryElement = body.GetProperty("entry");

        foreach (JsonElement entry in entryElement.EnumerateArray())
        {
            // Each entry contains "changes"
            JsonElement changes = entry.GetProperty("changes");

            foreach (JsonElement change in changes.EnumerateArray())
            {
                JsonElement value = change.GetProperty("value");

                string messagingProduct = value.GetProperty("messaging_product").GetString();

                // Extract metadata
                JsonElement metadata = value.GetProperty("metadata");
                string displayPhoneNumber = metadata.GetProperty("display_phone_number").GetString();
                string phoneNumberId = metadata.GetProperty("phone_number_id").GetString();

                // Extract contact info
                JsonElement contacts = value.GetProperty("contacts")[0];
                string userName = contacts.GetProperty("profile").GetProperty("name").GetString();
                string waId = contacts.GetProperty("wa_id").GetString();

                // Extract message
                JsonElement messages = value.GetProperty("messages")[0];
                string from = messages.GetProperty("from").GetString();
                string messageId = messages.GetProperty("id").GetString();
                string timestamp = messages.GetProperty("timestamp").GetString();
                string messageType = messages.GetProperty("type").GetString();
                string messageText = messages.GetProperty("text").GetProperty("body").GetString();

                // Now you can store these in DB or process further


            }
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to process incoming webhook.");
        return BadRequest();
    }

    return Ok();
}


}
