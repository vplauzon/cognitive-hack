function session_getSessionId() {
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