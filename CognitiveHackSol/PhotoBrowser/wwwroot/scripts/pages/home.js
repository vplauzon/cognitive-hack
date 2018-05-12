function bootstrapHome() {
    var status = document.getElementById('status');
    var imageCount = document.getElementById('imageCount');
    var imageSection = document.getElementById('imageSection');
    var imageResult = document.getElementById('imageResult');
    var imageResultTemplate = document.getElementById('imageResultTemplate');
    var tagList = document.getElementById('tagList');
    var tagTemplate = document.getElementById('tagTemplate');

    status_refreshStatus(
        status,
        imageCount,
        function () {
            imageSection.style.display = 'block';
            search_searchNoCriteria(imageResult, imageResultTemplate);
            searchCriteria_bootstrap(tagList, tagTemplate);
        });
}