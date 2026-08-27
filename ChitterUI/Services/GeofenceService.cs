namespace ChitterUI.Services;

/// <summary>
/// Checks whether a given coordinate is within range of home, gating access without requiring a login.
/// </summary>
/// <param name="config">The configuration instance.</param>
public class GeofenceService(IConfiguration config)
{
    // Home's coordinates and the allowed radius are kept out of source control - see secrets.json.
    /// <summary>
    /// The latitude of home, in degrees.
    /// </summary>
    private readonly double HomeLatitude = config.GetValue<double?>("Geofence:HomeLatitude") ?? throw new InvalidOperationException("Geofence:HomeLatitude is not configured");

    /// <summary>
    /// The longitude of home, in degrees.
    /// </summary>
    private readonly double HomeLongitude = config.GetValue<double?>("Geofence:HomeLongitude") ?? throw new InvalidOperationException("Geofence:HomeLongitude is not configured");

    /// <summary>
    /// The radius around home, in metres, within which coordinates are considered "in range".
    /// </summary>
    private readonly double RadiusMeters = config.GetValue<double?>("Geofence:RadiusMeters") ?? 150;

    /// <summary>
    /// The radius of the Earth, in metres, used for distance calculations.
    /// </summary>
    private const double EarthRadiusMeters = 6371000;

    /// <summary>
    /// Checks whether the given coordinate is within the configured radius of home.
    /// </summary>
    /// <param name="latitude">The coordinate's latitude.</param>
    /// <param name="longitude">The coordinate's longitude.</param>
    /// <returns>True if the coordinate is within range of home, false otherwise.</returns>
    public bool IsWithinRange(double latitude, double longitude) => DistanceMeters(HomeLatitude, HomeLongitude, latitude, longitude) <= RadiusMeters;

    /// <summary>
    /// Calculates the great-circle distance between two coordinates, in metres, using the Haversine formula.
    /// </summary>
    private static double DistanceMeters(double lat1, double lon1, double lat2, double lon2)
    {
        double lat1Rad = DegreesToRadians(lat1);
        double lat2Rad = DegreesToRadians(lat2);
        double deltaLatRad = DegreesToRadians(lat2 - lat1);
        double deltaLonRad = DegreesToRadians(lon2 - lon1);
        double a = (Math.Sin(deltaLatRad / 2) * Math.Sin(deltaLatRad / 2)) + (Math.Cos(lat1Rad) * Math.Cos(lat2Rad) * Math.Sin(deltaLonRad / 2) * Math.Sin(deltaLonRad / 2));
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EarthRadiusMeters * c;
    }

    /// <summary>
    /// Converts degrees to radians.
    /// </summary>
    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;
}