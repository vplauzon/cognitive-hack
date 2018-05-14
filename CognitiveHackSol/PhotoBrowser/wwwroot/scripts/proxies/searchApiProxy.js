function searchApiProxy_search(criteria, onLoad, onError) {
    var request = new XMLHttpRequest();
    var inputPayload = criteria;

    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status >= 200 && this.status < 300) {
                payload = JSON.parse(this.responseText);

                onLoad(payload);
            }
            else {
                onError();
            }
        }
    };
    request.open("post", "/api/search", true);
    request.setRequestHeader("Content-Type", "application/json");
    request.setRequestHeader("Accept", "application/json");
    request.send(JSON.stringify(inputPayload));
}