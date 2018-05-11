function statusStartScript() {
    var status = document.getElementById('status');
    var imageCount = document.getElementById('imageCount');
    var sessionId = readSessionId();

    refreshStatus(sessionId, status, imageCount);
}

function readSessionId() {
    var COOKIE_NAME = "imageBrowser.Session";
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

function refreshStatus(sessionId, status, imageCount) {
    requestStatus(
        sessionId,
        function (statusValue, imageCountValue) {
            status.textContent = statusValue;
            imageCount.textContent = imageCountValue;
            setTimeout(
                function () {
                    refreshStatus(sessionId, status, imageCount);
                },
                1000);
        },
        function () {
            status.value = "Error on API";
            setTimeout(
                function () {
                    refreshStatus(sessionId, status, imageCount);
                },
                1000);
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

                onStatus(payload.status, payload.imageCount);
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