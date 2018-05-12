function bootstrapHome() {
    var status = document.getElementById('status');
    var imageCount = document.getElementById('imageCount');
    var imageSection = document.getElementById('imageSection');

    status_refreshStatus(
        status,
        imageCount,
        function () {
            window.location.href = "/search";
        });
}