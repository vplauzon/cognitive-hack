function bootstrapHome() {
    var sessionId = session_getSessionId();
    var status = document.getElementById('status');
    var imageCount = document.getElementById('imageCount');
    var photoSection = document.getElementById('photoSection');

    status_refreshStatus(
        sessionId,
        status,
        imageCount,
        function () {
            photoSection.style.display = 'block';
        });
}