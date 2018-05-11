function status_refreshStatus(sessionId, status, imageCount, onReady) {
    statusApiProxy_requestStatus(
        sessionId,
        function (statusValue, imageCountValue) {
            status.textContent = statusValue;
            imageCount.textContent = imageCountValue;
            if (statusValue === "ready") {
                onReady();
            }
            else {
                setTimeout(
                    function () {
                        status_refreshStatus(sessionId, status, imageCount, onReady);
                    },
                    1000);
            }
        },
        function () {
            status.value = "Error on API";
            setTimeout(
                function () {
                    status_refreshStatus(sessionId, status, imageCount, onReady);
                },
                1000);
        });
}