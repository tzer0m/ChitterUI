# ChitterUI

A Blazor Server front end for [tzer0mApi](https://github.com/tzer0m/tzer0mApi)'s Chitter module — sends print jobs to a home-mounted Aures ODP 333 receipt printer.

## Features

- **Text Printing** — free text, up to 1024 characters.
- **Image Printing** — upload a file or capture a photo from the device's camera (desktop webcams included, via `getUserMedia`), with a live preview before sending.
- **Quote / Proverb / Packing List Buttons** — one-tap prints.
- **Home geofencing** — the whole form is gated behind a browser geolocation check, so it only works from home.

All printing (rendering, dithering, and the actual send to the printer) happens server-side in tzer0mApi; ChitterUI is just the front end that calls it, authenticated with an `X-API-Key` header.

## Attribution

- Favicon (`wwwroot/favicon.png`, `wwwroot/favicon.svg`): <a href="https://www.flaticon.com/free-icons/receipt" title="receipt icons">Icon created by HAJICON - Flaticon</a>
