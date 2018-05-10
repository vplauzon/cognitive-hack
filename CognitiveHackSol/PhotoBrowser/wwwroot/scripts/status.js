function statusStartScript() {
    var status = document.getElementById('status');
    var documentCount = document.getElementById('documentCount');
    var sessionID = readSessionId();

    refreshStatus(sessionID, status, documentCount);
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

function refreshStatus(sessionID, status, documentCount) {
    requestStatus(
        sessionID,
        function (statusValue, documentCountValue) {
            status.textContent = statusValue;
            documentCount.textContent = documentCountValue;
        },
        function () {
            status.value = "Error on API";
        });
}

function requestStatus(sessionID, onStatus, onError) {
    var request = new XMLHttpRequest();
    var inputPayload = { "session": sessionID };

    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            lastAnalysisTime = new Date().getTime();

            if (this.status >= 200 && this.status < 300) {
                payload = JSON.parse(this.responseText);

                onStatus(payload.status, payload.documentCount);
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