namespace ChitterUI.Models;

/// <summary>
/// The outcome of a print request, including a user-facing error message when it failed.
/// </summary>
/// <param name="Success">Whether the print job was accepted.</param>
/// <param name="ErrorMessage">A user-facing description of what went wrong, set only when <see cref="Success"/> is false.</param>
public record ChitterPrintResult(bool Success, string? ErrorMessage);