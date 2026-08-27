namespace ChitterUI.Models;

/// <summary>
/// A coordinate returned by the browser's Geolocation API.
/// </summary>
/// <param name="Latitude">The coordinate's latitude.</param>
/// <param name="Longitude">The coordinate's longitude.</param>
public record GeoPosition(double Latitude, double Longitude);