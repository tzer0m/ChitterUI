namespace ChitterUI;

/// <summary>
/// Shared limits used across the app.
/// </summary>
public static class UploadLimits
{
    /// <summary>
    /// The maximum size, in bytes, of an uploaded photo. Applied both to the SignalR circuit's max message size (Program.cs) and to InputFile's own read limit (Home.razor) - the two need to agree, since whichever is smaller is the one that actually takes effect.
    /// </summary>
    public const long MaxImageSizeBytes = 15 * 1024 * 1024;
}