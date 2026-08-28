// Drives a live webcam preview and lets the page grab a still frame from it, for browsers (mainly desktop) that don't route <input type="file" capture> to a camera.

export async function startCamera(videoElementId) {
    const video = document.getElementById(videoElementId);
    if (!video) {
        return false;
    }

    try {
        const stream = await navigator.mediaDevices.getUserMedia({ video: { facingMode: "environment" }, audio: false });
        video.srcObject = stream;
        await video.play();
        return true;
    } catch {
        return false;
    }
}

export function stopCamera(videoElementId) {
    const video = document.getElementById(videoElementId);
    if (!video || !video.srcObject) {
        return;
    }

    for (const track of video.srcObject.getTracks()) {
        track.stop();
    }
    video.srcObject = null;
}

export function capturePhoto(videoElementId) {
    const video = document.getElementById(videoElementId);
    if (!video || !video.videoWidth) {
        return null;
    }

    const canvas = document.createElement("canvas");
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;
    canvas.getContext("2d").drawImage(video, 0, 0);
    return canvas.toDataURL("image/jpeg", 0.85);
}