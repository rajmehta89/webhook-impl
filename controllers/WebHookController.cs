using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhatsappWebHook.Data;
using System.Net.Http.Headers;

using WhatsappWebHook.Models;

namespace WhatsappWebHook.Controllers
{
    [ApiController]
    [Route("webhook")]
    public class WebhookController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<WebhookController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public WebhookController(
            IConfiguration configuration,
            ILogger<WebhookController> logger,
            ApplicationDbContext dbContext)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        // GET: /webhook/messages - Return ALL WhatsApp messages ordered by ReceivedAt DESC
        [HttpGet("messages")]
        public async Task<ActionResult<List<WhatsAppMessage>>> GetAllMessages()
        {
            try
            {
                var messages = await _dbContext.WhatsAppMessages
                    .AsNoTracking()
                    .OrderByDescending(m => m.ReceivedAt)
                    .ToListAsync();

                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve WhatsApp messages from database.");
                return StatusCode(500, "Internal server error");
            }
        }

        // Optional GET method for Meta verification (can be removed if unused)
        [HttpGet]
        public IActionResult VerifyWebhook(
            [FromQuery(Name = "hub.mode")] string? hubMode,
            [FromQuery(Name = "hub.verify_token")] string? hubVerifyToken,
            [FromQuery(Name = "hub.challenge")] string? hubChallenge)
        {
            string? verifyToken = _configuration["WhatsApp:VerifyToken"];

            if (string.IsNullOrEmpty(verifyToken))
            {
                _logger.LogError("WhatsApp verify token missing.");
                return BadRequest("Verify token not configured.");
            }

            if (hubMode == "subscribe" && hubVerifyToken == verifyToken)
            {
                _logger.LogInformation("Webhook verified successfully with challenge: {HubChallenge}", hubChallenge);
                return Ok(hubChallenge);
            }

            _logger.LogWarning("Webhook verification failed. Mode: {HubMode}, Token: {HubVerifyToken} did not match.", hubMode, hubVerifyToken);
            return Unauthorized("Verification token mismatch.");
        }

        // POST: /webhook - Receive incoming WhatsApp messages from Twilio webhook and save
        // [HttpPost]
        // public async Task<IActionResult> ReceiveTwilioWebhook()
        // {
        //     try
        //     {
        //         // Safely read form parameters as nullable strings
        //         string? messageSid = Request.Form["MessageSid"];
        //         string? accountSid = Request.Form["AccountSid"];
        //         string? from = Request.Form["From"];
        //         string? to = Request.Form["To"];
        //         string? body = Request.Form["Body"];
        //         string? waId = Request.Form["WaId"];
        //         string? profileName = Request.Form["ProfileName"];
        //         string? numMediaStr = Request.Form["NumMedia"];
        //         string? apiVersion = Request.Form["ApiVersion"];
        //         string? smsStatus = Request.Form["SmsStatus"];
        //         string? smsMessageSid = Request.Form["SmsMessageSid"];
        //         string? numSegmentsStr = Request.Form["NumSegments"];
        //         string? messageType = Request.Form["MessageType"];
        //         string? channelMetadata = Request.Form["ChannelMetadata"];

        //         // Parse numeric fields safely
        //         int.TryParse(numMediaStr, out int numMedia);
        //         int.TryParse(numSegmentsStr, out int numSegments);

        //         // Initialize media variables as null; will assign only if media exists
        //         string mediaContentType = "";
        //         string mediaUrl = "";
        //         string mediaSid = "";

        //         // Check if there is any media, and assign first media item's details
        //         if (numMedia > 0)
        //         {
        //             mediaContentType = Request.Form["MediaContentType0"];
        //             mediaUrl = Request.Form["MediaUrl0"];
        //             mediaSid = Request.Form["MediaSid0"];
        //         }

        //         _logger.LogInformation("Received WhatsApp message from {From}, Body: {Body}, NumMedia: {NumMedia}, MessageType: {MessageType}",
        //             from, body, numMedia, messageType);

        //         var message = new WhatsAppMessage
        //         {
        //             MessageSid = messageSid,
        //             AccountSid = accountSid,
        //             From = from,
        //             To = to,
        //             Body = body,
        //             WaId = waId,
        //             ProfileName = profileName,
        //             NumMedia = numMedia,
        //             ApiVersion = apiVersion,
        //             SmsStatus = smsStatus,
        //             SmsMessageSid = smsMessageSid,
        //             NumSegments = numSegments,
        //             MessageType = messageType,
        //             ChannelMetadata = channelMetadata,
        //             ReceivedAt = DateTime.UtcNow,
        //             MediaContentType = mediaContentType,
        //             MediaUrl = mediaUrl,
        //             MediaSid = mediaSid
        //         };

