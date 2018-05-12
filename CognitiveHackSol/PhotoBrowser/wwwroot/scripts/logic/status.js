function status_refreshStatus(status, imageCount, onReady) {
    statusApiProxy_requestStatus(
        function (statusValue, imageCountValue) {
            status.textContent = statusValue;
            imageCount.textContent = imageCountValue;
            if (statusValue === "ready") {
                onReady();
            }
            else {
                setTimeout(
                    function () {
                        status_refreshStatus(status, imageCount, onReady);
                    },
                    1000);
            }
        },
        function () {
            status.value = "Error on API";
            setTimeout(
                function () {
                    status_refreshStatus(status, imageCount, onReady);
                },
                1000);
        });
}