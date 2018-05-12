function bootstrapHome() {
    var status = document.getElementById('status');
    var imageCount = document.getElementById('imageCount');
    var imageSection = document.getElementById('imageSection');
    var imageResult = document.getElementById('imageResult');
    var imageResultTemplate = document.getElementById('imageResultTemplate');

    status_refreshStatus(
        status,
        imageCount,
        function () {
            imageSection.style.display = 'block';
            search_searchNoCriteria(imageResult, imageResultTemplate);
        });
}