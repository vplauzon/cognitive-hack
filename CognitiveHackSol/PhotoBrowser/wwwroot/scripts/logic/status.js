function status_refreshStatus(sessionId, status, imageCount) {
    statusApiProxy_requestStatus(
        sessionId,
        function (statusValue, imageCountValue) {
            status.textContent = statusValue;
            imageCount.textContent = imageCountValue;
            if (statusValue != "ready") {
                setTimeout(
                    function () {
                        status_refreshStatus(sessionId, status, imageCount);
                    },
                    500);
            }
        },
        function () {
            status.value = "Error on API";
            setTimeout(
                function () {
                    status_refreshStatus(sessionId, status, imageCount);
                },
                1000);
        });
}