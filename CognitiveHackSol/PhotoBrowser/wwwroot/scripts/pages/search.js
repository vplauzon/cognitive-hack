function bootstrapSearch(tagCount) {
    var imageResult = document.getElementById('imageResult');
    var imageResultTemplate = document.getElementById('imageResultTemplate');
    var searchCount = document.getElementById('searchCount');

    search_bindCriteria(tagCount, imageResult, imageResultTemplate, searchCount);
}