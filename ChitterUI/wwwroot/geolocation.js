// Wraps the browser's Geolocation API in a promise so Blazor Server's JS interop can await it. Resolves to { latitude, longitude } on success, or null if unsupported, denied, or it times out.
export function getPosition() {
    return new Promise((resolve) => {
        if (!navigator.geolocation) {
            resolve(null);
            return;
        }
        navigator.geolocation.getCurrentPosition(
            (position) => resolve({ latitude: position.coords.latitude, longitude: position.coords.longitude }),
            () => resolve(null),
            { timeout: 10000 }
        );
    });
}