        //         _dbContext.WhatsAppMessages.Add(message);
        //         await _dbContext.SaveChangesAsync();

        //         _logger.LogInformation("Saved WhatsApp message {MessageSid} from {From}", messageSid, from);

        //         return Ok();
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error processing Twilio WhatsApp webhook.");
        //         return BadRequest();
        //     }
        // }

        [HttpPost]
        public async Task<IActionResult> ReceiveTwilioWebhook()
        {
            try
            {
                // Fetch Twilio credentials from configuration
                string twilioAccountSid = _configuration["Twilio:AccountSid"];
                string twilioAuthToken = _configuration["Twilio:AuthToken"];

                // Read form parameters safely
                string? messageSid = Request.Form["MessageSid"];
                string? accountSid = Request.Form["AccountSid"];
                string? from = Request.Form["From"];
                string? to = Request.Form["To"];
                string? body = Request.Form["Body"];
                string? waId = Request.Form["WaId"];
                string? profileName = Request.Form["ProfileName"];
                string? numMediaStr = Request.Form["NumMedia"];
                string? apiVersion = Request.Form["ApiVersion"];
                string? smsStatus = Request.Form["SmsStatus"];
                string? smsMessageSid = Request.Form["SmsMessageSid"];
                string? numSegmentsStr = Request.Form["NumSegments"];
                string? messageType = Request.Form["MessageType"];
                string? channelMetadata = Request.Form["ChannelMetadata"];

                int.TryParse(numMediaStr, out int numMedia);
                int.TryParse(numSegmentsStr, out int numSegments);

                string mediaContentType = string.Empty;
                string mediaUrlRemote = string.Empty;
                string mediaSid = string.Empty;
                string mediaLocalPath = string.Empty;

                if (numMedia > 0)
                {
                    mediaContentType = Request.Form["MediaContentType0"];
                    mediaUrlRemote = Request.Form["MediaUrl0"];
                    mediaSid = Request.Form["MediaSid0"];

                    if (!string.IsNullOrWhiteSpace(mediaUrlRemote))
                    {
                        
                        var authToken = Convert.ToBase64String(
                            System.Text.Encoding.ASCII.GetBytes($"{twilioAccountSid}:{twilioAuthToken}"));

                        using var httpClient = new HttpClient();
                        httpClient.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Basic", authToken);

                        string fileExtension = GetFileExtensionFromContentType(mediaContentType ?? "");

                        // Generate filename with prefix and timestamp
                        string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");

                        string fileName = $"file_{timestamp}{fileExtension}";

                        string folderPath = @"D:\documents_twillio";

                        Directory.CreateDirectory(folderPath);

                        string localFilePath = Path.Combine(folderPath, fileName);

                        using var response = await httpClient.GetAsync(mediaUrlRemote, CancellationToken.None);

                        response.EnsureSuccessStatusCode();

                        await using var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write);

                        await response.Content.CopyToAsync(fileStream);

                        mediaLocalPath = localFilePath;
                    }
                    
                }

                _logger.LogInformation("Received WhatsApp message from {From}, Body: {Body}, NumMedia: {NumMedia}, MessageType: {MessageType}",
                    from, body, numMedia, messageType);

                var message = new WhatsAppMessage
                {
                    MessageSid = messageSid,
                    AccountSid = accountSid,
                    From = from,
                    To = to,
                    Body = body,
                    WaId = waId,
                    ProfileName = profileName,
                    NumMedia = numMedia,
                    ApiVersion = apiVersion,
                    SmsStatus = smsStatus,
                    SmsMessageSid = smsMessageSid,
                    NumSegments = numSegments,
                    MessageType = messageType,
                    ChannelMetadata = channelMetadata,
                    ReceivedAt = DateTime.UtcNow,
                    MediaContentType = mediaContentType,
                    MediaUrl = mediaLocalPath,  // storing local file path here
                    MediaSid = mediaSid
                };

                _dbContext.WhatsAppMessages.Add(message);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Saved WhatsApp message {MessageSid} from {From}", messageSid, from);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Twilio WhatsApp webhook.");
                return BadRequest();
            }
        }

        /// <summary>
        /// Helper to get file extension from content type.
        /// </summary>
        private string GetFileExtensionFromContentType(string contentType)
        {
            return contentType?.ToLower() switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/gif" => ".gif",
                "application/pdf" => ".pdf",
                "application/msword" => ".doc",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => ".docx",
                "text/plain" => ".txt",
                "application/zip" => ".zip",
                _ => ""
            };
        }

private string? SanitizeFileName(string? input)
{
    if (string.IsNullOrWhiteSpace(input))
        return null;

    foreach (var c in Path.GetInvalidFileNameChars())
    {
        input = input.Replace(c.ToString(), "");
    }
    return input;
}

    }
}
