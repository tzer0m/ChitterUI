using System.Text.Json.Serialization;

namespace ChitterUI.Models;

/// <summary>
/// The shape of tzer0mApi's JSON error responses, e.g. <c>{ "error": "..." }</c>.
/// </summary>
/// <param name="Error">The error message.</param>
internal record ChitterApiErrorResponse([property: JsonPropertyName("error")] string? Error);