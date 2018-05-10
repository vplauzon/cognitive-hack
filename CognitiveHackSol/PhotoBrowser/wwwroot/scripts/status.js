function statusStartScript() {
    var status = document.getElementById('status');
    var photoCount = document.getElementById('photoCount');
    var sessionId = readSessionId();

    refreshStatus(sessionId, status, photoCount);
}

function readSessionId() {
    var COOKIE_NAME = "PhotoBrowser.Session";
    var parts = document.cookie.split(';');
    var memory = {};

    for (var i = 0; i != parts.length; ++i) {
        var subParts = parts[i].split('=');
        var name = subParts[0].trim();
        var value = subParts[1].trim();

        if (name == COOKIE_NAME) {
            return value;
        }
    }

    return null;
}

function refreshStatus(sessionId, status, photoCount) {
    requestStatus(
        sessionId,
        function (statusValue, photoCountValue) {
            status.textContent = statusValue;
            photoCount.textContent = photoCountValue;
        },
        function () {
            status.value = "Error on API";
        });
}

function requestStatus(sessionId, onStatus, onError) {
    var request = new XMLHttpRequest();
    var inputPayload = { "session": sessionId };

    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            lastAnalysisTime = new Date().getTime();

            if (this.status >= 200 && this.status < 300) {
                payload = JSON.parse(this.responseText);

                onStatus(payload.status, payload.photoCount);
            }
            else {
                onError();
            }
        }
    };
    request.open("post", "/api/status", true);
    request.setRequestHeader("Content-Type", "application/json");
    request.setRequestHeader("Accept", "application/json");
    request.send(JSON.stringify(inputPayload));

}