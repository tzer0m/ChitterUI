using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ChitterUI.Models;

namespace ChitterUI.Services;

/// <summary>
/// Calls tzer0mApi's Chitter endpoints to send print jobs to the receipt printer.
/// </summary>
/// <param name="httpClient">The typed HTTP client, configured with tzer0mApi's base address.</param>
/// <param name="config">The configuration instance.</param>
/// <param name="logger">The logger instance.</param>
public class ChitterApiService(HttpClient httpClient, IConfiguration config, ILogger<ChitterApiService> logger)
{
    /// <summary>
    /// Api key.
    /// </summary>
    private readonly string ApiKey = config["Api:Key"] ?? throw new InvalidOperationException("Api:Key is not configured");

    /// <summary>
    /// Sends the given text to tzer0mApi's Chitter text-print endpoint.
    /// </summary>
    /// <param name="text">The text to print.</param>
    /// <returns>A result describing whether the print job was accepted, with a user-facing message on failure.</returns>
    public async Task<ChitterPrintResult> PrintTextAsync(string text)
    {
        using HttpRequestMessage request = new(HttpMethod.Post, "Chitter/Text");
        request.Content = JsonContent.Create(text);
        return await SendAsync(request, "text print");
    }

    /// <summary>
    /// Sends the given image to tzer0mApi's Chitter image-print endpoint.
    /// </summary>
    /// <param name="imageStream">The image content stream.</param>
    /// <param name="fileName">The original file name of the image.</param>
    /// <param name="contentType">The image's content type.</param>
    /// <returns>A result describing whether the print job was accepted, with a user-facing message on failure.</returns>
    public async Task<ChitterPrintResult> PrintImageAsync(Stream imageStream, string fileName, string contentType)
    {
        using HttpRequestMessage request = new(HttpMethod.Post, "Chitter/Image");

        using MultipartFormDataContent formContent = new();
        using StreamContent streamContent = new(imageStream)
        {
            Headers = { ContentType = new MediaTypeHeaderValue(contentType) },
        };
        formContent.Add(streamContent, "image", fileName);
        request.Content = formContent;

        return await SendAsync(request, "image print");
    }

    /// <summary>
    /// Sends a request to tzer0mApi, attaching the API key and translating the response (or a connectivity failure) into a <see cref="ChitterPrintResult"/>.
    /// </summary>
    /// <param name="request">The request to send.</param>
    /// <param name="description">A short description of the request, used in log messages.</param>
    private async Task<ChitterPrintResult> SendAsync(HttpRequestMessage request, string description)
    {
        try
        {
            // Attach api key.
            request.Headers.Add("X-API-Key", ApiKey);

            // Send request and check response.
            using HttpResponseMessage response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
                return new ChitterPrintResult(true, null);

            // Log the failure and resolve a user-facing message for it.
            if (logger.IsEnabled(LogLevel.Warning))
                logger.LogWarning("tzer0mApi returned {StatusCode} for a Chitter {Description} request", response.StatusCode, description);

            // Resolve a user-facing error message for the failed response and return it in the result.
            return new ChitterPrintResult(false, await ResolveErrorMessageAsync(response));
        }
        catch (Exception ex)
        {
            // Log error and return a generic connectivity message on exception.
            if (logger.IsEnabled(LogLevel.Error))
                logger.LogError(ex, "Failed to reach tzer0mApi for a Chitter {Description} request", description);
            return new ChitterPrintResult(false, "Couldn't reach the print service.");
        }
    }

    /// <summary>
    /// Resolves a user-facing error message for a failed response, preferring tzer0mApi's own error message where one is present, falling back to a generic message per status code otherwise.
    /// </summary>
    /// <param name="response">The failed response.</param>
    private static async Task<string> ResolveErrorMessageAsync(HttpResponseMessage response)
    {
        // Attempt to parse the response body as JSON and extract the "error" property.
        try
        {
            ChitterApiErrorResponse? errorResponse = await response.Content.ReadFromJsonAsync<ChitterApiErrorResponse>().ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(errorResponse?.Error))
                return errorResponse.Error;
        }
        catch (JsonException)
        {
            // Response body wasn't JSON - fall through to the generic messages below.
        }

        // Return a generic message per status code.
        return response.StatusCode switch
        {
            HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden => "Not authorized to print - check the API key configuration.",
            HttpStatusCode.RequestEntityTooLarge => "That's too large to print.",
            HttpStatusCode.BadGateway => "Failed to reach the printer.",
            _ => $"tzer0mApi returned an unexpected error ({(int)response.StatusCode}).",
        };
    }
}