function bootstrapHome() {
    var sessionId = session_getSessionId();
    var status = document.getElementById('status');
    var imageCount = document.getElementById('imageCount');

    status_refreshStatus(sessionId, status, imageCount);
}